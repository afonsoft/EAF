using Abp.Configuration;
using System.Collections.Generic;

namespace Eaf.MailKit.Configuration
{
    /// <summary>
    /// Define as configurações (settings) específicas do módulo EAF MailKit.
    /// </summary>
    public class EafMailKitSettingProvider : SettingProvider
    {
        private readonly EafMailKitConfiguration _configuration;

        /// <summary>
        /// EafMailKitSettingProvider.
        /// </summary>
        /// <param name="configuration">Configuração padrão do módulo.</param>
        public EafMailKitSettingProvider(EafMailKitConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retorna as definições de configuração do módulo.
        /// </summary>
        /// <param name="context">Contexto do provedor de definições.</param>
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(
                    EafMailKitSettingNames.RetryCount,
                    _configuration.RetryCount.ToString(),
                    scopes: SettingScopes.Application | SettingScopes.Tenant),
                new SettingDefinition(
                    EafMailKitSettingNames.RetryDelayMilliseconds,
                    _configuration.RetryDelayMilliseconds.ToString(),
                    scopes: SettingScopes.Application | SettingScopes.Tenant),
                new SettingDefinition(
                    EafMailKitSettingNames.DisableCertificateValidation,
                    _configuration.DisableCertificateValidation.ToString().ToLowerInvariant(),
                    scopes: SettingScopes.Application | SettingScopes.Tenant)
            };
        }
    }
}
