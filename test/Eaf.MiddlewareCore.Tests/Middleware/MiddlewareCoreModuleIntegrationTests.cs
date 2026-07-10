using Abp;
using Eaf.Middleware;
using Eaf.Middleware.Timing;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware
{
    public class MiddlewareCoreModuleIntegrationTests
    {
        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_InicializarAbpBootstrapper_Entao_DeveCompletarSemErros()
        {
            var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareCoreModuleIntegrationTestModule>();
            Should.NotThrow(() => bootstrapper.Initialize());
            bootstrapper.IocManager.IsRegistered<AppTimes>().ShouldBeTrue();
            bootstrapper.Dispose();
        }
    }
}
