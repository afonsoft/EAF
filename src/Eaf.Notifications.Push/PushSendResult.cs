namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Result of a push notification send attempt.
    /// </summary>
    public class PushSendResult
    {
        /// <summary>
        /// True when the provider accepts the notification.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Provider-specific message or transaction identifier.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Human-readable error message when <see cref="Succeeded"/> is false.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
