using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.UiCustomization.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de UiCustomization seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UiCustomizationDtoBddTests
    {
        [Fact]
        public void Dado_UiCustomizationSettingsDto_Quando_CriarPadrao_Entao_AllowMenuScrollDeveSerTrue()
        {
            var dto = new UiCustomizationSettingsDto();
            dto.AllowMenuScroll.ShouldBeTrue();
        }

        [Fact]
        public void Dado_UiCustomizationSettingsDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UiCustomizationSettingsDto
            {
                IsLeftMenuUsed = true,
                IsTopMenuUsed = false,
                IsTabMenuUsed = true,
                AllowMenuScroll = false,
                BaseSettings = new ThemeSettingsDto { Theme = "theme2" }
            };

            dto.IsLeftMenuUsed.ShouldBeTrue();
            dto.IsTopMenuUsed.ShouldBeFalse();
            dto.IsTabMenuUsed.ShouldBeTrue();
            dto.AllowMenuScroll.ShouldBeFalse();
            dto.BaseSettings.ShouldNotBeNull();
            dto.BaseSettings.Theme.ShouldBe("theme2");
        }
    }
}
