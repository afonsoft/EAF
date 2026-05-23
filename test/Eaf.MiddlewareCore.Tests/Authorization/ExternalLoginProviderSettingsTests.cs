using Eaf.Middleware.Core.Authentication;
using Abp.UI;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization
{
    public class ExternalLoginProviderSettingsTests
    {
        [Fact]
        public void Dado_AuthZeroValido_Quando_ChamarIsValid_Entao_DeveRetornarTrue()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret",
                Endpoint = "https://auth0.example.com"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthZeroSemClientId_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "",
                ClientSecret = "csecret",
                Endpoint = "https://auth0.example.com"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSemClientSecret_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = null,
                Endpoint = "https://auth0.example.com"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSemEndpoint_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret",
                Endpoint = ""
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleValido_Quando_ChamarIsValid_Entao_DeveRetornarTrue()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_GoogleSemClientId_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "",
                ClientSecret = "csecret"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleComUserInfoEndpoint_Quando_DefinirValor_Entao_DeveArmazenar()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret",
                UserInfoEndpoint = "https://googleapis.com/userinfo"
            };
            settings.UserInfoEndpoint.ShouldBe("https://googleapis.com/userinfo");
        }

        [Fact]
        public void Dado_MicrosoftValido_Quando_ChamarIsValid_Entao_DeveRetornarTrue()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret",
                TenantId = "tid"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_MicrosoftSemClientId_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = null,
                ClientSecret = "csecret"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_OpenIdConnectValido_Quando_ChamarIsValid_Entao_DeveRetornarTrue()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "cid",
                Authority = "https://authority.example.com"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_OpenIdConnectSemHttps_Quando_ChamarIsValid_Entao_DeveLancarException()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "cid",
                Authority = "http://authority.example.com"
            };
            Should.Throw<UserFriendlyException>(() => settings.IsValid());
        }

        [Fact]
        public void Dado_OpenIdConnectSemClientIdEAuthority_Quando_ChamarIsValid_Entao_DeveRetornarFalse()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "",
                Authority = ""
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_OpenIdConnectComPropriedadesAdicionais_Quando_Definir_Entao_DeveArmazenar()
        {
            var settings = new OpenIdConnectExternalLoginProviderSettings
            {
                ClientId = "cid",
                ClientSecret = "csecret",
                Authority = "https://auth.example.com",
                LoginUrl = "https://auth.example.com/login",
                ValidateIssuer = true
            };
            settings.LoginUrl.ShouldBe("https://auth.example.com/login");
            settings.ValidateIssuer.ShouldBeTrue();
            settings.ClientSecret.ShouldBe("csecret");
        }
    }
}
