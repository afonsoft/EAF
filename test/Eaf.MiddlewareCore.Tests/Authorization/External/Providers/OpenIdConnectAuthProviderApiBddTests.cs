using Castle.Core.Logging;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Abp;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para OpenIdConnectAuthProviderApi seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class OpenIdConnectAuthProviderApiBddTests
    {
        [Fact]
        public void Dado_Constructor_Quando_CriarInstancia_Entao_LoggerDeveSerAtribuido()
        {
            var logger = Substitute.For<ILogger>();
            var sut = new OpenIdConnectAuthProviderApi(logger);
            sut.Logger.ShouldBe(logger);
        }

        [Fact]
        public async Task Dado_ProviderInfoSemAuthority_Quando_GetUserInfo_Entao_DeveLancarExcecao()
        {
            var sut = CriarSut();
            await Should.ThrowAsync<Exception>(async () => await sut.GetUserInfo("any-token"));
        }

        [Fact]
        public async Task Dado_ProviderInfoComAuthorityVazia_Quando_GetUserInfo_Entao_DeveLancarApplicationException()
        {
            var sut = CriarSut(new Dictionary<string, string> { ["Authority"] = "" });
            await Should.ThrowAsync<ApplicationException>(async () => await sut.GetUserInfo("any-token"));
        }

        [Fact]
        public async Task Dado_ProviderInfoComAuthorityValida_Quando_GetUserInfoComTokenNulo_Entao_DeveLancarArgumentNullException()
        {
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });
            await Should.ThrowAsync<ArgumentNullException>(async () => await sut.GetUserInfo(null));
        }

        [Fact]
        public async Task Dado_ProviderInfoComAuthorityValida_Quando_GetUserInfoComTokenVazio_Entao_DeveLancarArgumentNullException()
        {
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });
            await Should.ThrowAsync<ArgumentNullException>(async () => await sut.GetUserInfo(""));
        }

        [Fact]
        public async Task Dado_ProviderInfoComAuthorityValida_Quando_GetUserInfoComTokenInvalido_Entao_DeveLancarExcecaoDeToken()
        {
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });
            await Should.ThrowAsync<Exception>(async () => await sut.GetUserInfo("invalid-token"));
        }

        [Fact]
        public async Task Dado_ProviderInfoComAuthorityValida_Quando_GetUserInfoComTokenJwtInvalido_Entao_DeveLancarExcecaoDeFormato()
        {
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });
            await Should.ThrowAsync<Exception>(async () => await sut.GetUserInfo("invalid.jwt.token"));
        }

        [Fact]
        public async Task Dado_ConfigurationManagerComSigningKeysVazias_Quando_ValidateTokenInternal_Entao_DeveLancarExcecaoDeValidacao()
        {
            // Dado
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });

            var config = new OpenIdConnectConfiguration();
            config.SigningKeys.Add(new SymmetricSecurityKey(new byte[32]));
            var configurationManager = Substitute.For<IConfigurationManager<OpenIdConnectConfiguration>>();
            configurationManager.GetConfigurationAsync(Arg.Any<CancellationToken>()).Returns(config);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando & Então
            await Should.ThrowAsync<Exception>(async () => await ((Task)method.Invoke(sut, new object[] { "invalid-token", "https://localhost", configurationManager, default(CancellationToken) })));
        }

        [Fact]
        public async Task Dado_TokenJwtSemAudClaim_Quando_GetUserInfo_Entao_DeveLancarAbpException()
        {
            // Dado
            var key = new SymmetricSecurityKey(new byte[32]);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost",
                SigningCredentials = signingCredentials,
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("name", "Test User"),
                    new System.Security.Claims.Claim("unique_name", "test@example.com")
                })
            };
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });

            var config = new OpenIdConnectConfiguration();
            config.SigningKeys.Add(key);
            var configurationManager = Substitute.For<IConfigurationManager<OpenIdConnectConfiguration>>();
            configurationManager.GetConfigurationAsync(Arg.Any<CancellationToken>()).Returns(config);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando & Então
            await Should.ThrowAsync<Abp.AbpException>(async () => await ((Task)method.Invoke(sut, new object[] { token, "https://localhost", configurationManager, default(CancellationToken) })));
        }

        [Fact]
        public async Task Dado_ValidateIssuerInvalido_Quando_ValidateTokenInternal_Entao_DeveLancarFormatException()
        {
            // Dado
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "invalid"
            });

            var config = new OpenIdConnectConfiguration();
            config.SigningKeys.Add(new SymmetricSecurityKey(new byte[32]));
            var configurationManager = Substitute.For<IConfigurationManager<OpenIdConnectConfiguration>>();
            configurationManager.GetConfigurationAsync(Arg.Any<CancellationToken>()).Returns(config);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando & Então
            await Should.ThrowAsync<FormatException>(async () => await ((Task)method.Invoke(sut, new object[] { "any-token", "https://localhost", configurationManager, default(CancellationToken) })));
        }

        private static OpenIdConnectAuthProviderApi CriarSut(Dictionary<string, string> additionalParams = null)
        {
            var sut = new OpenIdConnectAuthProviderApi(NullLogger.Instance);
            var providerInfo = new ExternalLoginProviderInfo(
                name: OpenIdConnectAuthProviderApi.Name,
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(OpenIdConnectAuthProviderApi),
                additionalParams: additionalParams,
                claimMappings: new List<JsonClaimMap>());
            sut.Initialize(providerInfo);
            return sut;
        }
    }
}
