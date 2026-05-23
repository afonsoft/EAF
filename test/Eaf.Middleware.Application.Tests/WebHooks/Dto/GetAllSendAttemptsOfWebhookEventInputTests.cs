using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class GetAllSendAttemptsOfWebhookEventInputTests
    {
        [Fact]
        public void Dado_GetAllSendAttemptsOfWebhookEventInput_Quando_Criado_Entao_IdDeveSerNulo()
        {
            var input = new GetAllSendAttemptsOfWebhookEventInput();
            input.Id.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAllSendAttemptsOfWebhookEventInput_Quando_AtribuirId_Entao_DeveRetornarValor()
        {
            var input = new GetAllSendAttemptsOfWebhookEventInput { Id = "event-456" };
            input.Id.ShouldBe("event-456");
        }
    }
}
