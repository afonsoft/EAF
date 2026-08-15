using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Eaf.Notifications.Push.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Push.Providers
{
    /// <summary>
    /// Web Push provider using VAPID keys and the WebPush library.
    /// </summary>
    public class WebPushNotificationProvider : IPushNotificationProvider, ITransientDependency
    {
        /// <inheritdoc/>
        public string Name => "WebPush";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PushOptions _options;

        /// <summary>
        /// Creates a new <see cref="WebPushNotificationProvider"/>.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="optionsAccessor">Push options.</param>
        public WebPushNotificationProvider(IHttpClientFactory httpClientFactory, IOptions<PushOptions> optionsAccessor)
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

            var opts = _options.WebPush ?? throw new UserFriendlyException("A seção Eaf:Push:WebPush não foi configurada.");

            if (string.IsNullOrWhiteSpace(opts.PublicKey))
                throw new UserFriendlyException("Eaf:Push:WebPush:PublicKey é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.PrivateKey))
                throw new UserFriendlyException("Eaf:Push:WebPush:PrivateKey é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.Subject))
                throw new UserFriendlyException("Eaf:Push:WebPush:Subject é obrigatório.");

            if (string.IsNullOrWhiteSpace(subscription.P256dh))
                throw new UserFriendlyException("O campo P256dh da inscrição push é obrigatório.");
            if (string.IsNullOrWhiteSpace(subscription.Auth))
                throw new UserFriendlyException("O campo Auth da inscrição push é obrigatório.");

            var payload = JsonSerializer.Serialize(message);
            var webPushSubscription = new WebPush.PushSubscription(subscription.Endpoint, subscription.P256dh, subscription.Auth);
            var vapidDetails = new WebPush.VapidDetails(opts.Subject, opts.PublicKey, opts.PrivateKey);

            using var httpClient = _httpClientFactory.CreateClient("EafPush");
            httpClient.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds > 0 ? opts.TimeoutSeconds : 30);

            using var client = new WebPush.WebPushClient(httpClient);
            try
            {
                await client.SendNotificationAsync(webPushSubscription, payload, vapidDetails, cancellationToken);
                return new PushSendResult { Succeeded = true };
            }
            catch (WebPush.WebPushException exception)
            {
                return new PushSendResult
                {
                    Succeeded = false,
                    ErrorMessage = $"HTTP {(int)exception.StatusCode}: {exception.Message}"
                };
            }
        }
    }
}
