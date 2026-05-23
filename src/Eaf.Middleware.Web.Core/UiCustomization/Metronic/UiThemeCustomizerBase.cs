using Abp;
using Abp.Configuration;
using Abp.Extensions;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.UiCustomization.Metronic
{
    /// <summary>
    /// Representa a classe UiThemeCustomizerBase.
    /// </summary>
    public class UiThemeCustomizerBase
    {
        protected SettingManager SettingManager;
        protected string ThemeName;

        /// <summary>
        /// UiThemeCustomizerBase.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="themeName">Parâmetro themeName.</param>
        /// <returns>Resultado da operação.</returns>
        public UiThemeCustomizerBase(SettingManager settingManager, string themeName)
        {
            SettingManager = settingManager;
            ThemeName = themeName;
        }

        protected async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            await SettingManager.ChangeSettingForApplicationAsync(ThemeName + "." + name, value);
        }

        protected async Task ChangeSettingForTenantAsync(int tenantId, string name, string value)
        {
            await SettingManager.ChangeSettingForTenantAsync(tenantId, ThemeName + "." + name, value);
        }

        protected async Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value)
        {
            await SettingManager.ChangeSettingForUserAsync(user, ThemeName + "." + name, value);
        }

        protected async Task<string> GetSettingValueAsync(string settingName)
        {
            return await SettingManager.GetSettingValueAsync(ThemeName + "." + settingName);
        }

        protected async Task<T> GetSettingValueAsync<T>(string settingName) where T : struct
        {
            return (await SettingManager.GetSettingValueAsync(ThemeName + "." + settingName)).To<T>();
        }

        protected async Task<string> GetSettingValueForApplicationAsync(string settingName)
        {
            return await SettingManager.GetSettingValueForApplicationAsync(ThemeName + "." + settingName);
        }

        protected async Task<T> GetSettingValueForApplicationAsync<T>(string settingName) where T : struct
        {
            return (await SettingManager.GetSettingValueForApplicationAsync(ThemeName + "." + settingName)).To<T>();
        }

        protected async Task<string> GetSettingValueForTenantAsync(string settingName, int tenantId)
        {
            return await SettingManager.GetSettingValueForTenantAsync(ThemeName + "." + settingName, tenantId);
        }

        protected async Task<T> GetSettingValueForTenantAsync<T>(string settingName, int tenantId) where T : struct
        {
            return (await SettingManager.GetSettingValueForTenantAsync(ThemeName + "." + settingName, tenantId)).To<T>();
        }
    }
}