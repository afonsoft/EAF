using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class ThemeMenuSettingsDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAllowAsideMinimizing_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.AllowAsideMinimizing = true;
            sut.AllowAsideMinimizing.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAsideSkin_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.AsideSkin = "test_value";
            sut.AsideSkin.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDefaultHiddenAside_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.DefaultHiddenAside = true;
            sut.DefaultHiddenAside.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDefaultMinimizedAside_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.DefaultMinimizedAside = true;
            sut.DefaultMinimizedAside.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFixedAside_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.FixedAside = true;
            sut.FixedAside.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPosition_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.Position = "test_value";
            sut.Position.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSubmenuToggle_Entao_DeveArmazenar()
        {
            var sut = new ThemeMenuSettingsDto();
            sut.SubmenuToggle = "test_value";
            sut.SubmenuToggle.ShouldBe("test_value");
        }
    }
}
