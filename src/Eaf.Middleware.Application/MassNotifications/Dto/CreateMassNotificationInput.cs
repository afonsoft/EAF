using Abp.Notifications;
using System;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MassNotifications.Dto
{
    /// <summary>
    /// Entrada para criação de uma notificação em massa.
    /// </summary>
    public class CreateMassNotificationInput
    {
        /// <summary>
        /// Assunto.
        /// </summary>
        [Required]
        [StringLength(MassNotification.MaxSubjectLength)]
        public string Subject { get; set; }

        /// <summary>
        /// Mensagem.
        /// </summary>
        [Required]
        [StringLength(MassNotification.MaxMessageLength)]
        public string Message { get; set; }

        /// <summary>
        /// Severidade.
        /// </summary>
        public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

        /// <summary>
        /// Identificadores de usuários separados por vírgula.
        /// </summary>
        [StringLength(MassNotification.MaxTargetIdsLength)]
        public string TargetUserIds { get; set; }

        /// <summary>
        /// Identificadores de perfis separados por vírgula.
        /// </summary>
        [StringLength(MassNotification.MaxTargetIdsLength)]
        public string TargetRoleIds { get; set; }

        /// <summary>
        /// Identificadores de unidades organizacionais separados por vírgula.
        /// </summary>
        [StringLength(MassNotification.MaxTargetIdsLength)]
        public string TargetOrganizationUnitIds { get; set; }

        /// <summary>
        /// Enviar para todos os usuários do tenant.
        /// </summary>
        public bool SendToAllUsers { get; set; }

        /// <summary>
        /// Data/hora de agendamento (null para envio imediato).
        /// </summary>
        public DateTime? ScheduledTime { get; set; }
    }
}
