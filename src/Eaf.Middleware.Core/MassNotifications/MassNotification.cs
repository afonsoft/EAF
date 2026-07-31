using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Notifications;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Representa uma notificação em massa enviada para usuários, perfis e/ou unidades organizacionais.
    /// </summary>
    [Table("EafMassNotifications")]
    public class MassNotification : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxTargetIdsLength = 4000;
        public const int MaxSubjectLength = 256;
        public const int MaxMessageLength = 4000;

        /// <summary>
        /// Identificador do tenant (null para host).
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Assunto da notificação.
        /// </summary>
        [Required]
        [StringLength(MaxSubjectLength)]
        public string Subject { get; set; }

        /// <summary>
        /// Mensagem da notificação.
        /// </summary>
        [Required]
        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        /// <summary>
        /// Severidade da notificação.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Lista de identificadores de usuários separados por vírgula.
        /// </summary>
        [StringLength(MaxTargetIdsLength)]
        public string TargetUserIds { get; set; }

        /// <summary>
        /// Lista de identificadores de perfis separados por vírgula.
        /// </summary>
        [StringLength(MaxTargetIdsLength)]
        public string TargetRoleIds { get; set; }

        /// <summary>
        /// Lista de identificadores de unidades organizacionais separados por vírgula.
        /// </summary>
        [StringLength(MaxTargetIdsLength)]
        public string TargetOrganizationUnitIds { get; set; }

        /// <summary>
        /// Indica se a notificação deve ser enviada a todos os usuários do tenant.
        /// </summary>
        public bool SendToAllUsers { get; set; }

        /// <summary>
        /// Status da notificação.
        /// </summary>
        public MassNotificationStatus Status { get; set; }

        /// <summary>
        /// Data/hora de agendamento (null para envio imediato).
        /// </summary>
        public System.DateTime? ScheduledTime { get; set; }
    }
}
