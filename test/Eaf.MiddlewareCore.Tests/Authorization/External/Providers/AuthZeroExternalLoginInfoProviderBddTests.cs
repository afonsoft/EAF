using Eaf.Middleware.Authorization.External.AuthZero;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.AuthZero;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para AuthZeroExternalLoginInfoProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AuthZeroExternalLoginInfoProviderBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("key", "secret", "tenant");
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_NameDeveSerAuthZero()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("key", "secret", "tenant");
            sut.Name.ShouldBe("AuthZero");
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveImplementarIExternalLoginInfoProvider()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("key", "secret", "tenant");
            sut.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
        }

        #endregion

        #region GetExternalLoginInfo

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveRetornarInfoComNomeAuthZero()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("consumer-key", "consumer-secret", "consumer-tenant");
            var result = sut.GetExternalLoginInfo();
            result.ShouldNotBeNull();
            result.Name.ShouldBe("AuthZero");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientId()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("my-key", "my-secret", "my-tenant");
            var result = sut.GetExternalLoginInfo();
            result.ClientId.ShouldBe("my-key");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientSecret()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("key", "my-secret-123", "tenant");
            var result = sut.GetExternalLoginInfo();
            result.ClientSecret.ShouldBe("my-secret-123");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_ProviderApiTypeDeveSerAuthZeroAuthProviderApi()
        {
            var sut = new AuthZeroExternalLoginInfoProvider("key", "secret", "tenant");
            var result = sut.GetExternalLoginInfo();
            result.ProviderApiType.ShouldBe(typeof(AuthZeroAuthProviderApi));
        }

        #endregion
    }
}
