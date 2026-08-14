using System;
using Abp.Configuration.Startup;

namespace Eaf.BlobStoring
{
    /// <summary>
    /// Extensões para acessar a configuração do módulo <see cref="EafBlobStoringModule"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Obtém a configuração do módulo de armazenamento de BLOBs do EAF.
        /// </summary>
        /// <param name="moduleConfigurations">Configurações dos módulos ABP.</param>
        /// <returns>Configuração do módulo de BLOBs.</returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="moduleConfigurations"/> for nulo.</exception>
        public static IEafBlobStoringConfiguration EafBlobStoring(this IModuleConfigurations moduleConfigurations)
        {
            if (moduleConfigurations == null)
            {
                throw new ArgumentNullException(nameof(moduleConfigurations));
            }

            return moduleConfigurations.AbpConfiguration.IocManager.Resolve<IEafBlobStoringConfiguration>();
        }
    }
}
