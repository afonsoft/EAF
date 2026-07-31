using Abp.Application.Services.Dto;
using Abp.Notifications;

namespace Eaf.Middleware.MassNotifications.Dto
{
    /// <summary>
    /// DTO para representar uma notificação em massa.
    /// </summary>
    public class MassNotificationDto : EntityDto<long>
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Assunto.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Mensagem.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Severidade.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Identificadores de usuários alvo.
        /// </summary>
        public string TargetUserIds { get; set; }

        /// <summary>
        /// Identificadores de perfis alvo.
        /// </summary>
        public string TargetRoleIds { get; set; }

        /// <summary>
        /// Identificadores de unidades organizacionais alvo.
        /// </summary>
        public string TargetOrganizationUnitIds { get; set; }

        /// <summary>
        /// Indica se envia para todos os usuários do tenant.
        /// </summary>
        public bool SendToAllUsers { get; set; }

        /// <summary>
        /// Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Data/hora de agendamento.
        /// </summary>
        public System.DateTime? ScheduledTime { get; set; }
    }
}
