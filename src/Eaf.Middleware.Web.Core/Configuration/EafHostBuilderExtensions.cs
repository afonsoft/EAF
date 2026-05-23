using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Eaf.Middleware.Web.Configuration
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
        public static IHostBuilder UseAbpConfiguration(this IHostBuilder builder, Action<HostBuilderContext, IConfigurationBuilder> configureLogger = null, string prefix = null)
        {
            configureLogger ??= (ctx, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddInMemoryCollection();
                    config.AddEnvironmentVariables();
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);

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
        public static IHostBuilder UseAbpConfiguration(this IHostBuilder builder, string prefix)
        {
            return builder.UseAbpConfiguration((ctx, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddInMemoryCollection();
                config.AddEnvironmentVariables();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);

                if (!string.IsNullOrEmpty(prefix))
                    config.AddEnvironmentVariables(prefix: prefix);
            });
        }

        /// <summary>
        /// AddEnvironmentVariables and EAF_ AddInMemoryCollection SetBasePath AddJsonFile
        /// appsettings.json and appsettings.{EnvironmentName}.json
        /// </summary>
        /// <param name="builder">IWebHostBuilder</param>
        /// <param name="configureLogger">IConfigurationBuilder</param>
        /// <param name="prefix">prefix variable system ProjectName_</param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseAbpConfiguration(this IWebHostBuilder builder, Action<WebHostBuilderContext, IConfigurationBuilder> configureLogger = null, string prefix = null)
        {
            configureLogger ??= (ctx, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddInMemoryCollection();
                    config.AddEnvironmentVariables();
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);

                    if (!string.IsNullOrEmpty(prefix))
                        config.AddEnvironmentVariables(prefix: prefix);
                };

            return builder.ConfigureAppConfiguration(configureLogger);
        }

        /// <summary>
        /// AddEnvironmentVariables and EAF_ AddInMemoryCollection SetBasePath AddJsonFile
        /// appsettings.json and appsettings.{EnvironmentName}.json
        /// </summary>
        /// <param name="builder">IWebHostBuilder</param>
        /// <param name="prefix">prefix variable system ProjectName_</param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseAbpConfiguration(this IWebHostBuilder builder, string prefix)
        {
            return builder.UseAbpConfiguration((ctx, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddInMemoryCollection();
                config.AddEnvironmentVariables();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true);

                if (!string.IsNullOrEmpty(prefix))
                    config.AddEnvironmentVariables(prefix: prefix);
            });
        }
    }
}