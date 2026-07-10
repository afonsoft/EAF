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
    public class Theme4UiCustomizerBddTests
    {
        private static Theme4UiCustomizer CreateSut()
        {
            return new Theme4UiCustomizer(UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme4));
        }

        private static ThemeSettingsDto CreateSettings()
        {
            return new ThemeSettingsDto
            {
                Theme = MiddlewareAppConsts.Theme4,
                Layout = new ThemeLayoutSettingsDto
                {
                    ThemeColor = MiddlewareAppConsts.Theme4,
                    LayoutType = MiddlewareAppConsts.Theme4,
                    ContentSkin = MiddlewareAppConsts.Theme4
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = true,
                    MobileFixedHeader = true,
                    HeaderSkin = MiddlewareAppConsts.Theme4
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = MiddlewareAppConsts.Theme4,
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
            typeof(Theme4UiCustomizer).BaseType.ShouldBe(typeof(UiThemeCustomizerBase));
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_GetHostUiManagementSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetHostUiManagementSettings();

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme4);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_GetTenantUiCustomizationSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetTenantUiCustomizationSettings(42);

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme4);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_GetUiSettings_Entao_DeveRetornarConfiguracoesComMenuTab()
        {
            var sut = CreateSut();

            var result = await sut.GetUiSettings();

            result.ShouldNotBeNull();
            result.BaseSettings.ShouldNotBeNull();
            result.BaseSettings.Theme.ShouldBe(MiddlewareAppConsts.Theme4);
            result.BaseSettings.Menu.Position.ShouldBe("tab");
            result.IsLeftMenuUsed.ShouldBeFalse();
            result.IsTopMenuUsed.ShouldBeFalse();
            result.IsTabMenuUsed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_UpdateApplicationUiManagementSettings_Entao_DeveChamarChangeSettingForApplication()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme4);
            var sut = new Theme4UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateApplicationUiManagementSettingsAsync(settings);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme4);
            await settingManager.Received(1).ChangeSettingForApplicationAsync($"{MiddlewareAppConsts.Theme4}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme4);
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_UpdateTenantUiManagementSettings_Entao_DeveChamarChangeSettingForTenant()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme4);
            var sut = new Theme4UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateTenantUiManagementSettingsAsync(42, settings);

            await settingManager.Received(1).ChangeSettingForTenantAsync(42, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme4);
            await settingManager.Received(1).ChangeSettingForTenantAsync(42, $"{MiddlewareAppConsts.Theme4}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme4);
        }

        [Fact]
        public async Task Dado_Theme4UiCustomizer_Quando_UpdateUserUiManagementSettings_Entao_DeveChamarChangeSettingForUser()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme4);
            var sut = new Theme4UiCustomizer(settingManager);
            var settings = CreateSettings();
            var user = new UserIdentifier(42, 1);

            await sut.UpdateUserUiManagementSettingsAsync(user, settings);

            await settingManager.Received(1).ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme4);
            await settingManager.Received(1).ChangeSettingForUserAsync(user, $"{MiddlewareAppConsts.Theme4}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme4);
        }
    }
}
