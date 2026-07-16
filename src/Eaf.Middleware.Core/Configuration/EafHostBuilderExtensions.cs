using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe EafHostBuilderExtensions.
    /// </summary>
    public static class EafHostBuilderExtensions
    {
        /// <summary>
        /// AddEnvironmentVariables and EAF_ AddInMemoryCollection SetBasePath AddJsonFile
        /// appsettings.json and appsettings.{EnvironmentName}.json
        /// </summary>
        /// <param name="builder">IHostBuilder</param>
        /// <param name="configureLogger">IConfigurationBuilder</param>
        /// <param name="prefix">prefix variable system ProjectName_</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder UseEafConfiguration(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureLogger = null, string prefix = null)
        {
            configureLogger ??= (ctx, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddInMemoryCollection();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();

                if (!string.IsNullOrEmpty(prefix))
                    config.AddEnvironmentVariables(prefix: prefix);
            };

            return builder.ConfigureAppConfiguration(configureLogger);
        }

        /// <summary>
        /// AddEnvironmentVariables and EAF_ AddInMemoryCollection SetBasePath AddJsonFile
        /// appsettings.json and appsettings.{EnvironmentName}.json
        /// </summary>
        /// <param name="builder">IHostBuilder</param>
        /// <param name="prefix">prefix variable system ProjectName_</param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder UseEafConfiguration(this IHostBuilder builder, string prefix)
        {
            return builder.UseEafConfiguration((ctx, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddInMemoryCollection();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();

                if (!string.IsNullOrEmpty(prefix))
                    config.AddEnvironmentVariables(prefix: prefix);
            });
        }
    }
}