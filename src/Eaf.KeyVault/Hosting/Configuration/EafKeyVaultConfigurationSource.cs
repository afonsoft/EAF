using Eaf.KeyVault;
using Microsoft.Extensions.Configuration;

namespace Eaf.Hosting.Configuration
{
    /// <summary>
    /// Representa a classe EafKeyVaultConfigurationSource.
    /// </summary>
    public class EafKeyVaultConfigurationSource : IConfigurationSource
    {
        private readonly EafKeyVaultOptions options;

        /// <summary>
        /// EafKeyVaultConfigurationSource.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <returns>Resultado da operação.</returns>
        public EafKeyVaultConfigurationSource(EafKeyVaultOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Build.
        /// </summary>
        /// <param name="builder">Parâmetro builder.</param>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new EafKeyVaultConfigurationProvider(options);
    }
}