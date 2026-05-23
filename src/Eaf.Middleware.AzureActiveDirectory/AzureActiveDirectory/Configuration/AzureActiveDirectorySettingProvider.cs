using Abp;
using Abp.Configuration;
using Abp.Localization;
using System.Collections.Generic;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Defines AzureActiveDirectory settings.
    /// </summary>
    public class AzureActiveDirectorySettingProvider : SettingProvider
    {
        /// <summary>
        /// AzureActiveDirectorySettingProvider.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public AzureActiveDirectorySettingProvider()
        {
            LocalizationSourceName = "EafAzureActiveDirectory";
        }

        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// GetSettingDefinitions.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        /// <returns>Resultado da operação.</returns>
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(AzureActiveDirectorySettingNames.IsEnabled, "false", L("AzureActiveDirectory_IsEnabled"), scopes: SettingScopes.Application),
                       new SettingDefinition(AzureActiveDirectorySettingNames.ClientId, null, L("AzureActiveDirectory_ClientId"), scopes: SettingScopes.Application),
                       new SettingDefinition(AzureActiveDirectorySettingNames.Tenant, null, L("AzureActiveDirectory_Tenant"), scopes: SettingScopes.Application),
                       new SettingDefinition(AzureActiveDirectorySettingNames.ClientSecret, null, L("AzureActiveDirectory_ClientSecret"), scopes: SettingScopes.Application),
                   };
        }

        protected virtual ILocalizableString L(string name)
        {
            return new LocalizableString(name, LocalizationSourceName);
        }
    }
}