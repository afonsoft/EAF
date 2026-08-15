using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Eaf.Notifications.Push.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Push.Providers
{
    /// <summary>
    /// Generic HTTP provider for push notification gateways such as Zenvia, Firebase or OneSignal.
    /// </summary>
    public class GenericHttpPushProvider : IPushNotificationProvider, ITransientDependency
    {
        /// <inheritdoc/>
        public string Name => "GenericHttp";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PushOptions _options;

        /// <summary>
        /// Creates a new <see cref="GenericHttpPushProvider"/>.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="optionsAccessor">Push options.</param>
        public GenericHttpPushProvider(IHttpClientFactory httpClientFactory, IOptions<PushOptions> optionsAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }

        /// <inheritdoc/>
        public async Task<PushSendResult> SendAsync(PushSubscription subscription, PushNotificationMessage message, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var opts = _options.GenericHttp ?? throw new UserFriendlyException("A seção Eaf:Push:GenericHttp não foi configurada.");

            if (string.IsNullOrWhiteSpace(opts.BaseUrl))
                throw new UserFriendlyException("Eaf:Push:GenericHttp:BaseUrl é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.Endpoint))
                throw new UserFriendlyException("Eaf:Push:GenericHttp:Endpoint é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.Template))
                throw new UserFriendlyException("Eaf:Push:GenericHttp:Template é obrigatório.");

            var client = _httpClientFactory.CreateClient("EafPush");
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds > 0 ? opts.TimeoutSeconds : 30);

            var content = opts.Template
                .Replace("{{endpoint}}", subscription.Endpoint ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{p256dh}}", subscription.P256dh ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{auth}}", subscription.Auth ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{title}}", message.Title ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{body}}", message.Body ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{icon}}", message.Icon ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{data}}", message.Data ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{tag}}", message.Tag ?? string.Empty, StringComparison.OrdinalIgnoreCase);

            var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(opts))
            {
                Content = BuildContent(opts, content)
            };

            ApplyAuthentication(request, opts);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new PushSendResult
                {
                    Succeeded = true,
                    MessageId = responseBody
                };
            }

            return new PushSendResult
            {
                Succeeded = false,
                ErrorMessage = $"HTTP {(int)response.StatusCode}: {responseBody}"
            };
        }

        private static Uri BuildUrl(GenericHttpPushProviderOptions opts)
        {
            var baseUrl = opts.BaseUrl.TrimEnd('/');
            var endpoint = opts.Endpoint.TrimStart('/');
            return new Uri($"{baseUrl}/{endpoint}", UriKind.Absolute);
        }

        private static HttpContent BuildContent(GenericHttpPushProviderOptions opts, string content)
        {
            if (string.Equals(opts.ContentType, "Form", StringComparison.OrdinalIgnoreCase))
                return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private static void ApplyAuthentication(HttpRequestMessage request, GenericHttpPushProviderOptions opts)
        {
            switch (opts.AuthenticationType?.ToLowerInvariant())
            {
                case "basic":
                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{opts.Username}:{opts.Password}"));
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                    break;

                case "bearer":
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", opts.Token);
                    break;

                case "header":
                    if (!string.IsNullOrWhiteSpace(opts.ApiKeyHeaderName) && !string.IsNullOrWhiteSpace(opts.ApiKey))
                        request.Headers.Add(opts.ApiKeyHeaderName, opts.ApiKey);
                    break;
            }
        }
    }
}
