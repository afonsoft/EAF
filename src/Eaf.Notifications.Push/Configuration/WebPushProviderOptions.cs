namespace Eaf.Notifications.Push.Configuration
{
    /// <summary>
    /// Configuration for the Web Push (VAPID) provider.
    /// </summary>
    public class WebPushProviderOptions
    {
        /// <summary>
        /// VAPID public key (Base64URL).
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// VAPID private key (Base64URL).
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// VAPID subject, usually a mailto: or https:// URI.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
