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
    public class Theme2UiCustomizerBddTests
    {
        private static Theme2UiCustomizer CreateSut()
        {
            return new Theme2UiCustomizer(UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme2));
        }

        private static ThemeSettingsDto CreateSettings()
        {
            return new ThemeSettingsDto
            {
                Theme = MiddlewareAppConsts.Theme2,
                Layout = new ThemeLayoutSettingsDto
                {
                    ThemeColor = MiddlewareAppConsts.Theme2,
                    LayoutType = MiddlewareAppConsts.Theme2,
                    ContentSkin = MiddlewareAppConsts.Theme2
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = true,
                    MobileFixedHeader = true,
                    HeaderSkin = MiddlewareAppConsts.Theme2
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = MiddlewareAppConsts.Theme2,
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
            typeof(Theme2UiCustomizer).BaseType.ShouldBe(typeof(UiThemeCustomizerBase));
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_GetHostUiManagementSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetHostUiManagementSettings();

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme2);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_GetTenantUiCustomizationSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetTenantUiCustomizationSettings(42);

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.Theme2);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_GetUiSettings_Entao_DeveRetornarConfiguracoesComMenuEsquerdo()
        {
            var sut = CreateSut();

            var result = await sut.GetUiSettings();

            result.ShouldNotBeNull();
            result.BaseSettings.ShouldNotBeNull();
            result.BaseSettings.Theme.ShouldBe(MiddlewareAppConsts.Theme2);
            result.BaseSettings.Menu.Position.ShouldBe("top");
            result.IsLeftMenuUsed.ShouldBeFalse();
            result.IsTopMenuUsed.ShouldBeTrue();
            result.IsTabMenuUsed.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_UpdateApplicationUiManagementSettings_Entao_DeveChamarChangeSettingForApplication()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme2);
            var sut = new Theme2UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateApplicationUiManagementSettingsAsync(settings);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme2);
            await settingManager.Received(1).ChangeSettingForApplicationAsync($"{MiddlewareAppConsts.Theme2}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme2);
            await settingManager.Received(1).ChangeSettingForApplicationAsync($"{MiddlewareAppConsts.Theme2}.{AppSettings.UiManagement.LeftAside.FixedAside}", "True");
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_UpdateTenantUiManagementSettings_Entao_DeveChamarChangeSettingForTenant()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme2);
            var sut = new Theme2UiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateTenantUiManagementSettingsAsync(42, settings);

            await settingManager.Received(1).ChangeSettingForTenantAsync(42, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme2);
            await settingManager.Received(1).ChangeSettingForTenantAsync(42, $"{MiddlewareAppConsts.Theme2}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme2);
        }

        [Fact]
        public async Task Dado_Theme2UiCustomizer_Quando_UpdateUserUiManagementSettings_Entao_DeveChamarChangeSettingForUser()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.Theme2);
            var sut = new Theme2UiCustomizer(settingManager);
            var settings = CreateSettings();
            var user = new UserIdentifier(42, 1);

            await sut.UpdateUserUiManagementSettingsAsync(user, settings);

            await settingManager.Received(1).ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, MiddlewareAppConsts.Theme2);
            await settingManager.Received(1).ChangeSettingForUserAsync(user, $"{MiddlewareAppConsts.Theme2}.App.UiManagement.ThemeColor", MiddlewareAppConsts.Theme2);
        }
    }
}
