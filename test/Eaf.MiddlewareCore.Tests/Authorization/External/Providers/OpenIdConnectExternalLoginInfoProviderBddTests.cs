using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.OpenIdConnect;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para OpenIdConnectExternalLoginInfoProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class OpenIdConnectExternalLoginInfoProviderBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client-id", "client-secret", "https://authority", "https://login", true, new List<JsonClaimMap>());
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_NameDeveSerOpenIdConnect()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client-id", "client-secret", "https://authority", "https://login", true, new List<JsonClaimMap>());
            sut.Name.ShouldBe("OpenIdConnect");
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveImplementarIExternalLoginInfoProvider()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client-id", "client-secret", "https://authority", "https://login", false, new List<JsonClaimMap>());
            sut.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
        }

        #endregion

        #region GetExternalLoginInfo

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveRetornarInfoComNomeOpenIdConnect()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client-id", "client-secret", "https://authority", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.ShouldNotBeNull();
            result.Name.ShouldBe("OpenIdConnect");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientId()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "my-oidc-client", "secret", "https://auth", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.ClientId.ShouldBe("my-oidc-client");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientSecret()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "my-oidc-secret", "https://auth", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.ClientSecret.ShouldBe("my-oidc-secret");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterAuthority()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://my-authority.com", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.AdditionalParams.ShouldContainKey("Authority");
            result.AdditionalParams["Authority"].ShouldBe("https://my-authority.com");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterLoginUrl()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://auth", "https://my-login-url.com", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.AdditionalParams.ShouldContainKey("LoginUrl");
            result.AdditionalParams["LoginUrl"].ShouldBe("https://my-login-url.com");
        }

        [Fact]
        public void Dado_ValidateIssuerTrue_Quando_GetExternalLoginInfo_Entao_DeveConterValidateIssuerTrue()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://auth", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.AdditionalParams["ValidateIssuer"].ShouldBe("True");
        }

        [Fact]
        public void Dado_ValidateIssuerFalse_Quando_GetExternalLoginInfo_Entao_DeveConterValidateIssuerFalse()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://auth", "https://login", false, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.AdditionalParams["ValidateIssuer"].ShouldBe("False");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_ProviderApiTypeDeveSerOpenIdConnectAuthProviderApi()
        {
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://auth", "https://login", true, new List<JsonClaimMap>());
            var result = sut.GetExternalLoginInfo();
            result.ProviderApiType.ShouldBe(typeof(OpenIdConnectAuthProviderApi));
        }

        [Fact]
        public void Dado_ClaimMaps_Quando_GetExternalLoginInfo_Entao_DeveConterClaimMappings()
        {
            var claimMaps = new List<JsonClaimMap>
            {
                new JsonClaimMap { Key = "email_key", Claim = "email" }
            };
            var sut = new OpenIdConnectExternalLoginInfoProvider(
                "client", "secret", "https://auth", "https://login", true, claimMaps);
            var result = sut.GetExternalLoginInfo();
            result.ClaimMappings.ShouldNotBeNull();
            result.ClaimMappings.Count.ShouldBe(1);
        }

        #endregion
    }
}
