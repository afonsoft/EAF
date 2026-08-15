using System.Linq;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Push.Tests
{
    /// <summary>
    /// Testes de integração do módulo Eaf.Notifications.Push.
    /// </summary>
    public class EafNotificationsPushModuleTests : EafNotificationsPushTestBase
    {
        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverProviders_Entao_DeveConterWebPushEGenericHttp()
        {
            var providers = IocManager.IocContainer.ResolveAll<IPushNotificationProvider>().ToList();

            providers.Count.ShouldBe(2);
            providers.Any(p => p.Name == "WebPush").ShouldBeTrue();
            providers.Any(p => p.Name == "GenericHttp").ShouldBeTrue();
        }

        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverPushSender_Entao_DeveSerEafPushNotificationSender()
        {
            var sender = Resolve<IPushNotificationSender>();

            sender.ShouldNotBeNull();
            sender.ShouldBeOfType<EafPushNotificationSender>();
        }
    }
}
