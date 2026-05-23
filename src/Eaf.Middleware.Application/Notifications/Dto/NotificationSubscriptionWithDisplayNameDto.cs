namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe NotificationSubscriptionWithDisplayNameDto.
    /// </summary>
    public class NotificationSubscriptionWithDisplayNameDto : NotificationSubscriptionDto
    {
        /// <summary>
        /// Obtém ou define Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Obtém ou define DisplayName.
        /// </summary>
        public string DisplayName { get; set; }
    }
}