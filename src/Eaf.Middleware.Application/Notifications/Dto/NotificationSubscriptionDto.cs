using Abp.Notifications;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe NotificationSubscriptionDto.
    /// </summary>
    public class NotificationSubscriptionDto
    {
        /// <summary>
        /// Obtém ou define IsSubscribed.
        /// </summary>
        public bool IsSubscribed { get; set; }

        [Required]
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public string Name { get; set; }
    }
}