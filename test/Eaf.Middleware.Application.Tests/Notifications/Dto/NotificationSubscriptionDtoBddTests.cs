using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Notifications
{
    public class NotificationSubscriptionDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new NotificationSubscriptionDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new NotificationSubscriptionDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }
    }
}
