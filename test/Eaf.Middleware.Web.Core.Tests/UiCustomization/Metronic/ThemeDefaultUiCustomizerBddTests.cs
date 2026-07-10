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
    public class ThemeDefaultUiCustomizerBddTests
    {
        private static ThemeDefaultUiCustomizer CreateSut()
        {
            return new ThemeDefaultUiCustomizer(UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.ThemeDefault));
        }

        private static ThemeSettingsDto CreateSettings()
        {
            return new ThemeSettingsDto
            {
                Theme = MiddlewareAppConsts.ThemeDefault,
                Layout = new ThemeLayoutSettingsDto
                {
                    ThemeColor = MiddlewareAppConsts.ThemeDefault,
                    LayoutType = MiddlewareAppConsts.ThemeDefault,
                    ContentSkin = MiddlewareAppConsts.ThemeDefault
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = true,
                    MobileFixedHeader = true,
                    HeaderSkin = MiddlewareAppConsts.ThemeDefault
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = MiddlewareAppConsts.ThemeDefault,
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
            typeof(ThemeDefaultUiCustomizer).BaseType.ShouldBe(typeof(UiThemeCustomizerBase));
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_GetHostUiManagementSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetHostUiManagementSettings();

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.ThemeDefault);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_GetTenantUiCustomizationSettings_Entao_DeveRetornarConfiguracoesComTema()
        {
            var sut = CreateSut();

            var result = await sut.GetTenantUiCustomizationSettings(42);

            result.ShouldNotBeNull();
            result.Theme.ShouldBe(MiddlewareAppConsts.ThemeDefault);
            result.Layout.ShouldNotBeNull();
            result.Header.ShouldNotBeNull();
            result.Menu.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_GetUiSettings_Entao_DeveRetornarConfiguracoesComMenuEsquerdo()
        {
            var sut = CreateSut();

            var result = await sut.GetUiSettings();

            result.ShouldNotBeNull();
            result.BaseSettings.ShouldNotBeNull();
            result.BaseSettings.Theme.ShouldBe(MiddlewareAppConsts.ThemeDefault);
            result.BaseSettings.Menu.Position.ShouldBe("left");
            result.BaseSettings.Menu.SubmenuToggle.ShouldBe("Accordion");
            result.IsLeftMenuUsed.ShouldBeTrue();
            result.IsTopMenuUsed.ShouldBeFalse();
            result.IsTabMenuUsed.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_UpdateApplicationUiManagementSettings_Entao_DeveChamarChangeSettingForApplication()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.ThemeDefault);
            var sut = new ThemeDefaultUiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateApplicationUiManagementSettingsAsync(settings);

            await settingManager.Received(1).ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, MiddlewareAppConsts.ThemeDefault);
            await settingManager.Received(1).ChangeSettingForApplicationAsync($"{MiddlewareAppConsts.ThemeDefault}.App.UiManagement.ThemeColor", MiddlewareAppConsts.ThemeDefault);
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_UpdateTenantUiManagementSettings_Entao_DeveChamarChangeSettingForTenant()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.ThemeDefault);
            var sut = new ThemeDefaultUiCustomizer(settingManager);
            var settings = CreateSettings();

            await sut.UpdateTenantUiManagementSettingsAsync(42, settings);

            await settingManager.Received(1).ChangeSettingForTenantAsync(42, AppSettings.UiManagement.Theme, MiddlewareAppConsts.ThemeDefault);
            await settingManager.Received(1).ChangeSettingForTenantAsync(42, $"{MiddlewareAppConsts.ThemeDefault}.App.UiManagement.ThemeColor", MiddlewareAppConsts.ThemeDefault);
        }

        [Fact]
        public async Task Dado_ThemeDefaultUiCustomizer_Quando_UpdateUserUiManagementSettings_Entao_DeveChamarChangeSettingForUser()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager(MiddlewareAppConsts.ThemeDefault);
            var sut = new ThemeDefaultUiCustomizer(settingManager);
            var settings = CreateSettings();
            var user = new UserIdentifier(42, 1);

            await sut.UpdateUserUiManagementSettingsAsync(user, settings);

            await settingManager.Received(1).ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, MiddlewareAppConsts.ThemeDefault);
            await settingManager.Received(1).ChangeSettingForUserAsync(user, $"{MiddlewareAppConsts.ThemeDefault}.App.UiManagement.ThemeColor", MiddlewareAppConsts.ThemeDefault);
        }
    }
}
