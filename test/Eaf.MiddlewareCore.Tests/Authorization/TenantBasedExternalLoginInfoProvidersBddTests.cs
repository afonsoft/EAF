using Abp.Configuration;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using NSubstitute;
using Shouldly;
using System;
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

        [Fact]
        public void Dado_ConfiguracaoHost_Quando_GetExternalLoginInfoGoogle_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns((int?)null);
            ConfigurarCache(cacheManager);

            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "google-client-id",
                ClientSecret = "google-secret",
                UserInfoEndpoint = "https://google/user"
            };
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.Google)
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedGoogleExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Google");
            result.ClientId.ShouldBe("google-client-id");
        }

        [Fact]
        public void Dado_ConfiguracaoTenant_Quando_GetExternalLoginInfoGoogle_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns(1);
            ConfigurarCache(cacheManager);

            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "tenant-google-client-id",
                ClientSecret = "tenant-google-secret",
                UserInfoEndpoint = "https://google/tenant"
            };
            settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Google, Arg.Any<int>())
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedGoogleExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Google");
            result.ClientId.ShouldBe("tenant-google-client-id");
        }

        [Fact]
        public void Dado_ConfiguracaoHost_Quando_GetExternalLoginInfoMicrosoft_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns((int?)null);
            ConfigurarCache(cacheManager);

            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "microsoft-client-id",
                ClientSecret = "microsoft-secret",
                TenantId = "microsoft-tenant"
            };
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.Microsoft)
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedMicrosoftExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Microsoft");
            result.ClientId.ShouldBe("microsoft-client-id");
            result.TenantId.ShouldBe("microsoft-tenant");
        }

        [Fact]
        public void Dado_ConfiguracaoTenant_Quando_GetExternalLoginInfoMicrosoft_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns(1);
            ConfigurarCache(cacheManager);

            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "tenant-microsoft-client-id",
                ClientSecret = "tenant-microsoft-secret",
                TenantId = "tenant-microsoft-tenant"
            };
            settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Microsoft, Arg.Any<int>())
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedMicrosoftExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Microsoft");
            result.ClientId.ShouldBe("tenant-microsoft-client-id");
        }

        [Fact]
        public void Dado_ConfiguracaoHost_Quando_GetExternalLoginInfoAuthZero_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns((int?)null);
            ConfigurarCache(cacheManager);

            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "authzero-client-id",
                ClientSecret = "authzero-secret",
                Endpoint = "https://authzero/endpoint"
            };
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.AuthZero)
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedAuthZeroExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("AuthZero");
            result.ClientId.ShouldBe("authzero-client-id");
            result.AdditionalParams["Endpoint"].ShouldBe("https://authzero/endpoint");
        }

        [Fact]
        public void Dado_ConfiguracaoTenant_Quando_GetExternalLoginInfoAuthZero_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns(1);
            ConfigurarCache(cacheManager);

            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "tenant-authzero-client-id",
                ClientSecret = "tenant-authzero-secret",
                Endpoint = "https://authzero/tenant"
            };
            settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.AuthZero, Arg.Any<int>())
                .Returns(settings.ToJsonString());

            var provider = new TenantBasedAuthZeroExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("AuthZero");
            result.ClientId.ShouldBe("tenant-authzero-client-id");
        }

        [Fact]
        public void Dado_ConfiguracaoHost_Quando_GetExternalLoginInfoOpenIdConnect_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns((int?)null);
            ConfigurarCache(cacheManager);

            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "oidc-client-id",
                ClientSecret = "oidc-secret",
                Authority = "https://oidc/authority",
                LoginUrl = "https://oidc/login",
                ValidateIssuer = true
            };
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.OpenIdConnect)
                .Returns(settings.ToJsonString());
            settingManager.GetSettingValue(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims)
                .Returns("[]");

            var provider = new TenantBasedOpenIdConnectExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("OpenIdConnect");
            result.ClientId.ShouldBe("oidc-client-id");
            result.AdditionalParams["Authority"].ShouldBe("https://oidc/authority");
        }

        [Fact]
        public void Dado_ConfiguracaoTenant_Quando_GetExternalLoginInfoOpenIdConnect_Entao_DeveRetornarProviderInfo()
        {
            // Dado
            var (settingManager, session, cacheManager) = CriarDependencias();
            session.TenantId.Returns(1);
            ConfigurarCache(cacheManager);

            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "tenant-oidc-client-id",
                ClientSecret = "tenant-oidc-secret",
                Authority = "https://oidc/tenant",
                LoginUrl = "https://oidc/tenant-login",
                ValidateIssuer = true
            };
            settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect, Arg.Any<int>())
                .Returns(settings.ToJsonString());
            settingManager.GetSettingValue(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims)
                .Returns("[]");

            var provider = new TenantBasedOpenIdConnectExternalLoginInfoProvider(settingManager, session, cacheManager);

            // Quando
            var result = provider.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("OpenIdConnect");
            result.ClientId.ShouldBe("tenant-oidc-client-id");
        }

        private static void ConfigurarCache(ICacheManager cacheManager)
        {
            var cache = Substitute.For<ICache>();
            cache.Get(Arg.Any<string>(), Arg.Any<Func<string, object>>())
                .Returns(callInfo => callInfo.Arg<Func<string, object>>().Invoke(callInfo.Arg<string>()));

            cacheManager.GetCache("AppExternalLoginInfoProvidersCache").Returns(cache);
        }
    }
}
