using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class ExternalAuthenticateModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExternalAuthenticateModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAuthProvider_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateModel();
            sut.AuthProvider = "Google";
            sut.AuthProvider.ShouldBe("Google");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirProviderAccessCode_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateModel();
            sut.ProviderAccessCode = "access_code_123";
            sut.ProviderAccessCode.ShouldBe("access_code_123");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirProviderKey_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateModel();
            sut.ProviderKey = "key_123";
            sut.ProviderKey.ShouldBe("key_123");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirReturnUrl_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateModel();
            sut.ReturnUrl = "https://example.com/callback";
            sut.ReturnUrl.ShouldBe("https://example.com/callback");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSingleSignIn_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateModel();
            sut.SingleSignIn = true;
            sut.SingleSignIn.ShouldBe(true);
        }
    }
}
