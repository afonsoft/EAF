using Abp.AutoMapper;
using Abp.Webhooks;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe GetAllAvailableWebhooksOutput.
    /// </summary>
    [AutoMap(typeof(WebhookDefinition))]
    public class GetAllAvailableWebhooksOutput
    {
        /// <summary>
        /// Description for the webhook. Optional.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Display name of the webhook. Optional.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Unique name of the webhook.
        /// </summary>
        public string Name { get; set; }
    }
}