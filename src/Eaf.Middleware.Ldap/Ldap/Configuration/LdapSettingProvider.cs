using Abp.Configuration;
using Abp.Localization;
using System.Collections.Generic;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Defines LDAP settings.
    /// </summary>

    public class LdapSettingProvider : SettingProvider
    {
        /// <summary>
        /// LdapSettingProvider.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public LdapSettingProvider()
        {
            LocalizationSourceName = "EafLdap";
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
                       new SettingDefinition(LdapSettingNames.IsEnabled, "false", L("Ldap_IsEnabled"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(LdapSettingNames.ContextType, "Domain", L("Ldap_ContextType"), scopes: SettingScopes.Application | SettingScopes.Tenant),
                       new SettingDefinition(LdapSettingNames.Container, null, L("Ldap_Container"), scopes: SettingScopes.Application  | SettingScopes.Tenant),
                       new SettingDefinition(LdapSettingNames.Domain, null, L("Ldap_Domain"), scopes: SettingScopes.Application  | SettingScopes.Tenant),
                       new SettingDefinition(LdapSettingNames.UserName, null, L("Ldap_UserName"), scopes: SettingScopes.Application  | SettingScopes.Tenant),
                       new SettingDefinition(LdapSettingNames.Password, null, L("Ldap_Password"), scopes: SettingScopes.Application  | SettingScopes.Tenant, isEncrypted:true)
                   };
        }

        protected virtual ILocalizableString L(string name)
        {
            return new LocalizableString(name, LocalizationSourceName);
        }
    }
}