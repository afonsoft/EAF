using Eaf.Middleware.Web.Notifications;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Notifications
{
    /// <summary>
    /// Testes BDD para MiddlewareNotificationNames seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class MiddlewareNotificationNamesBddTests
    {
        [Fact]
        public void Dado_WelcomeToTheApplication_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            MiddlewareNotificationNames.WelcomeToTheApplication.ShouldBe("App.WelcomeToTheApplication");
        }

        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            MiddlewareNotificationNames.NewUserRegistered.ShouldBe("App.NewUserRegistered");
        }
    }
}
