using Abp.Configuration;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using Eaf.Middleware.Web.Configuration;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class ExternalAuthConfigurerBddTests
    {
        [Fact]
        public void Dado_IocManagerComProviders_Quando_Configure_Entao_DeveAdicionarProvidersNaConfiguracao()
        {
            // Dado
            var externalAuthConfiguration = new ExternalAuthConfiguration();
            var iocManager = Substitute.For<Abp.Dependency.IIocManager>();

            iocManager.Resolve<ExternalAuthConfiguration>().Returns(externalAuthConfiguration);
            iocManager.Resolve<TenantBasedOpenIdConnectExternalLoginInfoProvider>().Returns(Substitute.For<TenantBasedOpenIdConnectExternalLoginInfoProvider>(Substitute.For<ISettingManager>(), Substitute.For<IAbpSession>(), Substitute.For<ICacheManager>()));
            iocManager.Resolve<TenantBasedGoogleExternalLoginInfoProvider>().Returns(Substitute.For<TenantBasedGoogleExternalLoginInfoProvider>(Substitute.For<ISettingManager>(), Substitute.For<IAbpSession>(), Substitute.For<ICacheManager>()));
            iocManager.Resolve<TenantBasedMicrosoftExternalLoginInfoProvider>().Returns(Substitute.For<TenantBasedMicrosoftExternalLoginInfoProvider>(Substitute.For<ISettingManager>(), Substitute.For<IAbpSession>(), Substitute.For<ICacheManager>()));
            iocManager.Resolve<TenantBasedAuthZeroExternalLoginInfoProvider>().Returns(Substitute.For<TenantBasedAuthZeroExternalLoginInfoProvider>(Substitute.For<ISettingManager>(), Substitute.For<IAbpSession>(), Substitute.For<ICacheManager>()));

            // Quando
            ExternalAuthConfigurer.Configure(iocManager);

            // Então
            externalAuthConfiguration.ExternalLoginInfoProviders.Count.ShouldBe(4);
        }

        [Fact]
        public void Dado_ConfiguracaoNula_Quando_Configure_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var iocManager = Substitute.For<Abp.Dependency.IIocManager>();
            iocManager.Resolve<ExternalAuthConfiguration>().Returns((ExternalAuthConfiguration)null!);

            // Quando & Então
            Should.NotThrow(() => ExternalAuthConfigurer.Configure(iocManager));
        }
    }
}
