using Abp.Webhooks;
using System;
using System.Net;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe GetAllSendAttemptsOfWebhookEventOutput.
    /// </summary>
    public class GetAllSendAttemptsOfWebhookEventOutput
    {
        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// <see cref="WebhookSendAttempt"/> unique id
        /// </summary>
        public Guid Id { get; set; }

        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Webhook response content that webhook endpoint send back
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Webhook response status code that webhook endpoint send back
        /// </summary>
        public HttpStatusCode? ResponseStatusCode { get; set; }

        /// <summary>
        /// <see cref="WebhookSubscription"/> foreign id
        /// </summary>
        public Guid WebhookSubscriptionId { get; set; }

        /// <summary>
        /// <see cref="WebhookSubscriptionInfo.WebhookUri"/>
        /// </summary>
        public string WebhookUri { get; set; }
    }
}