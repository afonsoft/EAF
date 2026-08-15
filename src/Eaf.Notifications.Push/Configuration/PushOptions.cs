namespace Eaf.Notifications.Push.Configuration
{
    /// <summary>
    /// Top-level configuration section for EAF push notifications.
    /// Bind to <c>Eaf:Push</c>.
    /// </summary>
    public class PushOptions
    {
        /// <summary>
        /// Indicates whether push notifications are enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Name of the active provider. Matches <see cref="IPushNotificationProvider.Name"/>.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Options for the Web Push provider.
        /// </summary>
        public WebPushProviderOptions WebPush { get; set; } = new WebPushProviderOptions();

        /// <summary>
        /// Options for the generic HTTP provider.
        /// </summary>
        public GenericHttpPushProviderOptions GenericHttp { get; set; } = new GenericHttpPushProviderOptions();
    }
}
