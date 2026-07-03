using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class ExternalAuthenticateResultModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAccessToken_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.AccessToken = "jwt_token";
            sut.AccessToken.ShouldBe("jwt_token");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEncryptedAccessToken_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.EncryptedAccessToken = "encrypted_token";
            sut.EncryptedAccessToken.ShouldBe("encrypted_token");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirExpireInSeconds_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.ExpireInSeconds = 3600;
            sut.ExpireInSeconds.ShouldBe(3600);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWaitingForActivation_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.WaitingForActivation = true;
            sut.WaitingForActivation.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new ExternalAuthenticateResultModel();
            sut.UserId = 42L;
            sut.UserId.ShouldBe(42L);
        }
    }
}
