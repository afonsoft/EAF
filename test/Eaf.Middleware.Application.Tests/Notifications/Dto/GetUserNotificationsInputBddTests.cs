using Eaf.Middleware.Notifications.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Notifications
{
    public class GetUserNotificationsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetUserNotificationsInput();
            sut.ShouldNotBeNull();
        }
    }
}
