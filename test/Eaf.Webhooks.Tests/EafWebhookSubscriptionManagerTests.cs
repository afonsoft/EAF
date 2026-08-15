using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Testes BDD para EafWebhookSubscriptionManager seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EafWebhookSubscriptionManagerTests
    {
        private static EafWebhookSubscriptionManager CriarGerenciador(EafWebhooksOptions options = null)
        {
            var guidGenerator = Substitute.For<IGuidGenerator>();
            guidGenerator.Create().Returns(Guid.NewGuid());

            var definitionManager = Substitute.For<IWebhookDefinitionManager>();
            definitionManager.IsAvailableAsync(Arg.Any<int?>(), Arg.Any<string>()).Returns(Task.FromResult(true));
            definitionManager.IsAvailable(Arg.Any<int?>(), Arg.Any<string>()).Returns(true);

            var protector = Substitute.For<IWebhookSubscriptionSecretProtector>();
            protector.Protect(Arg.Any<string>()).Returns(ci => "P:" + ci.Arg<string>());
            protector.Unprotect(Arg.Any<string>()).Returns(ci => ci.Arg<string>()?.TrimStart('P', ':'));

            var store = Substitute.For<IWebhookSubscriptionsStore>();
            store.GetAllSubscriptionsAsync(Arg.Any<int?>()).Returns(new List<WebhookSubscriptionInfo>());
            store.GetAllSubscriptions(Arg.Any<int?>()).Returns(new List<WebhookSubscriptionInfo>());

            var manager = new EafWebhookSubscriptionManager(
                guidGenerator,
                definitionManager,
                protector,
                Options.Create(options ?? new EafWebhooksOptions()));

            manager.WebhookSubscriptionsStore = store;
            manager.UnitOfWorkManager = CriarUnitOfWorkManager();

            return manager;
        }

        private static IUnitOfWorkManager CriarUnitOfWorkManager()
        {
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>()).Returns(Substitute.For<IUnitOfWorkCompleteHandle>());
            return unitOfWorkManager;
        }

        [Fact]
        public void Dado_ConstrutorComDependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            var sut = CriarGerenciador();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_NovaAssinaturaHttps_Quando_Adicionar_Entao_DeveGerarESalvarSegredoCifrado()
        {
            // Dado
            var sut = CriarGerenciador();
            var assinatura = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "NewUserRegistered" },
                Headers = new Dictionary<string, string> { { "X-Custom", "value" } }
            };

            // Quando
            await sut.AddOrUpdateSubscriptionAsync(assinatura);

            // Então
            await sut.WebhookSubscriptionsStore.Received(1).InsertAsync(Arg.Is<WebhookSubscriptionInfo>(info =>
                info.WebhookUri == "https://example.com/webhook" &&
                info.GetSubscribedWebhooks().Contains("NewUserRegistered") &&
                !string.IsNullOrEmpty(info.Secret) &&
                info.Secret.StartsWith("P:whs_")));

            assinatura.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task Dado_AssinaturaComUriHttp_Quando_Adicionar_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = CriarGerenciador();
            var assinatura = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "http://example.com/webhook",
                Webhooks = new List<string> { "NewUserRegistered" }
            };

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.AddOrUpdateSubscriptionAsync(assinatura));
        }

        [Fact]
        public async Task Dado_AssinaturaSemEvento_Quando_Adicionar_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = CriarGerenciador();
            var assinatura = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook"
            };

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.AddOrUpdateSubscriptionAsync(assinatura));
        }

        [Fact]
        public async Task Dado_AssinaturaDuplicadaMesmaUrlEEvento_Quando_Adicionar_Entao_DeveLancarExcecao()
        {
            // Dado
            var idExistente = Guid.NewGuid();
            var existente = new WebhookSubscriptionInfo
            {
                Id = idExistente,
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = "[\"NewUserRegistered\"]",
                IsActive = true
            };

            var sut = CriarGerenciador();
            sut.WebhookSubscriptionsStore.GetAllSubscriptionsAsync(1).Returns(new List<WebhookSubscriptionInfo> { existente });

            var assinatura = new WebhookSubscription
            {
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "NewUserRegistered" }
            };

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.AddOrUpdateSubscriptionAsync(assinatura));
        }

        [Fact]
        public async Task Dado_AssinaturaExistente_Quando_AtualizarSemNovoSegredo_Entao_DeveManterSegredoAntigo()
        {
            // Dado
            var id = Guid.NewGuid();
            var existente = new WebhookSubscriptionInfo
            {
                Id = id,
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = "[\"NewUserRegistered\"]",
                Secret = "P:antigo-segredo",
                IsActive = true
            };

            var sut = CriarGerenciador();
            sut.WebhookSubscriptionsStore.GetAsync(id).Returns(existente);

            var assinatura = new WebhookSubscription
            {
                Id = id,
                TenantId = 1,
                WebhookUri = "https://example.com/webhook-v2",
                Webhooks = new List<string> { "NewUserRegistered" },
                IsActive = false,
                Secret = null
            };

            // Quando
            await sut.AddOrUpdateSubscriptionAsync(assinatura);

            // Então
            await sut.WebhookSubscriptionsStore.Received(1).UpdateAsync(Arg.Is<WebhookSubscriptionInfo>(info =>
                info.WebhookUri == "https://example.com/webhook-v2" &&
                info.IsActive == false &&
                info.Secret == "P:antigo-segredo"));
        }

        [Fact]
        public async Task Dado_AssinaturaExistente_Quando_AtualizarComNovoSegredo_Entao_DeveCifrarENovoSegredo()
        {
            // Dado
            var id = Guid.NewGuid();
            var existente = new WebhookSubscriptionInfo
            {
                Id = id,
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = "[\"NewUserRegistered\"]",
                Secret = "P:antigo-segredo",
                IsActive = true
            };

            var sut = CriarGerenciador();
            sut.WebhookSubscriptionsStore.GetAsync(id).Returns(existente);

            var assinatura = new WebhookSubscription
            {
                Id = id,
                TenantId = 1,
                WebhookUri = "https://example.com/webhook",
                Webhooks = new List<string> { "NewUserRegistered" },
                Secret = "novo-segredo"
            };

            // Quando
            await sut.AddOrUpdateSubscriptionAsync(assinatura);

            // Então
            await sut.WebhookSubscriptionsStore.Received(1).UpdateAsync(Arg.Is<WebhookSubscriptionInfo>(info =>
                info.Secret == "P:novo-segredo"));
        }
    }
}
