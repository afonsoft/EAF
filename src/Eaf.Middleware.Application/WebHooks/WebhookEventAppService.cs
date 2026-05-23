using Abp.Authorization;
using Eaf.Middleware.Authorization;
using Abp.Webhooks;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.WebHooks
{
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration)]
    public class WebhookEventAppService : MiddlewareAppServiceBase, IWebhookEventAppService
    {
        private readonly IWebhookEventStore _webhookEventStore;

        /// <summary>
        /// WebhookEventAppService.
        /// </summary>
        /// <param name="webhookEventStore">Parâmetro webhookEventStore.</param>
        /// <returns>Resultado da operação.</returns>
        public WebhookEventAppService(IWebhookEventStore webhookEventStore)
        {
            _webhookEventStore = webhookEventStore;
        }

        /// <summary>
        /// Get.
        /// </summary>
        /// <param name="id">Parâmetro id.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<WebhookEvent> Get(string id)
        {
            return await _webhookEventStore.GetAsync(AbpSession.TenantId, Guid.Parse(id));
        }
    }
}