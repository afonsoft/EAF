using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Url;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de Tenant.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Tenants)]
    public class TenantAppService : MiddlewareAppServiceBase, ITenantAppService
    {
        private readonly IRepository<SubscribableEdition, int> _editionRepository;

        /// <summary>
        /// TenantAppService.
        /// </summary>
        /// <param name="editionRepository">Repositório de edições.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantAppService(IRepository<SubscribableEdition, int> editionRepository = null)
        {
            _editionRepository = editionRepository;
            AppUrlService = NullAppUrlService.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Obtém ou define AppUrlService.
        /// </summary>
        public IAppUrlService AppUrlService { get; set; }

        /// <summary>
        /// Obtém ou define EventBus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Create)]
        [UnitOfWork(IsDisabled = true)]
        public async Task CreateTenant(CreateTenantInput input)
        {
            await TenantManager.CreateWithAdminUserAsync(input.TenancyName,
                input.Name,
                input.AdminPassword,
                input.AdminEmailAddress,
                input.IsActive,
                input.ShouldChangePasswordOnNextLogin,
                input.SendActivationEmail,
                AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName)
            );
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Delete)]
        public async Task DeleteTenant(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            await TenantManager.DeleteAsync(tenant);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_ChangeFeatures)]
        public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
            var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

            return new GetTenantFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Edit)]
        public async Task<TenantEditDto> GetTenantForEdit(EntityDto input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
            return tenantEditDto;
        }

        /// <summary>
        /// GetTenants.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input)
        {
            var query = TenantManager.Tenants
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter));

            var tenantCount = await query.CountAsync();
            var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            var dtos = ObjectMapper.Map<List<TenantListDto>>(tenants);
            await FillEditionDisplayNamesAsync(dtos);

            return new PagedResultDto<TenantListDto>(tenantCount, dtos);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_ChangeFeatures)]
        public async Task ResetTenantSpecificFeatures(EntityDto input)
        {
            await TenantManager.ResetAllFeaturesAsync(input.Id);
        }

        /// <summary>
        /// UnlockTenantAdmin.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task UnlockTenantAdmin(EntityDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(input.Id))
            {
                var tenantAdmin = await UserManager.FindByNameAsync(AbpUserBase.AdminUserName);
                if (tenantAdmin != null)
                {
                    tenantAdmin.Unlock();
                }
            }
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Edit)]
        public async Task UpdateTenant(TenantEditDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            ObjectMapper.Map(input, tenant);

            await TenantManager.UpdateAsync(tenant);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_ChangeFeatures)]
        public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
        {
            await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }

        /// <summary>
        /// Obtém a assinatura de um tenant.
        /// </summary>
        /// <param name="input">Identificador do tenant.</param>
        /// <returns>Assinatura do tenant.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Subscription)]
        public async Task<TenantSubscriptionDto> GetTenantSubscriptionAsync(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            var edition = tenant.EditionId.HasValue ? await _editionRepository.GetAsync(tenant.EditionId.Value) : null;

            return MapToSubscriptionDto(tenant, edition);
        }

        /// <summary>
        /// Atribui uma edição a um tenant, calculando a data de expiração.
        /// </summary>
        /// <param name="input">Dados da atribuição.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Subscription)]
        public async Task AssignEditionToTenantAsync(AssignEditionToTenantInput input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.TenantId);
            var edition = await _editionRepository.GetAsync(input.EditionId);

            tenant.EditionId = edition.Id;
            tenant.SubscriptionEndDateUtc = CalculateEndDate(Clock.Now, input.PaymentPeriodType);

            await TenantManager.UpdateAsync(tenant);
        }

        /// <summary>
        /// Estende a assinatura de um tenant a partir da data atual ou da expiração existente.
        /// </summary>
        /// <param name="input">Dados da extensão.</param>
        /// <returns>Task.</returns>
        [AbpAuthorize(MiddlewarePermissions.Pages_Tenants_Subscription)]
        public async Task ExtendTenantSubscriptionAsync(ExtendTenantSubscriptionInput input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.TenantId);
            var start = tenant.SubscriptionEndDateUtc.HasValue && tenant.SubscriptionEndDateUtc.Value > Clock.Now
                ? tenant.SubscriptionEndDateUtc.Value
                : Clock.Now;

            tenant.SubscriptionEndDateUtc = CalculateEndDate(start, input.PaymentPeriodType);
            await TenantManager.UpdateAsync(tenant);
        }

        private static DateTime? CalculateEndDate(DateTime start, PaymentPeriodType period)
        {
            return period switch
            {
                PaymentPeriodType.Daily => start.AddDays(1),
                PaymentPeriodType.Weekly => start.AddDays(7),
                PaymentPeriodType.Monthly => start.AddMonths(1),
                PaymentPeriodType.Quarterly => start.AddMonths(3),
                PaymentPeriodType.Biannual => start.AddMonths(6),
                PaymentPeriodType.Annual => start.AddYears(1),
                PaymentPeriodType.Permanent => null,
                _ => start,
            };
        }

        private TenantSubscriptionDto MapToSubscriptionDto(Tenant tenant, SubscribableEdition edition)
        {
            var remainingDays = tenant.SubscriptionEndDateUtc.HasValue
                ? (int)(tenant.SubscriptionEndDateUtc.Value.Date - Clock.Now.Date).TotalDays
                : (int?)null;

            return new TenantSubscriptionDto
            {
                TenantId = tenant.Id,
                EditionId = tenant.EditionId,
                EditionDisplayName = edition?.DisplayName,
                SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc,
                RemainingDays = remainingDays,
                IsActive = tenant.IsActive && (remainingDays == null || remainingDays >= 0)
            };
        }

        private async Task FillEditionDisplayNamesAsync(List<TenantListDto> dtos)
        {
            if (_editionRepository == null || dtos.Count == 0)
            {
                return;
            }

            var editionIds = dtos.Where(d => d.EditionId.HasValue).Select(d => d.EditionId.Value).Distinct().ToList();
            if (editionIds.Count == 0)
            {
                return;
            }

            var editions = await _editionRepository.GetAllListAsync(e => editionIds.Contains(e.Id));
            var editionNames = editions.ToDictionary(e => e.Id, e => e.DisplayName);

            foreach (var dto in dtos.Where(d => d.EditionId.HasValue))
            {
                if (editionNames.TryGetValue(dto.EditionId.Value, out var name))
                {
                    dto.EditionDisplayName = name;
                }
            }
        }
    }
}
