namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Represents a push notification message.
    /// </summary>
    public class PushNotificationMessage
    {
        /// <summary>
        /// Notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Notification body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Icon URL.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Optional data payload serialized by the provider.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Optional tag used for grouping notifications.
        /// </summary>
        public string Tag { get; set; }
    }
}
