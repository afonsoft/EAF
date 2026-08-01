using Abp.Application.Services.Dto;
using Eaf.Middleware.MultiTenancy;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa uma solicitação de ingresso em tenant.
    /// </summary>
    public class TenantJoinRequestDto : CreationAuditedEntityDto<long>
    {
        /// <summary>
        /// Id do usuário solicitante.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Nome do usuário solicitante.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Id do tenant solicitado.
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Nome do tenant solicitado.
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Id do shadow user criado no tenant.
        /// </summary>
        public long TenantUserId { get; set; }

        /// <summary>
        /// Status da solicitação.
        /// </summary>
        public TenantJoinRequestStatus Status { get; set; }

        /// <summary>
        /// Mensagem do solicitante.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Id do usuário que aprovou/rejeitou.
        /// </summary>
        public long? ApproverUserId { get; set; }
    }
}
