using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.Microsoft;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para MicrosoftExternalLoginInfoProvider seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MicrosoftExternalLoginInfoProviderBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("key", "secret", "tenant");
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_NameDeveSerMicrosoft()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("key", "secret", "tenant");
            sut.Name.ShouldBe("Microsoft");
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveImplementarIExternalLoginInfoProvider()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("key", "secret", "tenant");
            sut.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
        }

        #endregion

        #region GetExternalLoginInfo

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveRetornarInfoComNomeMicrosoft()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("consumer-key", "consumer-secret", "consumer-tenant");
            var result = sut.GetExternalLoginInfo();
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Microsoft");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientId()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("my-key", "my-secret", "my-tenant");
            var result = sut.GetExternalLoginInfo();
            result.ClientId.ShouldBe("my-key");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_DeveConterClientSecret()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("key", "my-secret-val", "tenant");
            var result = sut.GetExternalLoginInfo();
            result.ClientSecret.ShouldBe("my-secret-val");
        }

        [Fact]
        public void Dado_ProviderCriado_Quando_GetExternalLoginInfo_Entao_ProviderApiTypeDeveSerMicrosoftAuthProviderApi()
        {
            var sut = new MicrosoftExternalLoginInfoProvider("key", "secret", "tenant");
            var result = sut.GetExternalLoginInfo();
            result.ProviderApiType.ShouldBe(typeof(MicrosoftAuthProviderApi));
        }

        #endregion
    }
}
