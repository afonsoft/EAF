using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class SwitchedAccountAuthenticateResultModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SwitchedAccountAuthenticateResultModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAccessToken_Entao_DeveArmazenar()
        {
            var sut = new SwitchedAccountAuthenticateResultModel();
            sut.AccessToken = "jwt_token";
            sut.AccessToken.ShouldBe("jwt_token");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEncryptedAccessToken_Entao_DeveArmazenar()
        {
            var sut = new SwitchedAccountAuthenticateResultModel();
            sut.EncryptedAccessToken = "encrypted";
            sut.EncryptedAccessToken.ShouldBe("encrypted");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirExpireInSeconds_Entao_DeveArmazenar()
        {
            var sut = new SwitchedAccountAuthenticateResultModel();
            sut.ExpireInSeconds = 7200;
            sut.ExpireInSeconds.ShouldBe(7200);
        }
    }
}
