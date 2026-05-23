using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using System;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Implements <see cref="ILdapSettings"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class LdapSettings : ILdapSettings, ITransientDependency
    {
        /// <summary>
        /// LdapSettings.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public LdapSettings(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        protected ISettingManager SettingManager { get; }

        /// <summary>
        /// GetContainer.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetContainer(int? tenantId)
        {
            return tenantId.HasValue
                ? SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.Container, tenantId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Container);
        }

        /// <summary>
        /// GetContextType.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual async Task<object> GetContextType(int? tenantId)
        {
            if (!OperatingSystem.IsWindows())
                return null;

            return tenantId.HasValue
                ? (await SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.ContextType, tenantId.Value)).ToEnum<ContextType>()
                : (await SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.ContextType)).ToEnum<ContextType>();
        }

        /// <summary>
        /// GetDomain.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetDomain(int? tenantId)
        {
            return tenantId.HasValue
                ? SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.Domain, tenantId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Domain);
        }

        /// <summary>
        /// GetIsEnabled.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<bool> GetIsEnabled(int? tenantId)
        {
            return tenantId.HasValue
                ? SettingManager.GetSettingValueForTenantAsync<bool>(LdapSettingNames.IsEnabled, tenantId.Value)
                : SettingManager.GetSettingValueForApplicationAsync<bool>(LdapSettingNames.IsEnabled);
        }

        /// <summary>
        /// GetPassword.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetPassword(int? tenantId)
        {
            return tenantId.HasValue
                ? SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.Password, tenantId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Password);
        }

        /// <summary>
        /// GetUserName.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<string> GetUserName(int? tenantId)
        {
            return tenantId.HasValue
                ? SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.UserName, tenantId.Value)
                : SettingManager.GetSettingValueForApplicationAsync(LdapSettingNames.UserName);
        }
    }
}