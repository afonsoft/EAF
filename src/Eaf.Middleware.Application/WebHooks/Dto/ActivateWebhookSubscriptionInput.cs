using System;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe ActivateWebhookSubscriptionInput.
    /// </summary>
    public class ActivateWebhookSubscriptionInput
    {
        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Obtém ou define SubscriptionId.
        /// </summary>
        public Guid SubscriptionId { get; set; }
    }
}