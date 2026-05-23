using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Eaf.Middleware.Configuration
{
    /* This service is replaced in Web layer and Test project separately */

    /// <summary>
    /// Representa a classe DefaultAppConfigurationAccessor.
    /// </summary>
    public class DefaultAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        /// <summary>
        /// DefaultAppConfigurationAccessor.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public DefaultAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Obtém ou define Configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }
    }
}