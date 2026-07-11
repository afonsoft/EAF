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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        [Fact]
        public async Task Dado_TokenValido_Quando_ValidateTokenInternal_Entao_DeveRetornarPrincipalEIdentity()
        {
            // Dado
            var key = new SymmetricSecurityKey(new byte[32]);
            var token = CriarTokenJwtValido("https://localhost", "client-id", "Test User", "test@example.com", DateTime.UtcNow.AddHours(1));
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "true"
            });

            var configurationManager = CriarConfigurationManagerSubstitute(key);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var task = (Task)method.Invoke(sut, new object[] { token, "https://localhost", configurationManager, default(CancellationToken) })!;
            await task;

            // Então
            var validated = task.GetType().GetProperty("Result")!.GetValue(task);
            var validatedType = validated.GetType();
            validatedType.GetProperty("Principal")!.GetValue(validated).ShouldNotBeNull();
            validatedType.GetProperty("Token")!.GetValue(validated).ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_TokenValido_Quando_ValidateToken_Entao_DeveRetornarValidateTokenResult()
        {
            // Dado
            var key = new SymmetricSecurityKey(new byte[32]);
            var token = CriarTokenJwtValido("https://localhost", "client-id", "Test User", "test@example.com", DateTime.UtcNow.AddHours(1));
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });

            var configurationManager = CriarConfigurationManagerSubstitute(key);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateToken", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var task = (Task)method.Invoke(sut, new object[] { token, "https://localhost", configurationManager })!;
            await task;

            // Então
            var validated = task.GetType().GetProperty("Result")!.GetValue(task);
            var validatedType = validated.GetType();
            validatedType.GetProperty("Principal")!.GetValue(validated).ShouldNotBeNull();
            validatedType.GetProperty("Token")!.GetValue(validated).ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_TokenJwtComAudIncorreto_Quando_ValidateTokenInternal_Entao_DeveLancarAbpException()
        {
            // Dado
            var key = new SymmetricSecurityKey(new byte[32]);
            var token = CriarTokenJwtValido("https://localhost", "wrong-client", "Test User", "test@example.com", DateTime.UtcNow.AddHours(1));
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });

            var configurationManager = CriarConfigurationManagerSubstitute(key);

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando & Então
            await Should.ThrowAsync<AbpException>(async () => await ((Task)method.Invoke(sut, new object[] { token, "https://localhost", configurationManager, default(CancellationToken) })));
        }

        [Fact]
        public async Task Dado_IssuerNulo_Quando_ValidateToken_Entao_DeveLancarArgumentNullException()
        {
            var sut = CriarSut(new Dictionary<string, string>
            {
                ["Authority"] = "https://localhost",
                ["ValidateIssuer"] = "false"
            });

            var method = typeof(OpenIdConnectAuthProviderApi).GetMethod("ValidateToken", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var ex = await Should.ThrowAsync<TargetInvocationException>(async () => await ((Task)method.Invoke(sut, new object[] { "token", null, Substitute.For<IConfigurationManager<OpenIdConnectConfiguration>>() })));
            ex.InnerException.ShouldBeOfType<ArgumentNullException>();
        }

        private static IConfigurationManager<OpenIdConnectConfiguration> CriarConfigurationManagerSubstitute(SymmetricSecurityKey key)
        {
            var config = new OpenIdConnectConfiguration();
            config.SigningKeys.Add(key);
            var configurationManager = Substitute.For<IConfigurationManager<OpenIdConnectConfiguration>>();
            configurationManager.GetConfigurationAsync(Arg.Any<CancellationToken>()).Returns(config);
            return configurationManager;
        }

        private static string CriarTokenJwtValido(string issuer, string audience, string name, string uniqueName, DateTime expires)
        {
            var key = new SymmetricSecurityKey(new byte[32]);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Expires = expires,
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("aud", audience),
                    new Claim("name", name),
                    new Claim("unique_name", uniqueName)
                })
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
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
