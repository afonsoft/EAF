using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class ActivateWebhookSubscriptionInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ActivateWebhookSubscriptionInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSubscriptionId_Entao_DeveArmazenar()
        {
            var sut = new ActivateWebhookSubscriptionInput();
            var guid = System.Guid.NewGuid(); sut.SubscriptionId = guid;
            sut.SubscriptionId.ShouldBe(guid);
        }
    }
}
