using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Configuration seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ConfigurationDtoBddTests
    {
        #region SettingsDto

        [Fact]
        public void Dado_SettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new SettingsDto { Name = "App.Theme", Value = "dark" };
            dto.Name.ShouldBe("App.Theme");
            dto.Value.ShouldBe("dark");
        }

        #endregion

        #region SettingsInputDto

        [Fact]
        public void Dado_SettingsInputDto_Quando_CriarPadrao_Entao_FilterDeveSerNull()
        {
            var dto = new SettingsInputDto();
            dto.Filter.ShouldBeNull();
        }

        [Fact]
        public void Dado_SettingsInputDto_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var dto = new SettingsInputDto { Filter = "smtp" };
            dto.Filter.ShouldBe("smtp");
        }

        #endregion

        #region ThemeHeaderSettingsDto

        [Fact]
        public void Dado_ThemeHeaderSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeHeaderSettingsDto
            {
                DesktopFixedHeader = true,
                MobileFixedHeader = false,
                HeaderSkin = "dark"
            };

            dto.DesktopFixedHeader.ShouldBeTrue();
            dto.MobileFixedHeader.ShouldBeFalse();
            dto.HeaderSkin.ShouldBe("dark");
        }

        #endregion

        #region ThemeLayoutSettingsDto

        [Fact]
        public void Dado_ThemeLayoutSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeLayoutSettingsDto
            {
                LayoutType = "fluid",
                ContentSkin = "light",
                ThemeColor = "#3699FF"
            };

            dto.LayoutType.ShouldBe("fluid");
            dto.ContentSkin.ShouldBe("light");
            dto.ThemeColor.ShouldBe("#3699FF");
        }

        #endregion

        #region ThemeMenuSettingsDto

        [Fact]
        public void Dado_ThemeMenuSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ThemeMenuSettingsDto
            {
                FixedAside = true,
                AllowAsideMinimizing = true,
                DefaultMinimizedAside = false,
                AllowAsideHiding = true,
                DefaultHiddenAside = false,
                Position = "left",
                SubmenuToggle = "hover",
                AsideSkin = "dark"
            };

            dto.FixedAside.ShouldBeTrue();
            dto.AllowAsideMinimizing.ShouldBeTrue();
            dto.DefaultMinimizedAside.ShouldBeFalse();
            dto.AllowAsideHiding.ShouldBeTrue();
            dto.DefaultHiddenAside.ShouldBeFalse();
            dto.Position.ShouldBe("left");
            dto.SubmenuToggle.ShouldBe("hover");
            dto.AsideSkin.ShouldBe("dark");
        }

        #endregion
    }
}
