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
    }
}