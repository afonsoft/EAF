using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Replica roles e permissões do host para tenants.
    /// </summary>
    public interface ITenantRolePermissionReplicationService : IDomainService
    {
        /// <summary>
        /// Garante que a role exista no tenant e atribui as permissões informadas.
        /// </summary>
        Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames);

        /// <summary>
        /// Copia as permissões da role host para a role do tenant.
        /// </summary>
        Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName);
    }
}
