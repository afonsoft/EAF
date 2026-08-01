using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Representa uma solicitação de um usuário host para ingressar em um tenant.
    /// O shadow user é criado inativo e só é ativado após aprovação do administrador do tenant.
    /// </summary>
    [Table("AbpTenantJoinRequests")]
    public class TenantJoinRequest : CreationAuditedEntity<long>
    {
        /// <summary>
        /// Id do usuário host que solicitou ingresso.
        /// </summary>
        [Required]
        public virtual long UserId { get; set; }

        /// <summary>
        /// Id do tenant solicitado.
        /// </summary>
        [Required]
        public virtual int TenantId { get; set; }

        /// <summary>
        /// Id do shadow user criado dentro do tenant.
        /// </summary>
        [Required]
        public virtual long TenantUserId { get; set; }

        /// <summary>
        /// Status da solicitação.
        /// </summary>
        public virtual TenantJoinRequestStatus Status { get; set; }

        /// <summary>
        /// Mensagem opcional do solicitante.
        /// </summary>
        [StringLength(512)]
        public virtual string Message { get; set; }

        /// <summary>
        /// Id do usuário que aprovou/rejeitou a solicitação.
        /// </summary>
        public virtual long? ApproverUserId { get; set; }
    }
}
