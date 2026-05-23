using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.UiCustomization.Dto
{
    public class UiCustomizationDtoCoverageTests
    {
        [Fact]
        public void Defaults()
        {
            var dto = new UiCustomizationSettingsDto();
            dto.AllowMenuScroll.ShouldBeTrue();
            dto.BaseSettings.ShouldBeNull();
            dto.IsLeftMenuUsed.ShouldBeFalse();
            dto.IsTabMenuUsed.ShouldBeFalse();
            dto.IsTopMenuUsed.ShouldBeFalse();
        }

        [Fact]
        public void ShouldSetAll()
        {
            var dto = new UiCustomizationSettingsDto
            {
                AllowMenuScroll = false,
                BaseSettings = new ThemeSettingsDto(),
                IsLeftMenuUsed = true,
                IsTabMenuUsed = true,
                IsTopMenuUsed = true
            };
            dto.AllowMenuScroll.ShouldBeFalse();
            dto.BaseSettings.ShouldNotBeNull();
            dto.IsLeftMenuUsed.ShouldBeTrue();
            dto.IsTabMenuUsed.ShouldBeTrue();
            dto.IsTopMenuUsed.ShouldBeTrue();
        }
    }
}
