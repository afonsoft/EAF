using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Eaf.Middleware;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Core.Tests.Identity;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
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

        [Fact]
        public void Dado_TokenValidoComUsuarioCadastrado_Quando_ValidateToken_Entao_DeveRetornarPrincipal()
        {
            // Dado
            var tokenKey = "token-key-1";
            var securityStamp = "stamp-123";
            var user = IdentityTestHelper.CreateUser(securityStamp: securityStamp);
            user.Tokens.Add(IdentityTestHelper.CreateTokenValidityKeyToken(user, tokenKey));

            var userManager = IdentityTestHelper.CreateUserManager(user);
            var unitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            var settingManager = CriarSettingManager();
            var cacheManager = CriarCacheManager();

            IdentityTestHelper.RegisterJwtDependencies(userManager, unitOfWorkManager, settingManager, cacheManager);

            var token = CriarTokenJwtValido(user, tokenKey, securityStamp);
            var validationParameters = CriarValidationParameters();
            var handler = new MiddlewareJwtSecurityTokenHandler();

            // Quando
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            // Então
            principal.ShouldNotBeNull();
            validatedToken.ShouldNotBeNull();
            principal.FindFirst(MiddlewareCoreConsts.UserIdentifier)?.Value.ShouldBe("1@1");
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

        private static string CriarTokenJwtValido(User user, string tokenKey, string securityStamp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: new[]
                {
                    new Claim(MiddlewareCoreConsts.UserIdentifier, $"{user.Id}@{user.TenantId}"),
                    new Claim(MiddlewareCoreConsts.TokenValidityKey, tokenKey),
                    new Claim(MiddlewareCoreConsts.TokenValidityValue, securityStamp)
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: null);

            return tokenHandler.WriteToken(token);
        }

        private static TokenValidationParameters CriarValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                RequireSignedTokens = false,
                ValidateLifetime = false
            };
        }

        private static ISettingManager CriarSettingManager()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return "1";
                return "false";
            });
            return settingManager;
        }

        private static ICacheManager CriarCacheManager()
        {
            var cache = Substitute.For<ICache>();
            cache.GetOrDefault(Arg.Any<string>()).Returns(null);

            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache(Arg.Any<string>()).Returns(cache);
            return cacheManager;
        }
    }
}
