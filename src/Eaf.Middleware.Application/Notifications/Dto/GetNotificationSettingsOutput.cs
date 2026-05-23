using System.Collections.Generic;

namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe GetNotificationSettingsOutput.
    /// </summary>
    public class GetNotificationSettingsOutput
    {
        public List<NotificationSubscriptionWithDisplayNameDto> Notifications { get; set; }
        /// <summary>
        /// Obtém ou define ReceiveNotifications.
        /// </summary>
        public bool ReceiveNotifications { get; set; }
    }
}