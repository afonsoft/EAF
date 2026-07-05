using Abp.Configuration;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para os provedores de login externo baseados em tenant seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class TenantBasedExternalLoginInfoProvidersBddTests
    {
        private static (ISettingManager, IAbpSession, ICacheManager) CriarDependencias()
        {
            return (
                Substitute.For<ISettingManager>(),
                Substitute.For<IAbpSession>(),
                Substitute.For<ICacheManager>());
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarGoogleProvider_Entao_DeveInstanciarComName()
        {
            var (settingManager, session, cacheManager) = CriarDependencias();

            var provider = new TenantBasedGoogleExternalLoginInfoProvider(settingManager, session, cacheManager);

            provider.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
            provider.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarAuthZeroProvider_Entao_DeveInstanciarComName()
        {
            var (settingManager, session, cacheManager) = CriarDependencias();

            var provider = new TenantBasedAuthZeroExternalLoginInfoProvider(settingManager, session, cacheManager);

            provider.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
            provider.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarMicrosoftProvider_Entao_DeveInstanciarComName()
        {
            var (settingManager, session, cacheManager) = CriarDependencias();

            var provider = new TenantBasedMicrosoftExternalLoginInfoProvider(settingManager, session, cacheManager);

            provider.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
            provider.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarOpenIdConnectProvider_Entao_DeveInstanciarComName()
        {
            var (settingManager, session, cacheManager) = CriarDependencias();

            var provider = new TenantBasedOpenIdConnectExternalLoginInfoProvider(settingManager, session, cacheManager);

            provider.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
            provider.Name.ShouldNotBeNullOrEmpty();
        }
    }
}
