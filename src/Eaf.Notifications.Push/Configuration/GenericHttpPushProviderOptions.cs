namespace Eaf.Notifications.Push.Configuration
{
    /// <summary>
    /// Configuration for the generic HTTP push provider.
    /// </summary>
    public class GenericHttpPushProviderOptions
    {
        /// <summary>
        /// Base URL of the provider API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Relative endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Authentication type: None, Basic, Bearer or Header.
        /// </summary>
        public string AuthenticationType { get; set; } = "None";

        /// <summary>
        /// Username for Basic authentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for Basic authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Token for Bearer authentication.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Header name for API-key authentication.
        /// </summary>
        public string ApiKeyHeaderName { get; set; }

        /// <summary>
        /// API key value when <see cref="AuthenticationType"/> is Header.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Content type: Json or Form.
        /// </summary>
        public string ContentType { get; set; } = "Json";

        /// <summary>
        /// Body template with placeholders {{endpoint}}, {{p256dh}}, {{auth}}, {{title}}, {{body}}, {{icon}}, {{data}} and {{tag}}.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
