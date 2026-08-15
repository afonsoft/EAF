using System;
using System.Net.Http;
using Abp.AspNetCore.Webhook;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Sender de webhooks EAF baseado em <see cref="AspNetCoreWebhookSender"/> com guarda HTTPS.
    /// </summary>
    public class EafWebhookSender : AspNetCoreWebhookSender
    {
        private readonly EafWebhooksOptions _options;

        public EafWebhookSender(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookManager webhookManager,
            IHttpClientFactory clientFactory,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(webhooksConfiguration, webhookManager, clientFactory)
        {
            _options = optionsAccessor.Value;
        }

        protected override HttpRequestMessage CreateWebhookRequestMessage(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs == null)
                throw new ArgumentNullException(nameof(webhookSenderArgs));
            if (string.IsNullOrWhiteSpace(webhookSenderArgs.WebhookUri))
                throw new ArgumentException("A URI do webhook é obrigatória.", nameof(webhookSenderArgs));

            if (!_options.AllowHttp && (!Uri.TryCreate(webhookSenderArgs.WebhookUri, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
                throw new UserFriendlyException("A URI do webhook deve usar HTTPS.");

            return base.CreateWebhookRequestMessage(webhookSenderArgs);
        }
    }
}
