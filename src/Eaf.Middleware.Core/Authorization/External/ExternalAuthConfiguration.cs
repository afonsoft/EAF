using Abp.Dependency;
using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe ExternalAuthConfiguration.
    /// </summary>
    public class ExternalAuthConfiguration : IExternalAuthConfiguration, ISingletonDependency
    {
        /// <summary>
        /// ExternalAuthConfiguration.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public ExternalAuthConfiguration()
        {
            this.ExternalLoginInfoProviders = new List<IExternalLoginInfoProvider>();
        }

        public List<IExternalLoginInfoProvider> ExternalLoginInfoProviders { get; }
    }
}