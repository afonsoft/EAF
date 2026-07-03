using Eaf.Middleware.Web.Authentication.JwtBearer;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Authentication.JwtBearer
{
    public class MiddlewareJwtSecurityTokenHandlerBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new MiddlewareJwtSecurityTokenHandler();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarCanValidateToken_Entao_DeveSerTrue()
        {
            var sut = new MiddlewareJwtSecurityTokenHandler();
            sut.CanValidateToken.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarMaximumTokenSizeInBytes_Entao_DeveSerPadrao()
        {
            var sut = new MiddlewareJwtSecurityTokenHandler();
            sut.MaximumTokenSizeInBytes.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_Instancia_Quando_CanReadTokenComTokenInvalido_Entao_DeveRetornarFalse()
        {
            var sut = new MiddlewareJwtSecurityTokenHandler();
            sut.CanReadToken("invalid_token").ShouldBeFalse();
        }
    }
}
