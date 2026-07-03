using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class ThemeLayoutSettingsDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ThemeLayoutSettingsDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirLayoutType_Entao_DeveArmazenar()
        {
            var sut = new ThemeLayoutSettingsDto();
            sut.LayoutType = "test_value";
            sut.LayoutType.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirThemeColor_Entao_DeveArmazenar()
        {
            var sut = new ThemeLayoutSettingsDto();
            sut.ThemeColor = "test_value";
            sut.ThemeColor.ShouldBe("test_value");
        }
    }
}
