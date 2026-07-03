using Eaf.Middleware.Web.Authentication.JwtBearer;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Authentication.JwtBearer
{
    public class JwtTokenMiddlewareBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(JwtTokenMiddleware).IsAbstract.ShouldBeTrue();
            typeof(JwtTokenMiddleware).IsSealed.ShouldBeTrue();
        }
    }
}
