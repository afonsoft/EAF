using System.Collections.Generic;

namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe UpdateNotificationSettingsInput.
    /// </summary>
    public class UpdateNotificationSettingsInput
    {
        public List<NotificationSubscriptionDto> Notifications { get; set; }
        /// <summary>
        /// Obtém ou define ReceiveNotifications.
        /// </summary>
        public bool ReceiveNotifications { get; set; }
    }
}