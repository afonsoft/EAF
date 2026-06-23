using Abp.MultiTenancy;
using Eaf.Middleware.Url;
using Eaf.Middleware.Web.Url;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Url
{
    /// <summary>
    /// Testes BDD para AppUrlServiceBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AppUrlServiceBaseBddTests
    {
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantCache _tenantCache;

        public AppUrlServiceBaseBddTests()
        {
            _webUrlService = Substitute.For<IWebUrlService>();
            _tenantCache = Substitute.For<ITenantCache>();
        }

        private sealed class TestableAppUrlService : AppUrlServiceBase
        {
            public TestableAppUrlService(IWebUrlService webUrlService, ITenantCache tenantCache)
                : base(webUrlService, tenantCache)
            {
            }

            public override string EmailActivationRoute => "account/email-activation";
            public override string PasswordResetRoute => "account/reset-password";
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveImplementarIAppUrlService()
        {
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);
            sut.ShouldBeAssignableTo<IAppUrlService>();
        }

        #endregion

        #region CreateEmailActivationUrlFormat

        [Fact]
        public void Dado_TenancyNameNull_Quando_CreateEmailActivationUrlFormat_Entao_DeveRetornarUrlSemTenantId()
        {
            // Dado
            _webUrlService.GetSiteRootAddress(null).Returns("https://app.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreateEmailActivationUrlFormat((string)null);

            // Entao
            result.ShouldContain("account/email-activation");
            result.ShouldContain("userId={userId}");
            result.ShouldContain("confirmationCode={confirmationCode}");
            result.ShouldNotContain("tenantId={tenantId}");
            result.ShouldContain("authenticationSource={authenticationSource}");
        }

        [Fact]
        public void Dado_TenancyNamePreenchido_Quando_CreateEmailActivationUrlFormat_Entao_DeveRetornarUrlComTenantId()
        {
            // Dado
            _webUrlService.GetSiteRootAddress("acme").Returns("https://acme.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreateEmailActivationUrlFormat("acme");

            // Entao
            result.ShouldContain("tenantId={tenantId}");
            result.ShouldContain("account/email-activation");
        }

        [Fact]
        public void Dado_TenantIdComValor_Quando_CreateEmailActivationUrlFormat_Entao_DeveResolverTenancyName()
        {
            // Dado
            var tenantCacheItem = new TenantCacheItem { TenancyName = "resolved-tenant" };
            _tenantCache.Get(1).Returns(tenantCacheItem);
            _webUrlService.GetSiteRootAddress("resolved-tenant").Returns("https://resolved-tenant.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreateEmailActivationUrlFormat((int?)1);

            // Entao
            result.ShouldContain("account/email-activation");
            result.ShouldContain("tenantId={tenantId}");
        }

        [Fact]
        public void Dado_TenantIdNull_Quando_CreateEmailActivationUrlFormat_Entao_DeveUsarNullComoTenancyName()
        {
            // Dado
            _webUrlService.GetSiteRootAddress(null).Returns("https://app.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreateEmailActivationUrlFormat((int?)null);

            // Entao
            result.ShouldNotContain("tenantId={tenantId}");
        }

        #endregion

        #region CreatePasswordResetUrlFormat

        [Fact]
        public void Dado_TenancyNameNull_Quando_CreatePasswordResetUrlFormat_Entao_DeveRetornarUrlSemTenantId()
        {
            // Dado
            _webUrlService.GetSiteRootAddress(null).Returns("https://app.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreatePasswordResetUrlFormat((string)null);

            // Entao
            result.ShouldContain("account/reset-password");
            result.ShouldContain("userId={userId}");
            result.ShouldContain("resetCode={resetCode}");
            result.ShouldNotContain("tenantId={tenantId}");
        }

        [Fact]
        public void Dado_TenancyNamePreenchido_Quando_CreatePasswordResetUrlFormat_Entao_DeveRetornarUrlComTenantId()
        {
            // Dado
            _webUrlService.GetSiteRootAddress("acme").Returns("https://acme.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreatePasswordResetUrlFormat("acme");

            // Entao
            result.ShouldContain("tenantId={tenantId}");
            result.ShouldContain("account/reset-password");
        }

        [Fact]
        public void Dado_TenantIdComValor_Quando_CreatePasswordResetUrlFormat_Entao_DeveResolverTenancyName()
        {
            // Dado
            var tenantCacheItem = new TenantCacheItem { TenancyName = "my-tenant" };
            _tenantCache.Get(5).Returns(tenantCacheItem);
            _webUrlService.GetSiteRootAddress("my-tenant").Returns("https://my-tenant.eaf.com/");
            var sut = new TestableAppUrlService(_webUrlService, _tenantCache);

            // Quando
            var result = sut.CreatePasswordResetUrlFormat((int?)5);

            // Entao
            result.ShouldContain("tenantId={tenantId}");
        }

        #endregion
    }
}
