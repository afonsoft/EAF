using Eaf.Hosting.Configuration;
using Eaf.KeyVault;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Representa a classe EafKeyVaultHostExtensions.
    /// </summary>
    public static class EafKeyVaultHostExtensions
    {
        /// <summary>
        /// UseEafKeyVault.
        /// </summary>
        /// <param name="builder">Parâmetro builder.</param>
        /// <param name="options">Parâmetro options.</param>
        /// <returns>Resultado da operação.</returns>
        public static IHostBuilder UseEafKeyVault(this IHostBuilder builder, Action<EafKeyVaultOptions> options = null)
        {
            var optionsDefault = new EafKeyVaultOptions();
            options?.Invoke(optionsDefault);

            builder.ConfigureServices((host, services) =>
            {
                services.AddOptions<EafKeyVaultOptions>();
                if (options != null)
                    services.Configure<EafKeyVaultOptions>(options);
            });

            builder.ConfigureAppConfiguration((host, config) =>
            {
                config.Add(new EafKeyVaultConfigurationSource(optionsDefault));
            });

            return builder;
        }
    }
}