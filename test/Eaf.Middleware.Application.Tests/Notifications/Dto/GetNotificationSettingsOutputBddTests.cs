using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Notifications
{
    public class GetNotificationSettingsOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetNotificationSettingsOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirReceiveNotifications_Entao_DeveArmazenar()
        {
            var sut = new GetNotificationSettingsOutput();
            sut.ReceiveNotifications = true;
            sut.ReceiveNotifications.ShouldBe(true);
        }
    }
}
