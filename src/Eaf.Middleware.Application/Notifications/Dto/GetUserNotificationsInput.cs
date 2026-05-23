using Eaf.Middleware.Dto;
using Abp.Notifications;

namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe GetUserNotificationsInput.
    /// </summary>
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}