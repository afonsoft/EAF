using Abp.Configuration;
using Abp.Dependency;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Implements <see cref="IAzureActiveDirectorySettings"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>

    public class AzureActiveDirectorySettings : IAzureActiveDirectorySettings, ITransientDependency
    {
        /// <summary>
        /// AzureActiveDirectorySettings.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public AzureActiveDirectorySettings(ISettingManager settingManager)
        {
            SettingManager = settingManager ?? throw new ArgumentNullException(nameof(settingManager));
        }

        protected ISettingManager SettingManager { get; }

        /// <summary>
        /// GetClientId.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetClientId()
        {
            return SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientId);
        }

        /// <summary>
        /// GetClientSecret.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetClientSecret()
        {
            return SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret);
        }

        /// <summary>
        /// GetIsEnabled.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<bool> GetIsEnabled()
        {
            return SettingManager.GetSettingValueForApplicationAsync<bool>(AzureActiveDirectorySettingNames.IsEnabled);
        }

        /// <summary>
        /// GetTenant.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetTenant()
        {
            return SettingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.Tenant);
        }
    }
}