using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllSendAttemptsOfWebhookEventOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            var guid = System.Guid.NewGuid(); sut.Id = guid;
            sut.Id.ShouldBe(guid);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirLastModificationTime_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            var dt = System.DateTime.UtcNow; sut.LastModificationTime = dt;
            sut.LastModificationTime.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirResponse_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            sut.Response = "test_value";
            sut.Response.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWebhookSubscriptionId_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            var guid = System.Guid.NewGuid(); sut.WebhookSubscriptionId = guid;
            sut.WebhookSubscriptionId.ShouldBe(guid);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWebhookUri_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventOutput();
            sut.WebhookUri = "test_value";
            sut.WebhookUri.ShouldBe("test_value");
        }
    }
}
