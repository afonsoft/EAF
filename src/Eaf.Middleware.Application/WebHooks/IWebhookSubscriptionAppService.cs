using Abp.Application.Services.Dto;
using Eaf.Middleware.WebHooks.Dto;
using Abp.Webhooks;
using System.Threading.Tasks;

namespace Eaf.Middleware.WebHooks
{
    /// <summary>
    /// Representa a interface IWebhookSubscriptionAppService.
    /// </summary>
    public interface IWebhookSubscriptionAppService
    {
        Task ActivateWebhookSubscription(ActivateWebhookSubscriptionInput input);

        Task AddSubscription(WebhookSubscription subscription);

        Task<ListResultDto<GetAllAvailableWebhooksOutput>> GetAllAvailableWebhooks();

        /// <summary>
        /// Returns all subscriptions of tenant
        /// </summary>
        /// <returns></returns>
        Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptions();

        Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptionsIfFeaturesGranted(string webhookName);

        /// <summary>
        /// Returns subscription for given id.
        /// </summary>
        /// <param name="subscriptionId">Unique identifier of <see cref="WebhookSubscriptionInfo"/></param>
        Task<WebhookSubscription> GetSubscription(string subscriptionId);

        Task<bool> IsSubscribed(string webhookName);

        Task UpdateSubscription(WebhookSubscription subscription);
    }
}