using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de configuração de tema seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ThemeSettingsDtoBddTests
    {
        [Fact]
        public void Dado_ThemeSettingsDto_Quando_Criar_Entao_SubDtosDevemSerInicializados()
        {
            var dto = new ThemeSettingsDto();
            dto.Header.ShouldNotBeNull();
            dto.Layout.ShouldNotBeNull();
            dto.Menu.ShouldNotBeNull();
            dto.Theme.ShouldBeNull();
        }

        [Fact]
        public void Dado_ThemeSettingsDto_Quando_DefinirTheme_Entao_DeveArmazenar()
        {
            var dto = new ThemeSettingsDto { Theme = "default" };
            dto.Theme.ShouldBe("default");
        }

        [Fact]
        public void Dado_ThemeHeaderSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeHeaderSettingsDto
            {
                DesktopFixedHeader = true,
                MobileFixedHeader = false,
                HeaderSkin = "light"
            };

            dto.DesktopFixedHeader.ShouldBeTrue();
            dto.MobileFixedHeader.ShouldBeFalse();
            dto.HeaderSkin.ShouldBe("light");
        }

        [Fact]
        public void Dado_ThemeLayoutSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeLayoutSettingsDto
            {
                LayoutType = "fluid",
                ContentSkin = "light2",
                ThemeColor = "#2196f3"
            };

            dto.LayoutType.ShouldBe("fluid");
            dto.ContentSkin.ShouldBe("light2");
            dto.ThemeColor.ShouldBe("#2196f3");
        }

        [Fact]
        public void Dado_ThemeMenuSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeMenuSettingsDto
            {
                FixedAside = true,
                AllowAsideMinimizing = true,
                AllowAsideHiding = false,
                DefaultMinimizedAside = false,
                DefaultHiddenAside = false,
                Position = "left",
                AsideSkin = "dark",
                SubmenuToggle = "accordion"
            };

            dto.FixedAside.ShouldBeTrue();
            dto.AllowAsideMinimizing.ShouldBeTrue();
            dto.AllowAsideHiding.ShouldBeFalse();
            dto.DefaultMinimizedAside.ShouldBeFalse();
            dto.DefaultHiddenAside.ShouldBeFalse();
            dto.Position.ShouldBe("left");
            dto.AsideSkin.ShouldBe("dark");
            dto.SubmenuToggle.ShouldBe("accordion");
        }
    }
}
