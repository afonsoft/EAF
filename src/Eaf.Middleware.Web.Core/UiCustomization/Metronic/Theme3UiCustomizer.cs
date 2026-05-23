using Abp;
using Abp.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization;
using Eaf.Middleware.UiCustomization.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.UiCustomization.Metronic
{
    /// <summary>
    /// Representa a classe Theme3UiCustomizer.
    /// </summary>
    public class Theme3UiCustomizer : UiThemeCustomizerBase, IUiCustomizer

    {
        /// <summary>
        /// Theme3UiCustomizer.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public Theme3UiCustomizer(SettingManager settingManager)
            : base(settingManager, MiddlewareAppConsts.Theme3)
        {
        }

        /// <summary>
        /// GetHostUiManagementSettings.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<ThemeSettingsDto> GetHostUiManagementSettings()
        {
            var theme = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.UiManagement.Theme);

            return new ThemeSettingsDto
            {
                Theme = theme,
                Layout = new ThemeLayoutSettingsDto
                {
                    LayoutType = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.LayoutType),
                    ContentSkin = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.ContentSkin),
                    ThemeColor = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.ThemeColor)
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Header.DesktopFixedHeader),
                    MobileFixedHeader = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.Header.MobileFixedHeader),
                    HeaderSkin = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.Header.Skin),
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = await GetSettingValueForApplicationAsync(AppSettings.UiManagement.LeftAside.AsideSkin),
                    FixedAside = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside.FixedAside),
                    AllowAsideMinimizing = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing),
                    DefaultMinimizedAside = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside),
                    AllowAsideHiding = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside.AllowAsideHiding),
                    DefaultHiddenAside = await GetSettingValueForApplicationAsync<bool>(AppSettings.UiManagement.LeftAside.DefaultHiddenAside)
                }
            };
        }

        /// <summary>
        /// GetTenantUiCustomizationSettings.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId)
        {
            var theme = await SettingManager.GetSettingValueForTenantAsync(AppSettings.UiManagement.Theme, tenantId);

            return new ThemeSettingsDto
            {
                Theme = theme,
                Layout = new ThemeLayoutSettingsDto
                {
                    LayoutType = await GetSettingValueForTenantAsync(AppSettings.UiManagement.LayoutType, tenantId),
                    ContentSkin = await GetSettingValueForTenantAsync(AppSettings.UiManagement.ContentSkin, tenantId),
                    ThemeColor = await GetSettingValueForTenantAsync(AppSettings.UiManagement.ThemeColor, tenantId)
                },
                Header = new ThemeHeaderSettingsDto
                {
                    DesktopFixedHeader = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Header.DesktopFixedHeader, tenantId),
                    MobileFixedHeader = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.Header.MobileFixedHeader, tenantId),
                    HeaderSkin = await GetSettingValueForTenantAsync(AppSettings.UiManagement.Header.Skin, tenantId),
                },
                Menu = new ThemeMenuSettingsDto
                {
                    AsideSkin = await GetSettingValueForTenantAsync(AppSettings.UiManagement.LeftAside.AsideSkin, tenantId),
                    FixedAside = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.FixedAside, tenantId),
                    AllowAsideMinimizing = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, tenantId),
                    DefaultMinimizedAside = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, tenantId),
                    AllowAsideHiding = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.AllowAsideHiding, tenantId),
                    DefaultHiddenAside = await GetSettingValueForTenantAsync<bool>(AppSettings.UiManagement.LeftAside.DefaultHiddenAside, tenantId)
                }
            };
        }

        /// <summary>
        /// GetUiSettings.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<UiCustomizationSettingsDto> GetUiSettings()
        {
            var settings = new UiCustomizationSettingsDto
            {
                BaseSettings = new ThemeSettingsDto
                {
                    Layout = new ThemeLayoutSettingsDto
                    {
                        LayoutType = await GetSettingValueAsync(AppSettings.UiManagement.LayoutType),
                        ContentSkin = await GetSettingValueAsync(AppSettings.UiManagement.ContentSkin),
                        ThemeColor = await GetSettingValueAsync(AppSettings.UiManagement.ThemeColor)
                    },
                    Header = new ThemeHeaderSettingsDto
                    {
                        DesktopFixedHeader = await GetSettingValueAsync<bool>(AppSettings.UiManagement.Header.DesktopFixedHeader),
                        MobileFixedHeader = await GetSettingValueAsync<bool>(AppSettings.UiManagement.Header.MobileFixedHeader),
                        HeaderSkin = await GetSettingValueAsync(AppSettings.UiManagement.Header.Skin),
                    },
                    Menu = new ThemeMenuSettingsDto
                    {
                        AsideSkin = await GetSettingValueAsync(AppSettings.UiManagement.LeftAside.AsideSkin),
                        FixedAside = await GetSettingValueAsync<bool>(AppSettings.UiManagement.LeftAside.FixedAside),
                    }
                }
            };

            settings.BaseSettings.Theme = ThemeName;
            settings.BaseSettings.Layout.ThemeColor = settings.BaseSettings.Theme;
            settings.BaseSettings.Menu.FixedAside = true;
            settings.BaseSettings.Menu.DefaultMinimizedAside = true;
            settings.BaseSettings.Menu.SubmenuToggle = "Dropdown";

            settings.IsLeftMenuUsed = true;
            settings.IsTopMenuUsed = false;
            settings.IsTabMenuUsed = false;
            settings.AllowMenuScroll = false;

            return settings;
        }

        /// <summary>
        /// UpdateApplicationUiManagementSettingsAsync.
        /// </summary>
        /// <param name="settings">Parâmetro settings.</param>
        public async Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.UiManagement.Theme, settings.Theme);

            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.ThemeColor, settings.Layout.ThemeColor);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LayoutType, settings.Layout.LayoutType);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.ContentSkin, settings.Layout.ContentSkin);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Header.DesktopFixedHeader, settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Header.MobileFixedHeader, settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.Header.Skin, settings.Header.HeaderSkin);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.AsideSkin, settings.Menu.AsideSkin);
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.FixedAside, settings.Menu.FixedAside.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.AllowAsideHiding, settings.Menu.AllowAsideHiding.ToString());
            await ChangeSettingForApplicationAsync(AppSettings.UiManagement.LeftAside.DefaultHiddenAside, settings.Menu.DefaultHiddenAside.ToString());
        }

        /// <summary>
        /// UpdateTenantUiManagementSettingsAsync.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="settings">Parâmetro settings.</param>
        public async Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Theme, settings.Theme);

            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.ThemeColor, settings.Layout.ThemeColor);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LayoutType, settings.Layout.LayoutType);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.ContentSkin, settings.Layout.ContentSkin);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Header.DesktopFixedHeader, settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Header.MobileFixedHeader, settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.Header.Skin, settings.Header.HeaderSkin);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.AsideSkin, settings.Menu.AsideSkin);
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.FixedAside, settings.Menu.FixedAside.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.AllowAsideHiding, settings.Menu.AllowAsideHiding.ToString());
            await ChangeSettingForTenantAsync(tenantId, AppSettings.UiManagement.LeftAside.DefaultHiddenAside, settings.Menu.DefaultHiddenAside.ToString());
        }

        /// <summary>
        /// UpdateUserUiManagementSettingsAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="settings">Parâmetro settings.</param>
        public async Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings)
        {
            await SettingManager.ChangeSettingForUserAsync(user, AppSettings.UiManagement.Theme, ThemeName);

            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.ThemeColor, settings.Layout.ThemeColor);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LayoutType, settings.Layout.LayoutType);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.ContentSkin, settings.Layout.ContentSkin);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Header.DesktopFixedHeader, settings.Header.DesktopFixedHeader.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Header.MobileFixedHeader, settings.Header.MobileFixedHeader.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.Header.Skin, settings.Header.HeaderSkin);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AsideSkin, settings.Menu.AsideSkin);
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.FixedAside, settings.Menu.FixedAside.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, settings.Menu.AllowAsideMinimizing.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, settings.Menu.DefaultMinimizedAside.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.AllowAsideHiding, settings.Menu.AllowAsideHiding.ToString());
            await ChangeSettingForUserAsync(user, AppSettings.UiManagement.LeftAside.DefaultHiddenAside, settings.Menu.DefaultHiddenAside.ToString());

        }
    }
}