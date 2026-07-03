using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllSendAttemptsOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllSendAttemptsOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirData_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOutput();
            sut.Data = "test_value";
            sut.Data.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOutput();
            var guid = System.Guid.NewGuid(); sut.Id = guid;
            sut.Id.ShouldBe(guid);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirResponse_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOutput();
            sut.Response = "test_value";
            sut.Response.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWebhookEventId_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOutput();
            var guid = System.Guid.NewGuid(); sut.WebhookEventId = guid;
            sut.WebhookEventId.ShouldBe(guid);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWebhookName_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsOutput();
            sut.WebhookName = "test_value";
            sut.WebhookName.ShouldBe("test_value");
        }
    }
}
