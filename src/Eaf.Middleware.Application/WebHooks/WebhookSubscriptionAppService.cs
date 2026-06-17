using Abp.Application.Services.Dto;
using Abp.Authorization;
using Eaf.Middleware;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.WebHooks;
using Eaf.Middleware.WebHooks.Dto;
using Abp.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.WebHooks
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de WebhookSubscription.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration)]
    public class WebhookSubscriptionAppService : MiddlewareAppServiceBase, IWebhookSubscriptionAppService
    {
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IWebhookDefinitionManager _webhookDefinitionManager;

        /// <summary>
        /// WebhookSubscriptionAppService.
        /// </summary>
        /// <param name="webhookSubscriptionManager">Parâmetro webhookSubscriptionManager.</param>
        /// <param name="webhookDefinitionManager">Parâmetro webhookDefinitionManager.</param>
        /// <returns>Resultado da operação.</returns>
        public WebhookSubscriptionAppService(IWebhookSubscriptionManager webhookSubscriptionManager, IWebhookDefinitionManager webhookDefinitionManager)
        {
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _webhookDefinitionManager = webhookDefinitionManager;
        }

        /// <summary>
        /// ActivateWebhookSubscription.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task ActivateWebhookSubscription(ActivateWebhookSubscriptionInput input)
        {
            var subscription = await _webhookSubscriptionManager.GetAsync(input.SubscriptionId);
            subscription.IsActive = input.IsActive;
            _webhookSubscriptionManager.AddOrUpdateSubscription(subscription);
        }

        /// <summary>
        /// AddSubscription.
        /// </summary>
        /// <param name="subscription">Parâmetro subscription.</param>
        public Task AddSubscription(WebhookSubscription subscription)
        {
            return _webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);
        }

        /// <summary>
        /// GetAllAvailableWebhooks.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<ListResultDto<GetAllAvailableWebhooksOutput>> GetAllAvailableWebhooks()
        {
            var itens = _webhookDefinitionManager.GetAll().ToList();
            var itensDto = new ListResultDto<GetAllAvailableWebhooksOutput>(ObjectMapper.Map<List<GetAllAvailableWebhooksOutput>>(itens));
            return await Task.FromResult(itensDto);
        }

        /// <summary>
        /// GetAllSubscriptions.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptions()
        {
            var itens = await _webhookSubscriptionManager.GetAllSubscriptionsAsync(AbpSession.TenantId);
            return new ListResultDto<GetAllSubscriptionsOutput>(ObjectMapper.Map<List<GetAllSubscriptionsOutput>>(itens));
        }

        /// <summary>
        /// GetAllSubscriptionsIfFeaturesGranted.
        /// </summary>
        /// <param name="webhookName">Parâmetro webhookName.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptionsIfFeaturesGranted(string webhookName)
        {
            var itens = await _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(AbpSession.TenantId, webhookName);
            return new ListResultDto<GetAllSubscriptionsOutput>(ObjectMapper.Map<List<GetAllSubscriptionsOutput>>(itens));
        }

        /// <summary>
        /// GetSubscription.
        /// </summary>
        /// <param name="subscriptionId">Parâmetro subscriptionId.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<WebhookSubscription> GetSubscription(string subscriptionId)
        {
            return _webhookSubscriptionManager.GetAsync(Guid.Parse(subscriptionId));
        }

        /// <summary>
        /// IsSubscribed.
        /// </summary>
        /// <param name="webhookName">Parâmetro webhookName.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<bool> IsSubscribed(string webhookName)
        {
            return _webhookSubscriptionManager.IsSubscribedAsync(AbpSession.TenantId, webhookName);
        }

        /// <summary>
        /// UpdateSubscription.
        /// </summary>
        /// <param name="subscription">Parâmetro subscription.</param>
        public Task UpdateSubscription(WebhookSubscription subscription)
        {
            return _webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);
        }
    }
}