using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllSendAttemptsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllSendAttemptsInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSubscriptionId_Entao_DeveArmazenar()
        {
            var sut = new GetAllSendAttemptsInput();
            sut.SubscriptionId = "test_value";
            sut.SubscriptionId.ShouldBe("test_value");
        }
    }
}
