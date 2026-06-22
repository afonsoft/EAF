using Eaf.Middleware.Web.Notifications;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    /// <summary>
    /// Testes BDD para MiddlewareNotificationNames seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareNotificationNamesBddTests
    {
        [Fact]
        public void Dado_MiddlewareNotificationNames_Quando_VerificarWelcomeToTheApplication_Entao_DeveSerCorreto()
        {
            MiddlewareNotificationNames.WelcomeToTheApplication.ShouldBe("App.WelcomeToTheApplication");
        }

        [Fact]
        public void Dado_MiddlewareNotificationNames_Quando_VerificarNewUserRegistered_Entao_DeveSerCorreto()
        {
            MiddlewareNotificationNames.NewUserRegistered.ShouldBe("App.NewUserRegistered");
        }

        [Fact]
        public void Dado_MiddlewareNotificationNames_Quando_VerificarPrefixo_Entao_DeveComecarComApp()
        {
            MiddlewareNotificationNames.WelcomeToTheApplication.ShouldStartWith("App.");
            MiddlewareNotificationNames.NewUserRegistered.ShouldStartWith("App.");
        }
    }
}
