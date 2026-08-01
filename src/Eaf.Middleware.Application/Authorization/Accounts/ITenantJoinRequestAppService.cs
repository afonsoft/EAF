using Abp.Application.Services;
using Eaf.Middleware.Authorization.Accounts.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Accounts
{
    /// <summary>
    /// Serviço de aplicação para solicitações de ingresso em tenants.
    /// </summary>
    public interface ITenantJoinRequestAppService : IApplicationService
    {
        /// <summary>
        /// Retorna a lista de tenants ativos disponíveis para solicitação de ingresso.
        /// </summary>
        Task<List<AvailableTenantDto>> GetAvailableTenantsAsync();

        /// <summary>
        /// Cria uma solicitação de ingresso em um tenant.
        /// </summary>
        Task<TenantJoinRequestDto> CreateRequestAsync(CreateTenantJoinRequestInput input);

        /// <summary>
        /// Retorna as solicitações do usuário logado.
        /// </summary>
        Task<List<TenantJoinRequestDto>> GetMyRequestsAsync();

        /// <summary>
        /// Retorna as solicitações pendentes do tenant atual (admin).
        /// </summary>
        Task<List<TenantJoinRequestDto>> GetPendingRequestsForCurrentTenantAsync();

        /// <summary>
        /// Aprova ou rejeita uma solicitação de ingresso.
        /// </summary>
        Task ApproveAsync(ApproveTenantJoinRequestInput input);
    }
}
