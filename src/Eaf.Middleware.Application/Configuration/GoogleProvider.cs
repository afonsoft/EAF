using Abp;
using Abp.Configuration;
using Abp.Localization;
using System.Collections.Generic;

namespace Eaf.Middleware.Configuration
{
    internal class GoogleProvider : SettingProvider
    {
        /// <summary>
        /// GetSettingDefinitions.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        /// <returns>Resultado da operação.</returns>
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                    {
                       new SettingDefinition(EafMiddlewareSettingNames.Google.Analytics, "", L("Analytics"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                       new SettingDefinition(EafMiddlewareSettingNames.Google.TagManager, "", L("TagManager"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                       new SettingDefinition(EafMiddlewareSettingNames.Google.RecaptchaSiteKey, "", L("RecaptchaSiteKey"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true)
                    };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, AbpConsts.LocalizationSourceName);
        }
    }
}