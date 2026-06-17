using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de UiCustomizationSettings.
    /// </summary>
    [AbpAuthorize]
    public class UiCustomizationSettingsAppService : MiddlewareAppServiceBase, IUiCustomizationSettingsAppService
    {
        private readonly IIocResolver _iocResolver;
        private readonly SettingManager _settingManager;
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;

        /// <summary>
        /// UiCustomizationSettingsAppService.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="iocResolver">Parâmetro iocResolver.</param>
        /// <param name="uiThemeCustomizerFactory">Parâmetro uiThemeCustomizerFactory.</param>
        /// <returns>Resultado da operação.</returns>
        public UiCustomizationSettingsAppService(
            SettingManager settingManager,
            IIocResolver iocResolver,
            IUiThemeCustomizerFactory uiThemeCustomizerFactory
        )
        {
            _settingManager = settingManager;
            _iocResolver = iocResolver;
            _uiThemeCustomizerFactory = uiThemeCustomizerFactory;
        }

        /// <summary>
        /// GetUiManagementSettings.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<List<ThemeSettingsDto>> GetUiManagementSettings()
        {
            var settings = new List<ThemeSettingsDto>();
            var themeCustomizers = _iocResolver.ResolveAll<IUiCustomizer>();

            foreach (var themeUiCustomizer in themeCustomizers)
            {
                var themeSettings = await themeUiCustomizer.GetUiSettings();
                settings.Add(themeSettings.BaseSettings);
            }

            return settings;
        }

        /// <summary>
        /// UpdateDefaultUiManagementSettings.
        /// </summary>
        /// <param name="settings">Parâmetro settings.</param>
        public async Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings)
        {
            var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(settings.Theme);

            if (AbpSession.TenantId.HasValue)
            {
                await themeCustomizer.UpdateTenantUiManagementSettingsAsync(AbpSession.TenantId.Value, settings);
            }
            else
            {
                await themeCustomizer.UpdateApplicationUiManagementSettingsAsync(settings);
            }
        }

        /// <summary>
        /// UpdateUiManagementSettings.
        /// </summary>
        /// <param name="settings">Parâmetro settings.</param>
        public async Task UpdateUiManagementSettings(ThemeSettingsDto settings)
        {
            var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(settings.Theme);
            await themeCustomizer.UpdateUserUiManagementSettingsAsync(AbpSession.ToUserIdentifier(), settings);
        }

        /// <summary>
        /// UseSystemDefaultSettings.
        /// </summary>
        public async Task UseSystemDefaultSettings()
        {
            if (AbpSession.TenantId.HasValue)
            {
                var theme = await _settingManager.GetSettingValueForTenantAsync(AppSettings.UiManagement.Theme, AbpSession.TenantId.Value);
                var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(theme);
                var settings = await themeCustomizer.GetTenantUiCustomizationSettings(AbpSession.TenantId.Value);
                await themeCustomizer.UpdateUserUiManagementSettingsAsync(AbpSession.ToUserIdentifier(), settings);
            }
            else
            {
                var theme = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.UiManagement.Theme);
                var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(theme);
                var settings = await themeCustomizer.GetHostUiManagementSettings();
                await themeCustomizer.UpdateUserUiManagementSettingsAsync(AbpSession.ToUserIdentifier(), settings);
            }
        }
    }
}