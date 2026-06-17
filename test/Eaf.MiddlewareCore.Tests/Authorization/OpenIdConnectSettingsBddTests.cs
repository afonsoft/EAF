using Abp.UI;
using Eaf.Middleware.Core.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para OpenIdConnectExternalLoginProviderSettings seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class OpenIdConnectSettingsBddTests
    {
        [Fact]
        public void Dado_Settings_ComClientIdEAuthority_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "client-id",
                Authority = "https://login.microsoftonline.com/tenant"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_Settings_SemClientIdEAuthority_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "",
                Authority = ""
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_Settings_ComAuthorityHttpSemHttps_Quando_IsValid_Entao_DeveLancarExcecao()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "client-id",
                Authority = "http://insecure.example.com"
            };

            Should.Throw<UserFriendlyException>(() => settings.IsValid());
        }

        [Fact]
        public void Dado_Settings_ApenasComClientId_SemAuthority_Quando_IsValid_Entao_DeveLancarNullReferenceException()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "client-id",
                Authority = null
            };

            // Authority null causa NRE no StartsWith - comportamento atual do código
            Should.Throw<System.NullReferenceException>(() => settings.IsValid());
        }

        [Fact]
        public void Dado_Settings_ApenasComAuthority_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = null,
                Authority = "https://login.example.com"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_Settings_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "my-client",
                ClientSecret = "my-secret",
                Authority = "https://auth.example.com",
                LoginUrl = "https://auth.example.com/login",
                ValidateIssuer = true
            };

            settings.ClientId.ShouldBe("my-client");
            settings.ClientSecret.ShouldBe("my-secret");
            settings.Authority.ShouldBe("https://auth.example.com");
            settings.LoginUrl.ShouldBe("https://auth.example.com/login");
            settings.ValidateIssuer.ShouldBeTrue();
        }
    }
}
