using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Gerenciador de webhooks EAF. Reutiliza <see cref="WebhookManager"/> do ABP e aplica HMAC e payload no formato EAF.
    /// </summary>
    public class EafWebhookManager : WebhookManager
    {
        private readonly IWebhookSubscriptionSecretProtector _secretProtector;
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly EafWebhooksOptions _options;

        public EafWebhookManager(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSendAttemptStore webhookSendAttemptStore,
            IWebhookSubscriptionSecretProtector secretProtector,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(webhooksConfiguration, webhookSendAttemptStore)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _secretProtector = secretProtector;
            _options = optionsAccessor.Value;
        }

        public override string GetSerializedBody(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));

            if (webhookSenderArgs.SendExactSameData)
                return webhookSenderArgs.Data;

            var payload = base.GetWebhookPayload(webhookSenderArgs);
            return SerializeEafPayload(payload);
        }

        public override async Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));

            if (webhookSenderArgs.SendExactSameData)
                return webhookSenderArgs.Data;

            var payload = await base.GetWebhookPayloadAsync(webhookSenderArgs);
            return SerializeEafPayload(payload);
        }

        public override void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(serializedBody))
                throw new ArgumentNullException(nameof(serializedBody));

            var plainSecret = _secretProtector.Unprotect(secret);

            if (string.IsNullOrWhiteSpace(plainSecret))
                throw new ArgumentException("O segredo do webhook está ausente ou não pôde ser descriptografado.", nameof(secret));

            var secretBytes = Encoding.UTF8.GetBytes(plainSecret);

            using (var hasher = new HMACSHA256(secretBytes))
            {
                request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

                var data = Encoding.UTF8.GetBytes(serializedBody);
                var hash = hasher.ComputeHash(data);
                var headerValue = string.Format(
                    CultureInfo.InvariantCulture,
                    _options.SignatureValueTemplate,
                    Convert.ToHexString(hash).ToLowerInvariant());

                request.Headers.Add(_options.SignatureHeaderName, headerValue);
            }
        }

        private string SerializeEafPayload(WebhookPayload payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var eafPayload = new
            {
                eventName = payload.WebhookEvent,
                timestamp = payload.CreationTimeUtc,
                payload = payload.Data
            };

            return _webhooksConfiguration.JsonSerializerOptions != null
                ? eafPayload.ToJsonString(_webhooksConfiguration.JsonSerializerOptions)
                : eafPayload.ToJsonString();
        }
    }
}
