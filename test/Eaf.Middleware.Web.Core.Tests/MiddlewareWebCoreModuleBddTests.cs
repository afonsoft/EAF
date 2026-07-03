using Eaf.Middleware.Web;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore
{
    public class MiddlewareWebCoreModuleBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarModulo_Entao_DeveTerNomeCorreto()
        {
            typeof(MiddlewareWebCoreModule).Name.ShouldBe("MiddlewareWebCoreModule");
        }
    }
}
