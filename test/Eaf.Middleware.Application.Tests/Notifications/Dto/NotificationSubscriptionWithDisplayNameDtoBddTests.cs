using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Notifications
{
    public class NotificationSubscriptionWithDisplayNameDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new NotificationSubscriptionWithDisplayNameDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDescription_Entao_DeveArmazenar()
        {
            var sut = new NotificationSubscriptionWithDisplayNameDto();
            sut.Description = "test_value";
            sut.Description.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new NotificationSubscriptionWithDisplayNameDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }
    }
}
