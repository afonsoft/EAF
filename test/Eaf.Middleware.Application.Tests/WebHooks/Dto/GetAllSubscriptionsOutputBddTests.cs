using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllSubscriptionsOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllSubscriptionsOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new GetAllSubscriptionsOutput();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWebhookUri_Entao_DeveArmazenar()
        {
            var sut = new GetAllSubscriptionsOutput();
            sut.WebhookUri = "test_value";
            sut.WebhookUri.ShouldBe("test_value");
        }
    }
}
