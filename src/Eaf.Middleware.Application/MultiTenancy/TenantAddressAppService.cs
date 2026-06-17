using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.MultiTenancy.Dto;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de TenantAddress.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Tenants)]
    public class TenantAddressAppService : AsyncCrudAppService<TenantAddress, TenantAddressDto>
    {
        /// <summary>
        /// TenantAddressAppService.
        /// </summary>
        /// <param name="repository">Parâmetro repository.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantAddressAppService(IRepository<TenantAddress, int> repository) : base(repository)
        {
        }
    }
}