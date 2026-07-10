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

        [Fact]
        public void Dado_Instancia_Quando_CanReadTokenComJwtValido_Entao_DeveRetornarTrue()
        {
            // Dado
            var sut = new MiddlewareJwtSecurityTokenHandler();
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

            // Quando
            var result = sut.CanReadToken(token);

            // Então
            result.ShouldBeTrue();
        }
    }
}
