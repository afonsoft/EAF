using Castle.Core.Logging;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.OpenIdConnect;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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
