using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class GetAllSendAttemptsInputTests
    {
        [Fact]
        public void Dado_GetAllSendAttemptsInput_Quando_Criado_Entao_SubscriptionIdDeveSerNulo()
        {
            var input = new GetAllSendAttemptsInput();
            input.SubscriptionId.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAllSendAttemptsInput_Quando_AtribuirSubscriptionId_Entao_DeveRetornarValor()
        {
            var input = new GetAllSendAttemptsInput { SubscriptionId = "sub-123" };
            input.SubscriptionId.ShouldBe("sub-123");
        }
    }
}
