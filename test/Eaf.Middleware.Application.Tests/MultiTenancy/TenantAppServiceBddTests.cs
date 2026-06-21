using Abp.Events.Bus;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Url;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantAppServiceBddTests
    {
        private readonly TenantAppService _sut;

        public TenantAppServiceBddTests()
        {
            _sut = new TenantAppService();
        }

        #region Construtor

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_DeveInicializarPadroes()
        {
            var sut = new TenantAppService();
            sut.ShouldNotBeNull();
            sut.AppUrlService.ShouldNotBeNull();
            sut.EventBus.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_AppUrlServiceDeveSerNullInstance()
        {
            var sut = new TenantAppService();
            sut.AppUrlService.ShouldBe(NullAppUrlService.Instance);
        }

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_EventBusDeveSerNullInstance()
        {
            var sut = new TenantAppService();
            sut.EventBus.ShouldBe(NullEventBus.Instance);
        }

        #endregion

        #region Injecao de Propriedade

        [Fact]
        public void Dado_AppUrlServiceCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customService = Substitute.For<IAppUrlService>();
            _sut.AppUrlService = customService;
            _sut.AppUrlService.ShouldBe(customService);
        }

        [Fact]
        public void Dado_EventBusCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customEventBus = Substitute.For<IEventBus>();
            _sut.EventBus = customEventBus;
            _sut.EventBus.ShouldBe(customEventBus);
        }

        #endregion
    }
}
