using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.UI;
using Abp.Webhooks;
using Abp.Webhooks.Extensions;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Gerenciador de assinaturas de webhook EAF. Reutiliza o ABP e ajusta persistência de IsActive e segredo.
    /// </summary>
    public class EafWebhookSubscriptionManager : WebhookSubscriptionManager
    {
        private const string WebhookSubscriptionSecretPrefix = "whs_";

        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebhookSubscriptionSecretProtector _secretProtector;
        private readonly EafWebhooksOptions _options;

        public EafWebhookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebhookDefinitionManager webhookDefinitionManager,
            IWebhookSubscriptionSecretProtector secretProtector,
            IOptions<EafWebhooksOptions> optionsAccessor) : base(guidGenerator, webhookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _secretProtector = secretProtector;
            _options = optionsAccessor.Value;
        }

        public override async Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            Validate(webhookSubscription);
            await CheckDuplicateAsync(webhookSubscription);

            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await CheckIfPermissionsGrantedAsync(webhookSubscription);

                if (webhookSubscription.Id == default)
                    await CreateSubscriptionAsync(webhookSubscription);
                else
                    await UpdateSubscriptionAsync(webhookSubscription);
            });
        }

        public override void AddOrUpdateSubscription(WebhookSubscription webhookSubscription)
        {
            Validate(webhookSubscription);
            CheckDuplicate(webhookSubscription);

            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                CheckIfPermissionsGranted(webhookSubscription);

                if (webhookSubscription.Id == default)
                    CreateSubscription(webhookSubscription);
                else
                    UpdateSubscription(webhookSubscription);
            });
        }

        private async Task CreateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            webhookSubscription.Id = _guidGenerator.Create();

            if (string.IsNullOrWhiteSpace(webhookSubscription.Secret))
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");

            webhookSubscription.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            await WebhookSubscriptionsStore.InsertAsync(webhookSubscription.ToWebhookSubscriptionInfo());
        }

        private async Task UpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            var info = await WebhookSubscriptionsStore.GetAsync(webhookSubscription.Id);

            info.WebhookUri = webhookSubscription.WebhookUri;
            info.Webhooks = webhookSubscription.Webhooks.ToJsonString();
            info.Headers = webhookSubscription.Headers != null
                ? webhookSubscription.Headers.ToJsonString()
                : "{}";
            info.IsActive = webhookSubscription.IsActive;

            if (!string.IsNullOrWhiteSpace(webhookSubscription.Secret) && webhookSubscription.Secret != info.Secret)
                info.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            await WebhookSubscriptionsStore.UpdateAsync(info);
        }

        private void CreateSubscription(WebhookSubscription webhookSubscription)
        {
            webhookSubscription.Id = _guidGenerator.Create();

            if (string.IsNullOrWhiteSpace(webhookSubscription.Secret))
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString("N");

            webhookSubscription.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            WebhookSubscriptionsStore.Insert(webhookSubscription.ToWebhookSubscriptionInfo());
        }

        private void UpdateSubscription(WebhookSubscription webhookSubscription)
        {
            var info = WebhookSubscriptionsStore.Get(webhookSubscription.Id);

            info.WebhookUri = webhookSubscription.WebhookUri;
            info.Webhooks = webhookSubscription.Webhooks.ToJsonString();
            info.Headers = webhookSubscription.Headers != null
                ? webhookSubscription.Headers.ToJsonString()
                : "{}";
            info.IsActive = webhookSubscription.IsActive;

            if (!string.IsNullOrWhiteSpace(webhookSubscription.Secret) && webhookSubscription.Secret != info.Secret)
                info.Secret = _secretProtector.Protect(webhookSubscription.Secret);

            WebhookSubscriptionsStore.Update(info);
        }

        protected virtual void Validate(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription == null)
                throw new ArgumentNullException(nameof(webhookSubscription));
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
                throw new UserFriendlyException("Pelo menos um evento de webhook é obrigatório.");
            if (string.IsNullOrWhiteSpace(webhookSubscription.WebhookUri))
                throw new UserFriendlyException("A URI do webhook é obrigatória.");
            if (!Uri.TryCreate(webhookSubscription.WebhookUri, UriKind.Absolute, out var uri))
                throw new UserFriendlyException("A URI do webhook não é válida.");
            if (!_options.AllowHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new UserFriendlyException("A URI do webhook deve usar HTTPS.");
        }

        protected virtual async Task CheckDuplicateAsync(WebhookSubscription webhookSubscription)
        {
            var existing = await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(webhookSubscription.TenantId);
            ThrowIfDuplicate(webhookSubscription, existing);
        }

        protected virtual void CheckDuplicate(WebhookSubscription webhookSubscription)
        {
            var existing = WebhookSubscriptionsStore.GetAllSubscriptions(webhookSubscription.TenantId);
            ThrowIfDuplicate(webhookSubscription, existing);
        }

        private void ThrowIfDuplicate(WebhookSubscription webhookSubscription, List<WebhookSubscriptionInfo> existing)
        {
            var inputEvents = new HashSet<string>(webhookSubscription.Webhooks);

            foreach (var item in existing.Where(x => x.Id != webhookSubscription.Id))
            {
                if (item.WebhookUri != webhookSubscription.WebhookUri)
                    continue;

                var itemEvents = item.GetSubscribedWebhooks();
                if (inputEvents.Any(e => itemEvents.Contains(e)))
                    throw new UserFriendlyException("Já existe uma assinatura com a mesma URL e evento para este tenant.");
            }
        }
    }
}
