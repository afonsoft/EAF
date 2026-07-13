using Abp.Application.Services.Dto;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Webhooks;
using Eaf.Middleware.WebHooks;
using Eaf.Middleware.WebHooks.Dto;
using Eaf.WebHooks;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks
{
    /// <summary>
    /// Testes BDD para WebhookSubscriptionAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebhookSubscriptionAppServiceBddTests
    {
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IWebhookDefinitionManager _webhookDefinitionManager;
        private readonly WebhookSubscriptionAppService _sut;

        public WebhookSubscriptionAppServiceBddTests()
        {
            _webhookSubscriptionManager = Substitute.For<IWebhookSubscriptionManager>();
            _webhookDefinitionManager = Substitute.For<IWebhookDefinitionManager>();
            _sut = new WebhookSubscriptionAppService(_webhookSubscriptionManager, _webhookDefinitionManager);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<GetAllSubscriptionsOutput>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>() as IList<WebhookSubscription>;
                    if (source == null) return new List<GetAllSubscriptionsOutput>();
                    var result = new List<GetAllSubscriptionsOutput>();
                    foreach (var s in source)
                        result.Add(new GetAllSubscriptionsOutput { WebhookUri = s.WebhookUri });
                    return result;
                });
            objectMapper.Map<List<GetAllAvailableWebhooksOutput>>(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<object>() as IList<WebhookDefinition>;
                    if (source == null) return new List<GetAllAvailableWebhooksOutput>();
                    var result = new List<GetAllAvailableWebhooksOutput>();
                    foreach (var d in source)
                        result.Add(new GetAllAvailableWebhooksOutput { Name = d.Name });
                    return result;
                });
            _sut.ObjectMapper = objectMapper;
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAllSubscriptions

        [Fact]
        public async Task Dado_SubscricoesExistentes_Quando_GetAllSubscriptions_Entao_DeveRetornarLista()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var subscriptions = new List<WebhookSubscription>
            {
                new WebhookSubscription { WebhookUri = "https://example.com/hook1" },
                new WebhookSubscription { WebhookUri = "https://example.com/hook2" }
            };
            _webhookSubscriptionManager.GetAllSubscriptionsAsync(1).Returns(subscriptions);

            // Quando
            var result = await _sut.GetAllSubscriptions();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
        }

        #endregion

        #region GetSubscription

        [Fact]
        public async Task Dado_SubscricaoExistente_Quando_GetSubscription_Entao_DeveRetornarSubscricao()
        {
            // Dado
            var subscriptionId = Guid.NewGuid();
            var subscription = new WebhookSubscription
            {
                WebhookUri = "https://example.com/hook"
            };
            _webhookSubscriptionManager.GetAsync(subscriptionId).Returns(subscription);

            // Quando
            var result = await _sut.GetSubscription(subscriptionId.ToString());

            // Então
            result.ShouldNotBeNull();
            result.WebhookUri.ShouldBe("https://example.com/hook");
        }

        #endregion

        #region IsSubscribed

        [Fact]
        public async Task Dado_TenantSubscrito_Quando_IsSubscribed_Entao_DeveRetornarTrue()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            _webhookSubscriptionManager.IsSubscribedAsync(1, "WebhookTest").Returns(true);

            // Quando
            var result = await _sut.IsSubscribed("WebhookTest");

            // Então
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_TenantNaoSubscrito_Quando_IsSubscribed_Entao_DeveRetornarFalse()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            _webhookSubscriptionManager.IsSubscribedAsync(1, "WebhookTest").Returns(false);

            // Quando
            var result = await _sut.IsSubscribed("WebhookTest");

            // Então
            result.ShouldBeFalse();
        }

        #endregion

        #region AddSubscription

        [Fact]
        public async Task Dado_NovaSubscricao_Quando_AddSubscription_Entao_DeveChamarManager()
        {
            // Dado
            var subscription = new WebhookSubscription
            {
                WebhookUri = "https://example.com/hook"
            };

            // Quando
            await _sut.AddSubscription(subscription);

            // Então
            await _webhookSubscriptionManager.Received(1)
                .AddOrUpdateSubscriptionAsync(subscription);
        }

        #endregion

        #region UpdateSubscription

        [Fact]
        public async Task Dado_SubscricaoExistente_Quando_UpdateSubscription_Entao_DeveChamarManager()
        {
            // Dado
            var subscription = new WebhookSubscription
            {
                WebhookUri = "https://example.com/hook-updated"
            };

            // Quando
            await _sut.UpdateSubscription(subscription);

            // Então
            await _webhookSubscriptionManager.Received(1)
                .AddOrUpdateSubscriptionAsync(subscription);
        }

        #endregion

        #region ActivateWebhookSubscription

        [Fact]
        public async Task Dado_SubscricaoInativa_Quando_ActivateWebhookSubscription_Entao_DeveAtivar()
        {
            // Dado
            var subscriptionId = Guid.NewGuid();
            var subscription = new WebhookSubscription
            {
                IsActive = false
            };
            _webhookSubscriptionManager.GetAsync(subscriptionId).Returns(subscription);

            var input = new ActivateWebhookSubscriptionInput
            {
                SubscriptionId = subscriptionId,
                IsActive = true
            };

            // Quando
            await _sut.ActivateWebhookSubscription(input);

            // Então
            subscription.IsActive.ShouldBeTrue();
            await _webhookSubscriptionManager.Received(1).AddOrUpdateSubscriptionAsync(subscription);
        }

        [Fact]
        public async Task Dado_SubscricaoAtiva_Quando_DesativarWebhookSubscription_Entao_DeveDesativar()
        {
            // Dado
            var subscriptionId = Guid.NewGuid();
            var subscription = new WebhookSubscription
            {
                IsActive = true
            };
            _webhookSubscriptionManager.GetAsync(subscriptionId).Returns(subscription);

            var input = new ActivateWebhookSubscriptionInput
            {
                SubscriptionId = subscriptionId,
                IsActive = false
            };

            // Quando
            await _sut.ActivateWebhookSubscription(input);

            // Então
            subscription.IsActive.ShouldBeFalse();
        }

        #endregion

        #region GetAllAvailableWebhooks

        [Fact]
        public async Task Dado_WebhooksDefinidos_Quando_GetAllAvailableWebhooks_Entao_DeveRetornarDefinicoes()
        {
            // Dado
            var definitions = new List<WebhookDefinition>
            {
                new WebhookDefinition("Webhook1"),
                new WebhookDefinition("Webhook2")
            };
            _webhookDefinitionManager.GetAll().Returns(definitions);

            // Quando
            var result = await _sut.GetAllAvailableWebhooks();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
        }

        #endregion

        #region GetAllSubscriptionsIfFeaturesGranted

        [Fact]
        public async Task Dado_FeaturesHabilitadas_Quando_GetAllSubscriptionsIfFeaturesGranted_Entao_DeveRetornarSubscricoes()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var subscriptions = new List<WebhookSubscription>
            {
                new WebhookSubscription { WebhookUri = "https://example.com/hook1" }
            };
            _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(1, "WebhookTest")
                .Returns(subscriptions);

            // Quando
            var result = await _sut.GetAllSubscriptionsIfFeaturesGranted("WebhookTest");

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
        }

        #endregion
    }
}
