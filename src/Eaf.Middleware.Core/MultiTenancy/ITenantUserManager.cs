using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Gerencia memberships entre usuários host e tenants, garantindo shadow users e replicação de roles.
    /// </summary>
    public interface ITenantUserManager : IDomainService
    {
        /// <summary>
        /// Garante que o usuário host tenha uma membership no tenant, criando/atualizando o shadow user.
        /// </summary>
        Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false);

        /// <summary>
        /// Cria um shadow user inativo e uma solicitação de ingresso pendente no tenant.
        /// </summary>
        Task<TenantJoinRequest> CreatePendingMembershipAsync(long hostUserId, int tenantId, string message = null, string plainPassword = null);

        /// <summary>
        /// Ativa o shadow user de uma solicitação aprovada e cria a membership.
        /// </summary>
        Task<UserTenantMembership> ApproveMembershipAsync(long requestId, long approverUserId);

        /// <summary>
        /// Remove a membership do usuário host no tenant informado.
        /// </summary>
        Task RemoveMembershipAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Define o tenant padrão para o usuário host.
        /// </summary>
        Task SetDefaultAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Retorna o id do shadow user dentro do tenant.
        /// </summary>
        Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Lista todas as memberships de um usuário host.
        /// </summary>
        Task<IReadOnlyList<UserTenantMembership>> GetMembershipsAsync(long hostUserId);
    }
}
