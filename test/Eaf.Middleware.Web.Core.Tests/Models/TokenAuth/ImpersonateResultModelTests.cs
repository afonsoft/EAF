using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class ImpersonateResultModelTests
    {
        [Fact]
        public void Dado_ImpersonateResultModel_Quando_Criado_Entao_TokenDeveSerAtribuido()
        {
            var model = new ImpersonateResultModel
            {
                ImpersonationToken = "abc-token-123"
            };

            model.ImpersonationToken.ShouldBe("abc-token-123");
        }

        [Fact]
        public void Dado_ImpersonateResultModel_Quando_PadraoInicial_Entao_TokenDeveSerNulo()
        {
            var model = new ImpersonateResultModel();
            model.ImpersonationToken.ShouldBeNull();
        }
    }
}
