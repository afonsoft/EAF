namespace Eaf.Notifications.Sms.Configuration
{
    /// <summary>
    /// Top-level configuration section for EAF SMS notifications.
    /// Bind to <c>Eaf:Sms</c>.
    /// </summary>
    public class SmsOptions
    {
        /// <summary>
        /// Indicates whether SMS notifications are enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Name of the active provider. Matches <see cref="ISmsProvider.Name"/>.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Default sender identifier used when <see cref="SmsMessage.From"/> is not set.
        /// </summary>
        public string DefaultFrom { get; set; }

        /// <summary>
        /// Options for the generic HTTP provider.
        /// </summary>
        public GenericHttpSmsProviderOptions GenericHttp { get; set; } = new GenericHttpSmsProviderOptions();

        /// <summary>
        /// Options for the Twilio provider.
        /// </summary>
        public TwilioSmsProviderOptions Twilio { get; set; } = new TwilioSmsProviderOptions();
    }
}
