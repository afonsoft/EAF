using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class SendTwoFactorAuthCodeModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SendTwoFactorAuthCodeModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirProvider_Entao_DeveArmazenar()
        {
            var sut = new SendTwoFactorAuthCodeModel();
            sut.Provider = "Email";
            sut.Provider.ShouldBe("Email");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new SendTwoFactorAuthCodeModel();
            sut.UserId = 42L;
            sut.UserId.ShouldBe(42L);
        }
    }
}
