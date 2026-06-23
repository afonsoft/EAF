using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.Google;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para GoogleExternalLoginInfoProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class GoogleExternalLoginInfoProviderBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new GoogleExternalLoginInfoProvider("client-id", "client-secret", "https://endpoint");
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_NameDeveSerGoogle()
        {
            var sut = new GoogleExternalLoginInfoProvider("client-id", "client-secret", "https://endpoint");
            sut.Name.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveImplementarIExternalLoginInfoProvider()
        {
            var sut = new GoogleExternalLoginInfoProvider("client-id", "client-secret", "https://endpoint");
            sut.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
        }

        #endregion

        #region GetExternalLoginInfo

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveRetornarInfoComNomeGoogle()
        {
            // Dado
            var sut = new GoogleExternalLoginInfoProvider("my-client", "my-secret", "https://googleapis.com/userinfo");

            // Quando
            var result = sut.GetExternalLoginInfo();

            // Entao
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientId()
        {
            var sut = new GoogleExternalLoginInfoProvider("my-client-id", "my-secret", "https://endpoint");
            var result = sut.GetExternalLoginInfo();
            result.ClientId.ShouldBe("my-client-id");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientSecret()
        {
            var sut = new GoogleExternalLoginInfoProvider("id", "my-secret", "https://endpoint");
            var result = sut.GetExternalLoginInfo();
            result.ClientSecret.ShouldBe("my-secret");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterUserInfoEndpoint()
        {
            var sut = new GoogleExternalLoginInfoProvider("id", "secret", "https://www.googleapis.com/oauth2/v3/userinfo");
            var result = sut.GetExternalLoginInfo();
            result.AdditionalParams.ShouldContainKey("UserInfoEndpoint");
            result.AdditionalParams["UserInfoEndpoint"].ShouldBe("https://www.googleapis.com/oauth2/v3/userinfo");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_ProviderApiTypeDeveSerGoogleAuthProviderApi()
        {
            var sut = new GoogleExternalLoginInfoProvider("id", "secret", "https://endpoint");
            var result = sut.GetExternalLoginInfo();
            result.ProviderApiType.ShouldBe(typeof(GoogleAuthProviderApi));
        }

        #endregion
    }
}
