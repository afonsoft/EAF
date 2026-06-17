using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Eaf.Middleware.Authorization;
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
        /// <summary>
        /// TenantAppService.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public TenantAppService()
        {
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

            return new PagedResultDto<TenantListDto>(tenantCount, ObjectMapper.Map<List<TenantListDto>>(tenants));
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
    }
}