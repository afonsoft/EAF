using Abp.Dependency;
using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Representa a classe AppConfigurationAccessor.
    /// </summary>
    public class AppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        /// <summary>
        /// AppConfigurationAccessor.
        /// </summary>
        /// <param name="env">Parâmetro env.</param>
        /// <returns>Resultado da operação.</returns>
        public AppConfigurationAccessor(IHostEnvironment env)
        {
            Configuration = env.GetAppConfiguration();
        }

        /// <summary>
        /// Obtém ou define Configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }
    }
}