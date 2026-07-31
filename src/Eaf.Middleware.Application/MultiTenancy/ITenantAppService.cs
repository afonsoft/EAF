using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Representa a interface ITenantAppService.
    /// </summary>
    public interface ITenantAppService : IApplicationService
    {
        Task CreateTenant(CreateTenantInput input);

        Task DeleteTenant(EntityDto input);

        Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input);

        Task<TenantEditDto> GetTenantForEdit(EntityDto input);

        Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input);

        Task ResetTenantSpecificFeatures(EntityDto input);

        Task UnlockTenantAdmin(EntityDto input);

        Task UpdateTenant(TenantEditDto input);

        Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

        /// <summary>
        /// Obtém a assinatura de um tenant.
        /// </summary>
        Task<TenantSubscriptionDto> GetTenantSubscriptionAsync(EntityDto input);

        /// <summary>
        /// Atribui uma edição a um tenant.
        /// </summary>
        Task AssignEditionToTenantAsync(AssignEditionToTenantInput input);

        /// <summary>
        /// Estende a assinatura de um tenant.
        /// </summary>
        Task ExtendTenantSubscriptionAsync(ExtendTenantSubscriptionInput input);
    }
}
