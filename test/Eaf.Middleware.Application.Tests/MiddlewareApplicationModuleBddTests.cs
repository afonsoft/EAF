using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class MiddlewareApplicationModuleBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarModulo_Entao_DeveTerNomeCorreto()
        {
            typeof(MiddlewareApplicationModule).ShouldNotBeNull();
            typeof(MiddlewareApplicationModule).Name.ShouldBe("MiddlewareApplicationModule");
        }
    }
}
