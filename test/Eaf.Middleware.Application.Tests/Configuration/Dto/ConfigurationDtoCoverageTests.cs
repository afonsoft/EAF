using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration.Dto
{
    public class ConfigurationDtoCoverageTests
    {
        [Fact]
        public void SettingsDto_ShouldSet()
        {
            var dto = new SettingsDto { Name = "n", Value = "v" };
            dto.Name.ShouldBe("n");
            dto.Value.ShouldBe("v");
        }

        [Fact]
        public void SettingsInputDto_ShouldSet()
        {
            var dto = new SettingsInputDto();
            dto.Filter.ShouldBeNull();
            dto.Filter = "abc";
            dto.Filter.ShouldBe("abc");
        }

        [Fact]
        public void ThemeHeaderSettingsDto_ShouldSet()
        {
            var dto = new ThemeHeaderSettingsDto
            {
                DesktopFixedHeader = true,
                HeaderSkin = "dark",
                MobileFixedHeader = true
            };
            dto.DesktopFixedHeader.ShouldBeTrue();
            dto.HeaderSkin.ShouldBe("dark");
            dto.MobileFixedHeader.ShouldBeTrue();
        }

        [Fact]
        public void ThemeLayoutSettingsDto_ShouldSet()
        {
            var dto = new ThemeLayoutSettingsDto
            {
                ContentSkin = "light",
                LayoutType = "fluid",
                ThemeColor = "blue"
            };
            dto.ContentSkin.ShouldBe("light");
            dto.LayoutType.ShouldBe("fluid");
            dto.ThemeColor.ShouldBe("blue");
        }

        [Fact]
        public void ThemeMenuSettingsDto_ShouldSet()
        {
            var dto = new ThemeMenuSettingsDto
            {
                AllowAsideHiding = true,
                AllowAsideMinimizing = true,
                AsideSkin = "dark",
                DefaultHiddenAside = false,
                DefaultMinimizedAside = true,
                FixedAside = true,
                Position = "left",
                SubmenuToggle = "click"
            };
            dto.AllowAsideHiding.ShouldBeTrue();
            dto.AllowAsideMinimizing.ShouldBeTrue();
            dto.AsideSkin.ShouldBe("dark");
            dto.DefaultHiddenAside.ShouldBeFalse();
            dto.DefaultMinimizedAside.ShouldBeTrue();
            dto.FixedAside.ShouldBeTrue();
            dto.Position.ShouldBe("left");
            dto.SubmenuToggle.ShouldBe("click");
        }

        [Fact]
        public void ThemeSettingsDto_Defaults_AreInitialized()
        {
            var dto = new ThemeSettingsDto();
            dto.Header.ShouldNotBeNull();
            dto.Layout.ShouldNotBeNull();
            dto.Menu.ShouldNotBeNull();
            dto.Theme.ShouldBeNull();
        }

        [Fact]
        public void ThemeSettingsDto_ShouldSet()
        {
            var dto = new ThemeSettingsDto
            {
                Header = new ThemeHeaderSettingsDto(),
                Layout = new ThemeLayoutSettingsDto(),
                Menu = new ThemeMenuSettingsDto(),
                Theme = "default"
            };
            dto.Header.ShouldNotBeNull();
            dto.Layout.ShouldNotBeNull();
            dto.Menu.ShouldNotBeNull();
            dto.Theme.ShouldBe("default");
        }
    }
}
