using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ImpersonatedAuthenticateResultModelTests
    {
        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_Criado_Entao_PropriedadesDevemSerAtribuidas()
        {
            var model = new ImpersonatedAuthenticateResultModel
            {
                AccessToken = "access-token-xyz",
                EncryptedAccessToken = "encrypted-token-xyz",
                ExpireInSeconds = 3600
            };

            model.AccessToken.ShouldBe("access-token-xyz");
            model.EncryptedAccessToken.ShouldBe("encrypted-token-xyz");
            model.ExpireInSeconds.ShouldBe(3600);
        }

        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_PadraoInicial_Entao_ExpireInSecondsDeveSerZero()
        {
            var model = new ImpersonatedAuthenticateResultModel();
            model.ExpireInSeconds.ShouldBe(0);
            model.AccessToken.ShouldBeNull();
            model.EncryptedAccessToken.ShouldBeNull();
        }
    }
}
