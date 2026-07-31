using Abp.Notifications;

namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Dados da notificação em massa publicada pelo ABP.
    /// </summary>
    public class MassNotificationData : NotificationData
    {
        /// <summary>
        /// Assunto da notificação.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Mensagem da notificação.
        /// </summary>
        public string Message { get; set; }
    }
}
