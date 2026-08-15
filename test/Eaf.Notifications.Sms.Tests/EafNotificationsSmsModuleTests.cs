using System.Linq;
using Shouldly;
using Xunit;

namespace Eaf.Notifications.Sms.Tests
{
    /// <summary>
    /// Testes de integração do módulo Eaf.Notifications.Sms.
    /// </summary>
    public class EafNotificationsSmsModuleTests : EafNotificationsSmsTestBase
    {
        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverProviders_Entao_DeveConterGenericHttpETwilio()
        {
            var providers = IocManager.IocContainer.ResolveAll<ISmsProvider>().ToList();

            providers.Count.ShouldBe(2);
            providers.Any(p => p.Name == "GenericHttp").ShouldBeTrue();
            providers.Any(p => p.Name == "Twilio").ShouldBeTrue();
        }

        [Fact]
        public void Dado_ModuloInicializado_Quando_ResolverSmsSender_Entao_DeveSerEafSmsSender()
        {
            var sender = Resolve<ISmsSender>();

            sender.ShouldNotBeNull();
            sender.ShouldBeOfType<EafSmsSender>();
        }
    }
}
