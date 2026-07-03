using Eaf.Middleware.Web.Controllers;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public class TokenAuthControllerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(TokenAuthController).Name.ShouldBe("TokenAuthController");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            typeof(TokenAuthController).BaseType.Name.ShouldBe("MiddlewareControllerBase");
        }
    }
}
