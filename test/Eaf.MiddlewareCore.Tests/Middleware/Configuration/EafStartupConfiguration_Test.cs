using Abp.Configuration.Startup;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Configuration
{
    public class EafStartupConfiguration_Test : EafMiddlewareTestBase
    {
        private readonly IAbpStartupConfiguration _startupConfiguration;

        public EafStartupConfiguration_Test()
        {
            _startupConfiguration = Resolve<IAbpStartupConfiguration>();
        }

        [Fact]
        public void Should_Get_Custom_Config_Providers()
        {
            var providers = _startupConfiguration.CustomConfigProviders;

            providers.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Get_Custom_Config_Providers_Values()
        {
            _startupConfiguration.GetCustomConfig().Count.ShouldBeGreaterThan(0);
        }
    }
}