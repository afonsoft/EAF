using Abp.Notifications;
using Eaf.Middleware.Web.Notifications;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    /// <summary>
    /// Testes BDD para MiddlewareNotificationProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareNotificationProviderBddTests
    {
        #region SetNotifications

        [Fact]
        public void Dado_NotificationProvider_Quando_SetNotifications_Entao_DeveRegistrarNotificacoes()
        {
            // Dado
            var provider = new MiddlewareNotificationProvider();
            var context = Substitute.For<INotificationDefinitionContext>();
            var manager = Substitute.For<INotificationDefinitionManager>();
            context.Manager.Returns(manager);

            // Quando
            provider.SetNotifications(context);

            // Entao
            manager.Received(2).Add(Arg.Any<NotificationDefinition>());
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_MiddlewareNotificationProvider_Quando_CriarInstancia_Entao_DeveSerNotificationProvider()
        {
            var provider = new MiddlewareNotificationProvider();
            provider.ShouldNotBeNull();
            provider.ShouldBeAssignableTo<NotificationProvider>();
        }

        #endregion
    }
}
