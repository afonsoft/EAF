using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para ExternalAuthConfiguration e classes relacionadas
    /// </summary>
    public class ExternalAuthConfigurationBddTests
    {
        #region ExternalAuthConfiguration

        [Fact]
        public void Dado_ExternalAuthConfiguration_Quando_Criar_Entao_DeveInicializarLista()
        {
            var config = new ExternalAuthConfiguration();
            config.ExternalLoginInfoProviders.ShouldNotBeNull();
            config.ExternalLoginInfoProviders.Count.ShouldBe(0);
        }

        #endregion

        #region ExternalAuthUserInfo

        [Fact]
        public void Dado_ExternalAuthUserInfo_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var info = new ExternalAuthUserInfo
            {
                EmailAddress = "user@acme.com",
                Name = "João",
                Surname = "Silva",
                Provider = "Google",
                ProviderKey = "google-key-123",
                Picture = "https://photo.url/pic.jpg",
                AccessCode = "access-code-xyz"
            };

            info.EmailAddress.ShouldBe("user@acme.com");
            info.Name.ShouldBe("João");
            info.Surname.ShouldBe("Silva");
            info.Provider.ShouldBe("Google");
            info.ProviderKey.ShouldBe("google-key-123");
            info.Picture.ShouldBe("https://photo.url/pic.jpg");
            info.AccessCode.ShouldBe("access-code-xyz");
        }

        #endregion

        #region ExternalLoginProviderInfo

        [Fact]
        public void Dado_ExternalLoginProviderInfo_Quando_CriarComParametros_Entao_DeveDefinirPropriedades()
        {
            var provider = new ExternalLoginProviderInfo(
                "Google", "client-id", "client-secret", "tenant-1", typeof(object));

            provider.Name.ShouldBe("Google");
            provider.ClientId.ShouldBe("client-id");
            provider.ClientSecret.ShouldBe("client-secret");
            provider.TenantId.ShouldBe("tenant-1");
            provider.ProviderApiType.ShouldBe(typeof(object));
            provider.AdditionalParams.ShouldNotBeNull();
            provider.AdditionalParams.Count.ShouldBe(0);
            provider.ClaimMappings.ShouldNotBeNull();
            provider.ClaimMappings.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_ExternalLoginProviderInfo_Quando_CriarComParamsAdicionais_Entao_DeveArmazenar()
        {
            var additionalParams = new Dictionary<string, string> { { "scope", "email" } };
            var claimMappings = new List<JsonClaimMap>
            {
                new JsonClaimMap { Claim = "email", Key = "emailAddress" }
            };

            var provider = new ExternalLoginProviderInfo(
                "AuthZero", "id", "secret", "t1", typeof(string),
                additionalParams, claimMappings);

            provider.AdditionalParams.Count.ShouldBe(1);
            provider.AdditionalParams["scope"].ShouldBe("email");
            provider.ClaimMappings.Count.ShouldBe(1);
            provider.ClaimMappings[0].Claim.ShouldBe("email");
        }

        #endregion

        #region JsonClaimMap

        [Fact]
        public void Dado_JsonClaimMap_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var map = new JsonClaimMap
            {
                Claim = "sub",
                Key = "subject"
            };

            map.Claim.ShouldBe("sub");
            map.Key.ShouldBe("subject");
        }

        #endregion

        #region MicrosoftExternalLoginProviderSettings

        [Fact]
        public void Dado_MicrosoftSettingsValido_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "ms-client-id",
                ClientSecret = "ms-client-secret",
                TenantId = "ms-tenant"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_MicrosoftSettingsSemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientSecret = "secret"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_MicrosoftSettingsSemClientSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "client"
            };

            settings.IsValid().ShouldBeFalse();
        }

        #endregion

        #region OpenIdConnectExternalLoginProviderSettings

        [Fact]
        public void Dado_OpenIdConnectValido_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "oidc-client",
                Authority = "https://login.provider.com",
                ValidateIssuer = true
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_OpenIdConnectSemClientIdComAuthority_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                Authority = "https://login.provider.com"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_OpenIdConnectSemHttps_Quando_IsValid_Entao_DeveLancarExcecao()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "client",
                Authority = "http://insecure.provider.com"
            };

            Should.Throw<Abp.UI.UserFriendlyException>(() => settings.IsValid());
        }

        [Fact]
        public void Dado_OpenIdConnectSemClientIdSemAuthority_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings();
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_OpenIdConnectPropriedades_Quando_Definir_Entao_DeveArmazenar()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "client",
                ClientSecret = "secret",
                Authority = "https://auth.com",
                LoginUrl = "https://auth.com/login",
                ValidateIssuer = false
            };

            settings.ClientSecret.ShouldBe("secret");
            settings.LoginUrl.ShouldBe("https://auth.com/login");
            settings.ValidateIssuer.ShouldBeFalse();
        }

        #endregion
    }
}
