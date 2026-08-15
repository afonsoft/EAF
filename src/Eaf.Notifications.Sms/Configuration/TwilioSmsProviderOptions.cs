namespace Eaf.Notifications.Sms.Configuration
{
    /// <summary>
    /// Configuration for the Twilio SMS provider.
    /// </summary>
    public class TwilioSmsProviderOptions
    {
        /// <summary>
        /// Twilio account SID.
        /// </summary>
        public string AccountSid { get; set; }

        /// <summary>
        /// Twilio auth token.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Twilio phone number used as sender.
        /// </summary>
        public string From { get; set; }
    }
}
