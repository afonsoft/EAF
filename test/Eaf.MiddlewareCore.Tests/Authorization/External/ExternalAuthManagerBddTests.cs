using Abp.Dependency;
using Eaf.Middleware.Core.Authentication.External;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para ExternalAuthManager exercitando caminhos reais de provedores externos.
    /// </summary>
    public class ExternalAuthManagerBddTests
    {
        private const string ProviderName = "EAFProvider";

        private ExternalAuthManager CreateSut(IExternalAuthProviderApi providerApi = null)
        {
            var iocResolver = Substitute.For<IIocResolver>();
            var externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();

            var providerType = typeof(FakeExternalAuthProviderApi);
            var providerInfo = new ExternalLoginProviderInfo(ProviderName, "client", "secret", "1", providerType);

            var provider = Substitute.For<IExternalLoginInfoProvider>();
            provider.Name.Returns(ProviderName);
            provider.GetExternalLoginInfo().Returns(providerInfo);

            externalAuthConfiguration.ExternalLoginInfoProviders
                .Returns(new List<IExternalLoginInfoProvider> { provider });

            if (providerApi != null)
            {
                iocResolver.Resolve(providerType).Returns(providerApi);
            }
            else
            {
                iocResolver.Resolve(providerType).Returns(new FakeExternalAuthProviderApi());
            }

            return new ExternalAuthManager(iocResolver, externalAuthConfiguration);
        }

        private IExternalAuthProviderApi CreateProviderApi()
        {
            var api = Substitute.For<IExternalAuthProviderApi>();
            api.GetUserInfo("access-code").Returns(new ExternalAuthUserInfo
            {
                Provider = ProviderName,
                ProviderKey = "provider-key",
                AccessCode = "access-code"
            });
            api.IsValidUser("provider-key", "access-code").Returns(true);
            return api;
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = CreateSut();
            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<IExternalAuthManager>();
        }

        #endregion

        #region CreateProviderApi

        [Fact]
        public void Dado_ProviderDesconhecido_Quando_CreateProviderApi_Entao_DeveLancarArgumentNullException()
        {
            // Dado
            var externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();
            externalAuthConfiguration.ExternalLoginInfoProviders.Returns(new List<IExternalLoginInfoProvider>());
            var sut = new ExternalAuthManager(Substitute.For<IIocResolver>(), externalAuthConfiguration);

            // Quando/Entao
            Should.Throw<ArgumentNullException>(() => sut.CreateProviderApi("UnknownProvider"));
        }

        [Fact]
        public void Dado_ProviderConfigurado_Quando_CreateProviderApi_Entao_DeveRetornarApiInicializada()
        {
            // Dado
            var sut = CreateSut();

            // Quando
            using var api = sut.CreateProviderApi(ProviderName);

            // Então
            api.ShouldNotBeNull();
            api.Object.ShouldNotBeNull();
        }

        #endregion

        #region GetUserInfo

        [Fact]
        public async Task Dado_ProviderConfigurado_Quando_GetUserInfo_Entao_DeveRetornarUsuarioExterno()
        {
            // Dado
            var providerApi = CreateProviderApi();
            var sut = CreateSut(providerApi);

            // Quando
            var result = await sut.GetUserInfo(ProviderName, "access-code");

            // Então
            result.ShouldNotBeNull();
            result.ProviderKey.ShouldBe("provider-key");
        }

        #endregion

        #region IsValidUser

        [Fact]
        public async Task Dado_UsuarioValidoNoProvider_Quando_IsValidUser_Entao_DeveRetornarVerdadeiro()
        {
            // Dado
            var providerApi = CreateProviderApi();
            var sut = CreateSut(providerApi);

            // Quando
            var result = await sut.IsValidUser(ProviderName, "provider-key", "access-code");

            // Então
            result.ShouldBeTrue();
        }

        #endregion

        private class FakeExternalAuthProviderApi : IExternalAuthProviderApi
        {
            public ExternalLoginProviderInfo ProviderInfo { get; set; }

            public Task<ExternalAuthUserInfo> GetUserInfo(string accessCode) => Task.FromResult(new ExternalAuthUserInfo());

            public void Initialize(ExternalLoginProviderInfo providerInfo) => ProviderInfo = providerInfo;

            public Task<bool> IsValidUser(string userId, string accessCode) => Task.FromResult(false);
        }
    }
}
