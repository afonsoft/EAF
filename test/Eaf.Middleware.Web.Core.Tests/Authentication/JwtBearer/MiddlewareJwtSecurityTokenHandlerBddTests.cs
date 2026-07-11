using Eaf.Middleware.Web.Authentication.JwtBearer;
using Shouldly;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Authentication.JwtBearer
{
    public class MiddlewareJwtSecurityTokenHandlerBddTests
    {
        [Fact]
        public void Dado_MiddlewareJwtSecurityTokenHandler_Quando_CriarInstancia_Entao_CanValidateTokenDeveSerVerdadeiro()
        {
            var handler = new MiddlewareJwtSecurityTokenHandler();

            handler.CanValidateToken.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtValido_Quando_CanReadToken_Entao_DeveRetornarVerdadeiro()
        {
            var handler = new MiddlewareJwtSecurityTokenHandler();
            var token = CriarTokenJwtValido();

            var result = handler.CanReadToken(token);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtInvalido_Quando_CanReadToken_Entao_DeveRetornarFalso()
        {
            var handler = new MiddlewareJwtSecurityTokenHandler();

            var result = handler.CanReadToken("not-a-jwt");

            result.ShouldBeFalse();
        }

        [Fact]
        public void Dado_Handler_Quando_VerificarMaximumTokenSizeInBytes_Entao_DeveTerValorPadrao()
        {
            var handler = new MiddlewareJwtSecurityTokenHandler();

            handler.MaximumTokenSizeInBytes.ShouldBeGreaterThan(0);
        }

        private static string CriarTokenJwtValido()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: null);

            return tokenHandler.WriteToken(token);
        }
    }
}
