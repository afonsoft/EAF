using Abp.MultiTenancy;
using Eaf.Middleware.Url;
using Eaf.Middleware.Web.Url;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para AngularAppUrlService seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AngularAppUrlServiceBddTests
    {
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantCache _tenantCache;
        private readonly AngularAppUrlService _sut;

        public AngularAppUrlServiceBddTests()
        {
            _webUrlService = Substitute.For<IWebUrlService>();
            _tenantCache = Substitute.For<ITenantCache>();
            _sut = new AngularAppUrlService(_webUrlService, _tenantCache);
        }

        #region Rotas

        [Fact]
        public void Dado_AngularAppUrlService_Quando_VerificarEmailActivationRoute_Entao_DeveSerCorreto()
        {
            _sut.EmailActivationRoute.ShouldBe("account/confirm-email");
        }

        [Fact]
        public void Dado_AngularAppUrlService_Quando_VerificarPasswordResetRoute_Entao_DeveSerCorreto()
        {
            _sut.PasswordResetRoute.ShouldBe("account/reset-password");
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion
    }
}
