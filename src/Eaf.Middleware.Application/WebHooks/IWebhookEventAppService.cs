using Abp.Webhooks;
using System.Threading.Tasks;

namespace Eaf.Middleware.WebHooks
{
    /// <summary>
    /// Representa a interface IWebhookEventAppService.
    /// </summary>
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}