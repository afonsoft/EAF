using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Webhooks;
using Abp.Webhooks.BackgroundWorker;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.WebHooks.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.WebHooks
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de WebhookSendAttempt.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration)]
    public class WebhookSendAttemptAppService : MiddlewareAppServiceBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<WebhookSubscriptionInfo, Guid> _subscriptionRepository;
        private readonly IWebhookEventAppService _webhookEventAppService;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;

        /// <summary>
        /// WebhookSendAttemptAppService.
        /// </summary>
        /// <param name="webhookSendAttemptStore">Parâmetro webhookSendAttemptStore.</param>
        /// <param name="backgroundJobManager">Parâmetro backgroundJobManager.</param>
        /// <param name="webhookEventAppService">Parâmetro webhookEventAppService.</param>
        /// <param name="webhookSubscriptionManager">Parâmetro webhookSubscriptionManager.</param>
        /// <param name="subscriptionRepository">Parâmetro subscriptionRepository.</param>
        /// <returns>Resultado da operação.</returns>
        public WebhookSendAttemptAppService(
             IWebhookSendAttemptStore webhookSendAttemptStore,
             IBackgroundJobManager backgroundJobManager,
             IWebhookEventAppService webhookEventAppService,
             IWebhookSubscriptionManager webhookSubscriptionManager,
             IRepository<WebhookSubscriptionInfo, Guid> subscriptionRepository
             )
        {
            _webhookSendAttemptStore = webhookSendAttemptStore;
            _backgroundJobManager = backgroundJobManager;
            _webhookEventAppService = webhookEventAppService;
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _subscriptionRepository = subscriptionRepository;
        }

        /// <summary>
        /// GetAllSendAttempts.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<PagedResultDto<GetAllSendAttemptsOutput>> GetAllSendAttempts(GetAllSendAttemptsInput input)
        {
            if (string.IsNullOrEmpty(input.SubscriptionId))
            {
                throw new ArgumentNullException(input.SubscriptionId);
            }

            return GetAllSendAttemptsInternal(input);
        }

        private async Task<PagedResultDto<GetAllSendAttemptsOutput>> GetAllSendAttemptsInternal(GetAllSendAttemptsInput input)
        {
            var list = await _webhookSendAttemptStore.GetAllSendAttemptsBySubscriptionAsPagedListAsync(
                AbpSession.TenantId,
                Guid.Parse(input.SubscriptionId),
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<GetAllSendAttemptsOutput>(list.TotalCount, ObjectMapper.Map<List<GetAllSendAttemptsOutput>>(list.Items));
        }

        /// <summary>
        /// GetAllSendAttemptsOfWebhookEvent.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<ListResultDto<GetAllSendAttemptsOfWebhookEventOutput>> GetAllSendAttemptsOfWebhookEvent(GetAllSendAttemptsOfWebhookEventInput input)
        {
            if (string.IsNullOrEmpty(input.Id))
            {
                throw new ArgumentNullException(input.Id);
            }

            return GetAllSendAttemptsOfWebhookEventInternal(input);
        }

        private async Task<ListResultDto<GetAllSendAttemptsOfWebhookEventOutput>> GetAllSendAttemptsOfWebhookEventInternal(GetAllSendAttemptsOfWebhookEventInput input)
        {
            var list = await _webhookSendAttemptStore.GetAllSendAttemptsByWebhookEventIdAsync(
                AbpSession.TenantId,
                Guid.Parse(input.Id)
            );

            var mappedList = ObjectMapper.Map<List<GetAllSendAttemptsOfWebhookEventOutput>>(list);
            var subscriptionIds = list.Select(x => x.WebhookSubscriptionId).Distinct().ToArray();

            var subscriptionUrisDictionary = await (await _subscriptionRepository.GetAllAsync()).Where(subscription => subscriptionIds.Contains(subscription.Id))
                 .Select(subscription => new { subscription.Id, subscription.WebhookUri })
                 .ToDictionaryAsync(s => s.Id, s => s.WebhookUri);

            foreach (var output in mappedList)
            {
                output.WebhookUri = subscriptionUrisDictionary[output.WebhookSubscriptionId];
            }

            return new ListResultDto<GetAllSendAttemptsOfWebhookEventOutput>(mappedList);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration)]
        public async Task Resend(string sendAttemptId)
        {
            var webhookSendAttempt = await _webhookSendAttemptStore.GetAsync(AbpSession.TenantId, Guid.Parse(sendAttemptId));
            var webhookEvent = await _webhookEventAppService.Get(webhookSendAttempt.WebhookEventId.ToString());
            var webhookSubscription = await _webhookSubscriptionManager.GetAsync(webhookSendAttempt.WebhookSubscriptionId);

            await _backgroundJobManager.EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(new WebhookSenderArgs()
            {
                TenantId = AbpSession.TenantId,
                WebhookEventId = webhookSendAttempt.WebhookEventId,
                WebhookSubscriptionId = webhookSendAttempt.WebhookSubscriptionId,
                Data = webhookEvent.Data,
                WebhookName = webhookEvent.WebhookName,
                WebhookUri = webhookSubscription.WebhookUri,
                Headers = webhookSubscription.Headers,
                Secret = webhookSubscription.Secret,
                TryOnce = true
            });
        }
    }
}