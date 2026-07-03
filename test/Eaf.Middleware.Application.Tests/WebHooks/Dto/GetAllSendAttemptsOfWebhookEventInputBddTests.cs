using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllSendAttemptsOfWebhookEventInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllSendAttemptsOfWebhookEventInput();
            sut.ShouldNotBeNull();
        }
    }
}
