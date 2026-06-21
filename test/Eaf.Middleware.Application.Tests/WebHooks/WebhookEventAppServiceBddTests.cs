using Abp.Runtime.Session;
using Abp.Webhooks;
using Eaf.Middleware.WebHooks;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks
{
    /// <summary>
    /// Testes BDD para WebhookEventAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class WebhookEventAppServiceBddTests
    {
        private readonly IWebhookEventStore _webhookEventStore;
        private readonly WebhookEventAppService _sut;

        public WebhookEventAppServiceBddTests()
        {
            _webhookEventStore = Substitute.For<IWebhookEventStore>();
            _sut = new WebhookEventAppService(_webhookEventStore);
        }

        #region Construtor

        [Fact]
        public void Dado_WebhookEventStore_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region Get

        [Fact]
        public async Task Dado_WebhookEventExistente_Quando_Get_Entao_DeveRetornarEvento()
        {
            // Dado
            var eventId = Guid.NewGuid();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var webhookEvent = new WebhookEvent
            {
                Id = eventId,
                WebhookName = "TestEvent",
                Data = "{}"
            };
            _webhookEventStore.GetAsync(1, eventId).Returns(webhookEvent);

            // Quando
            var result = await _sut.Get(eventId.ToString());

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(eventId);
            result.WebhookName.ShouldBe("TestEvent");
        }

        [Fact]
        public async Task Dado_WebhookEventSemTenant_Quando_Get_Entao_DeveUsarTenantIdNulo()
        {
            // Dado
            var eventId = Guid.NewGuid();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            _webhookEventStore.GetAsync(null, eventId).Returns(new WebhookEvent { Id = eventId });

            // Quando
            var result = await _sut.Get(eventId.ToString());

            // Então
            result.ShouldNotBeNull();
            await _webhookEventStore.Received(1).GetAsync(null, eventId);
        }

        #endregion
    }
}
