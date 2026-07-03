using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class ThemeHeaderSettingsDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ThemeHeaderSettingsDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirHeaderSkin_Entao_DeveArmazenar()
        {
            var sut = new ThemeHeaderSettingsDto();
            sut.HeaderSkin = "test_value";
            sut.HeaderSkin.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMobileFixedHeader_Entao_DeveArmazenar()
        {
            var sut = new ThemeHeaderSettingsDto();
            sut.MobileFixedHeader = true;
            sut.MobileFixedHeader.ShouldBe(true);
        }
    }
}
