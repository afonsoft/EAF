using Eaf.Middleware.Web.Notifications;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    public class NotificationNamesTests
    {
        [Fact]
        public void Dado_WelcomeToTheApplication_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareNotificationNames.WelcomeToTheApplication.ShouldBe("App.WelcomeToTheApplication");
        }

        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveSerCorreto()
        {
            MiddlewareNotificationNames.NewUserRegistered.ShouldBe("App.NewUserRegistered");
        }
    }
}
