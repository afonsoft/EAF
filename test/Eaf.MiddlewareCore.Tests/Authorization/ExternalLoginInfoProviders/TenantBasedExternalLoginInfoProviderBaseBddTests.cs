using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.ExternalLoginInfoProviders
{
    /// <summary>
    /// Testes BDD para TenantBasedExternalLoginInfoProviderBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TenantBasedExternalLoginInfoProviderBaseBddTests
    {
        private readonly IAbpSession _session;
        private readonly ICacheManager _cacheManager;

        public TenantBasedExternalLoginInfoProviderBaseBddTests()
        {
            _session = Substitute.For<IAbpSession>();
            _cacheManager = Substitute.For<ICacheManager>();
        }

        private sealed class TestableTenantBasedProvider : TenantBasedExternalLoginInfoProviderBase
        {
            private readonly bool _tenantHasSettings;
            private readonly ExternalLoginProviderInfo _hostInfo;
            private readonly ExternalLoginProviderInfo _tenantInfo;

            public TestableTenantBasedProvider(
                IAbpSession session,
                ICacheManager cacheManager,
                bool tenantHasSettings = false,
                ExternalLoginProviderInfo hostInfo = null,
                ExternalLoginProviderInfo tenantInfo = null)
                : base(session, cacheManager)
            {
                _tenantHasSettings = tenantHasSettings;
                _hostInfo = hostInfo ?? new ExternalLoginProviderInfo("TestHost", "hid", "hsec", null, typeof(TestableTenantBasedProvider));
                _tenantInfo = tenantInfo ?? new ExternalLoginProviderInfo("TestTenant", "tid", "tsec", null, typeof(TestableTenantBasedProvider));
            }

            public override string Name => "TestProvider";

            protected override ExternalLoginProviderInfo GetHostInformation() => _hostInfo;

            protected override ExternalLoginProviderInfo GetTenantInformation() => _tenantInfo;

            protected override bool TenantHasSettings() => _tenantHasSettings;
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new TestableTenantBasedProvider(_session, _cacheManager);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_NameDeveSerTestProvider()
        {
            var sut = new TestableTenantBasedProvider(_session, _cacheManager);
            sut.Name.ShouldBe("TestProvider");
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveImplementarIExternalLoginInfoProvider()
        {
            var sut = new TestableTenantBasedProvider(_session, _cacheManager);
            sut.ShouldBeAssignableTo<IExternalLoginInfoProvider>();
        }

        #endregion

        #region GetExternalLoginInfo - Sem Tenant

        [Fact]
        public void Dado_SemTenantId_Quando_GetExternalLoginInfo_Entao_DeveChamarGetCacheParaHost()
        {
            // Dado
            _session.TenantId.Returns((int?)null);
            var cache = Substitute.For<ICache>();
            _cacheManager.GetCache("AppExternalLoginInfoProvidersCache").Returns(cache);

            var hostInfo = new ExternalLoginProviderInfo("HostProvider", "hid", "hsec", null, typeof(TestableTenantBasedProvider));
            var sut = new TestableTenantBasedProvider(_session, _cacheManager, hostInfo: hostInfo);

            // Quando
            var result = sut.GetExternalLoginInfo();

            // Entao
            _cacheManager.Received().GetCache("AppExternalLoginInfoProvidersCache");
        }

        #endregion

        #region GetExternalLoginInfo - Com Tenant sem settings

        [Fact]
        public void Dado_ComTenantIdSemSettings_Quando_GetExternalLoginInfo_Entao_DeveChamarGetCacheParaHost()
        {
            // Dado
            _session.TenantId.Returns(1);
            var cache = Substitute.For<ICache>();
            _cacheManager.GetCache("AppExternalLoginInfoProvidersCache").Returns(cache);

            var sut = new TestableTenantBasedProvider(_session, _cacheManager, tenantHasSettings: false);

            // Quando
            var result = sut.GetExternalLoginInfo();

            // Entao
            _cacheManager.Received().GetCache("AppExternalLoginInfoProvidersCache");
        }

        #endregion

        #region GetExternalLoginInfo - Com Tenant com settings

        [Fact]
        public void Dado_ComTenantIdComSettings_Quando_GetExternalLoginInfo_Entao_DeveChamarGetCacheParaTenant()
        {
            // Dado
            _session.TenantId.Returns(5);
            var cache = Substitute.For<ICache>();
            _cacheManager.GetCache("AppExternalLoginInfoProvidersCache").Returns(cache);

            var sut = new TestableTenantBasedProvider(_session, _cacheManager, tenantHasSettings: true);

            // Quando
            var result = sut.GetExternalLoginInfo();

            // Entao
            _cacheManager.Received().GetCache("AppExternalLoginInfoProvidersCache");
        }

        #endregion
    }
}
