using Eaf.Middleware.Core.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para settings de provedores de login externo seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ExternalLoginProviderSettingsBddTests
    {
        #region AuthZeroExternalLoginProviderSettings

        [Fact]
        public void Dado_AuthZeroValido_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "client-123",
                ClientSecret = "secret-456",
                Endpoint = "https://auth0.acme.com"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthZeroSemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientSecret = "secret",
                Endpoint = "https://auth0.acme.com"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSemClientSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "client",
                Endpoint = "https://auth0.acme.com"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSemEndpoint_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "client",
                ClientSecret = "secret"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroComClientIdVazio_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "  ",
                ClientSecret = "secret",
                Endpoint = "https://auth0.acme.com"
            };

            settings.IsValid().ShouldBeFalse();
        }

        #endregion

        #region GoogleExternalLoginProviderSettings

        [Fact]
        public void Dado_GoogleValido_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "google-client-id",
                ClientSecret = "google-client-secret"
            };

            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_GoogleSemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientSecret = "secret"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleSemClientSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "client"
            };

            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleComUserInfoEndpoint_Quando_DefinirPropriedade_Entao_DeveArmazenar()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "client",
                ClientSecret = "secret",
                UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo"
            };

            settings.UserInfoEndpoint.ShouldBe("https://www.googleapis.com/oauth2/v3/userinfo");
            settings.IsValid().ShouldBeTrue();
        }

        #endregion

        #region JsonClaimMapDto

        [Fact]
        public void Dado_JsonClaimMapDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new JsonClaimMapDto
            {
                Claim = "email",
                Key = "emailAddress"
            };

            dto.Claim.ShouldBe("email");
            dto.Key.ShouldBe("emailAddress");
        }

        #endregion
    }
}
