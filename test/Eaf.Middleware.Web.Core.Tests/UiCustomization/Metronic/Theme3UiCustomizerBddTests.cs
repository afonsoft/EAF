using Abp;
using Abp.Configuration;
using Eaf.Middleware;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.Web.UiCustomization.Metronic;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.UiCustomization.Metronic
{
    public class Theme3UiCustomizerBddTests
    {
        private static Theme3UiCustomizer CreateSut()
        {
            return new Theme3UiCustomizer(UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme3));
        }

        private static ThemeSettingsDto CreateSettings()
        {
            return new ThemeSettingsDto
            {
                Theme = MiddlewareAppConsts.Theme3,
                Layout = new ThemeLayoutSettingsDto
                {
                    ThemeColor = MiddlewareAppConsts.Theme3,
                    LayoutType = MiddlewareAppConsts.Theme3,
                    ContentSkin = MiddlewareAppConsts.Theme3
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = true,
                    MobileFixedHeader = true,
                    HeaderSkin = MiddlewareAppConsts.Theme3
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = MiddlewareAppConsts.Theme3,
                    FixedAside = true,
                    AllowAsideMinimizing = true,
                    DefaultMinimizedAside = true,
                    AllowAsideHiding = true,
                    DefaultHiddenAside = true
                }
            };
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeUiThemeCustomizerBase()
        {
            typeof(Theme3UiCustomizer).BaseType.ShouldBe(typeof(UiThemeCustomizerBase));
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_GetHostUiManagementSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetHostUiManagementSettings();

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme3);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_GetTenantUiCustomizationSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetTenantUiCustomizationSettings(42);

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme3);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_GetUiSettings_Entao_DeveRetornarConfiguracoesComMenuEsquerdo()
        {
            var sut = CreateSut();

            var result = await sut.GetUiSettings();

            result.ShouldNotBeNull();
            result.BaseSettings.ShouldNotBeNull();
            result.BaseSettings.Theme.ShouldBe(MiddlewareAppConsts.Theme3);
            result.BaseSettings.Menu.SubmenuToggle.ShouldBe("Dropdown");
            result.IsLeftMenuUsed.ShouldBeTrue();
            result.IsTopMenuUsed.ShouldBeFalse();
            result.IsTabMenuUsed.ShouldBeFalse();
            result.AllowMenuScroll.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_UpdateApplicationUiManagementSettings_Entao_DeveChamarChangeSettingForApplication()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme3);
            var sut = new Theme3UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateApplicationUiManagementSettingsAsync(settings);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme3);
            await settingManager.Received(1).ChangeSettingForApplicationAsync($"{MiddlewareAppConsts.Theme3}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme3);
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_UpdateTenantUiManagementSettings_Entao_DeveChamarChangeSettingForTenant()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme3);
            var sut = new Theme3UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateTenantUiManagementSettingsAsync(42, settings);

            await settingManager.Received(1).ChangeSettingForTenantAsync(42, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme3);
            await settingManager.Received(1).ChangeSettingForTenantAsync(42, $"{MiddlewareAppConsts.Theme3}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme3);
        }

        [Fact]
        public async Task Dado_Theme3UiCustomizer_Quando_UpdateUserUiManagementSettings_Entao_DeveChamarChangeSettingForUser()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme3);
            var sut = new Theme3UiCustomizer(settingManager);
            var settings = CreateSettings();
            var user = new UserIdentifier(42, 1);

            await sut.UpdateUserUiManagementSettingsAsync(user, settings);

            await settingManager.Received(1).ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme3);
            await settingManager.Received(1).ChangeSettingForUserAsync(user, $"{MiddlewareAppConsts.Theme3}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme3);
        }
    }
}
