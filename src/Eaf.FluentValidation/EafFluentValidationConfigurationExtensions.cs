using Abp.Configuration.Startup;
using System;

namespace Eaf.FluentValidation
{
    /// <summary>
    /// Extensões para acessar a configuração do módulo <see cref="EafFluentValidationModule"/>.
    /// </summary>
    public static class EafFluentValidationConfigurationExtensions
    {
        /// <summary>
        /// Obtém as opções de configuração do FluentValidation no EAF.
        /// </summary>
        /// <param name="moduleConfigurations">Configurações dos módulos ABP.</param>
        /// <returns>Opções de configuração do FluentValidation.</returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="moduleConfigurations"/> for nulo.</exception>
        public static EafFluentValidationOptions EafFluentValidation(this IModuleConfigurations moduleConfigurations)
        {
            if (moduleConfigurations == null)
            {
                throw new ArgumentNullException(nameof(moduleConfigurations));
            }

            return moduleConfigurations.AbpConfiguration.IocManager.Resolve<EafFluentValidationOptions>();
        }
    }
}
