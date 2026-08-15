namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Result of an SMS send attempt.
    /// </summary>
    public class SmsSendResult
    {
        /// <summary>
        /// True when the provider confirms the message was accepted.
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
