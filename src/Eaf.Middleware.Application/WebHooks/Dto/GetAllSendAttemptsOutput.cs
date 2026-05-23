using Abp.Webhooks;
using System;
using System.Net;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe GetAllSendAttemptsOutput.
    /// </summary>
    public class GetAllSendAttemptsOutput
    {
        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Webhook data as JSON string.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Obtém ou define Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Webhook response content that webhook endpoint send back
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Webhook response status code that webhook endpoint send back
        /// </summary>
        public HttpStatusCode? ResponseStatusCode { get; set; }

        /// <summary>
        /// <see cref="WebhookEvent"/> foreign id
        /// </summary>
        public Guid WebhookEventId { get; set; }

        /// <summary>
        /// Webhook unique name <see cref="WebhookDefinition.Name"/>
        /// </summary>
        public string WebhookName { get; set; }
    }
}