using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.AspNetCore.Webhook;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Webhooks.Configuration;
using Eaf.Webhooks.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Testes BDD para EafWebhookSender seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EafWebhookSenderTests
    {
        private static (EafWebhookSender Sender, FakeHttpMessageHandler Handler) CriarSender(EafWebhooksOptions options = null)
        {
            var handler = new FakeHttpMessageHandler();
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName).Returns(new HttpClient(handler));

            var configuration = Substitute.For<IWebhooksConfiguration>();
            configuration.TimeoutDuration.Returns(TimeSpan.FromSeconds(30));

            var webhookSendAttemptStore = Substitute.For<IWebhookSendAttemptStore>();
            webhookSendAttemptStore.When(x => x.InsertAsync(Arg.Any<WebhookSendAttempt>())).Do(callInfo =>
            {
                callInfo.Arg<WebhookSendAttempt>().Id = Guid.NewGuid();
            });

            var protector = Substitute.For<IWebhookSubscriptionSecretProtector>();
            protector.Protect(Arg.Any<string>()).Returns(ci => ci.Arg<string>());
            protector.Unprotect(Arg.Any<string>()).Returns(ci => ci.Arg<string>());

            var webhookManager = new EafWebhookManager(
                configuration,
                webhookSendAttemptStore,
                protector,
                Options.Create(options ?? new EafWebhooksOptions()));

            var sender = new EafWebhookSender(
                configuration,
                webhookManager,
                factory,
                Options.Create(options ?? new EafWebhooksOptions()));

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            webhookManager.UnitOfWorkManager = unitOfWorkManager;
            sender.UnitOfWorkManager = unitOfWorkManager;

            return (sender, handler);
        }

        [Fact]
        public async Task Dado_UriHttps_Quando_Enviar_Entao_DeveAdicionarAssinaturaEafNoCabecalho()
        {
            // Dado
            var (sender, handler) = CriarSender();
            var args = new WebhookSenderArgs
            {
                WebhookName = "NewUserRegistered",
                WebhookUri = "https://example.com/webhook",
                Secret = "segredo",
                Data = "{\"foo\":1}",
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                SendExactSameData = true,
                Headers = new System.Collections.Generic.Dictionary<string, string>()
            };

            // Quando
            await sender.SendWebhookAsync(args);

            // Então
            handler.LastRequest.ShouldNotBeNull();
            handler.LastRequest.Headers.ShouldContain(h => h.Key == "X-Eaf-Signature-256");
        }

        [Fact]
        public async Task Dado_UriHttpSemPermissao_Quando_Enviar_Entao_DeveLancarExcecao()
        {
            // Dado
            var (sender, _) = CriarSender();
            var args = new WebhookSenderArgs
            {
                WebhookName = "NewUserRegistered",
                WebhookUri = "http://example.com/webhook",
                Secret = "segredo",
                Data = "{}",
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                SendExactSameData = true,
                Headers = new System.Collections.Generic.Dictionary<string, string>()
            };

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sender.SendWebhookAsync(args));
        }

        [Fact]
        public async Task Dado_UriHttpComPermissao_Quando_Enviar_Entao_DevePermitirRequisicao()
        {
            // Dado
            var (sender, handler) = CriarSender(new EafWebhooksOptions { AllowHttp = true });
            var args = new WebhookSenderArgs
            {
                WebhookName = "NewUserRegistered",
                WebhookUri = "http://example.com/webhook",
                Secret = "segredo",
                Data = "{\"foo\":1}",
                WebhookEventId = Guid.NewGuid(),
                WebhookSubscriptionId = Guid.NewGuid(),
                SendExactSameData = true,
                Headers = new System.Collections.Generic.Dictionary<string, string>()
            };

            // Quando
            await sender.SendWebhookAsync(args);

            // Então
            handler.LastRequest.ShouldNotBeNull();
        }
    }
}
