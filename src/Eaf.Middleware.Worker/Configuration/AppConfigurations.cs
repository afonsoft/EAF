using Abp.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe AppConfigurations.
    /// </summary>
    public static class AppConfigurations
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> ConfigurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();

        /// <summary>
        /// Get.
        /// </summary>
        /// <param name="path">Parâmetro path.</param>
        /// <param name="environmentName">Parâmetro environmentName.</param>
        /// <param name="addUserSecrets">Parâmetro addUserSecrets.</param>
        /// <returns>Resultado da operação.</returns>
        public static IConfigurationRoot Get(string path, string environmentName = null, bool addUserSecrets = false)
        {
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("Hosting:Environment") ?? Environment.GetEnvironmentVariable("ASPNET_ENV");
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

            var cacheKey = path + "#" + environmentName + "#";
            return ConfigurationCache.GetOrAdd(
                cacheKey,
                _ => BuildConfiguration(path, environmentName)
            );
        }

        private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddInMemoryCollection()
                .AddEnvironmentVariables()
                .AddEnvironmentVariables(prefix: "EAF_")
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!environmentName.IsNullOrWhiteSpace())
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            return builder.Build();
        }
    }
}