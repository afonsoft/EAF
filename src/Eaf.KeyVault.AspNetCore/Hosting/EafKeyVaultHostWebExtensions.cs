using Eaf.Hosting.Configuration;
using Eaf.KeyVault;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Representa a classe EafKeyVaultHostWebExtensions.
    /// </summary>
    public static class EafKeyVaultHostWebExtensions
    {
        /// <summary>
        /// UseEafKeyVault.
        /// </summary>
        /// <param name="builder">Parâmetro builder.</param>
        /// <param name="options">Parâmetro options.</param>
        /// <returns>Resultado da operação.</returns>
        public static IWebHostBuilder UseEafKeyVault(this IWebHostBuilder builder, Action<EafKeyVaultOptions> options = null)
        {
            var optionsDefault = new EafKeyVaultOptions();
            options?.Invoke(optionsDefault);

            builder.ConfigureServices(services =>
            {
                services.AddOptions<EafKeyVaultOptions>();

                if (options != null)
                    services.Configure<EafKeyVaultOptions>(options);
                else
                    services.Configure<EafKeyVaultOptions>(opt => opt.Provider = EnumKeyVault.None);
            });

            builder.ConfigureAppConfiguration((host, config) =>
            {
                config.Add(new EafKeyVaultConfigurationSource(optionsDefault));
            });

            return builder;
        }
    }
}