using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class SwitchedAccountAuthenticateResultModelTests
    {
        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var model = new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = "switched-token",
                EncryptedAccessToken = "encrypted-switched",
                ExpireInSeconds = 7200
            };

            model.AccessToken.ShouldBe("switched-token");
            model.EncryptedAccessToken.ShouldBe("encrypted-switched");
            model.ExpireInSeconds.ShouldBe(7200);
        }

        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_PadraoInicial_Entao_ValoresDevemSerPadrao()
        {
            var model = new SwitchedAccountAuthenticateResultModel();
            model.ExpireInSeconds.ShouldBe(0);
            model.AccessToken.ShouldBeNull();
            model.EncryptedAccessToken.ShouldBeNull();
        }
    }
}
