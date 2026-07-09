using Abp.Configuration;
using Eaf.Middleware.Configuration;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para GoogleAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class GoogleAppServiceBddTests
    {
        private readonly GoogleAppService _sut;

        public GoogleAppServiceBddTests()
        {
            _sut = new GoogleAppService();
        }

        #region Construtor

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            // Dado / Quando
            var sut = new GoogleAppService();

            // Então
            sut.ShouldNotBeNull();
        }

        #endregion

        #region Configuracao

        private static GoogleAppService CriarServicoComSettingManager(string valor)
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(valor);
            return new GoogleAppService { SettingManager = settingManager };
        }

        #endregion

        #region GetAnalytics

        [Fact]
        public async Task Dado_SettingManagerConfigurado_Quando_GetAnalytics_Entao_DeveRetornarValorDoSetting()
        {
            // Dado
            var sut = CriarServicoComSettingManager("UA-123456");

            // Quando
            var result = await sut.GetAnalytics();

            // Então
            result.ShouldBe("UA-123456");
        }

        #endregion

        #region GetRecaptchaSiteKey

        [Fact]
        public async Task Dado_SettingManagerConfigurado_Quando_GetRecaptchaSiteKey_Entao_DeveRetornarValorDoSetting()
        {
            // Dado
            var sut = CriarServicoComSettingManager("site-key-abc");

            // Quando
            var result = await sut.GetRecaptchaSiteKey();

            // Então
            result.ShouldBe("site-key-abc");
        }

        #endregion

        #region GetTagManager

        [Fact]
        public async Task Dado_SettingManagerConfigurado_Quando_GetTagManager_Entao_DeveRetornarValorDoSetting()
        {
            // Dado
            var sut = CriarServicoComSettingManager("GTM-XYZ");

            // Quando
            var result = await sut.GetTagManager();

            // Então
            result.ShouldBe("GTM-XYZ");
        }

        #endregion
    }
}
