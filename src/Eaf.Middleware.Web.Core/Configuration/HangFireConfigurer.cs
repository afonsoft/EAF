using Eaf.Hangfire;
using Hangfire;
using Hangfire.Console;
using Hangfire.Heartbeat;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Representa a classe HangFireConfigurer.
    /// </summary>
    public static class HangFireConfigurer
    {
        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="services">Parâmetro services.</param>
        /// <param name="configuration">Parâmetro configuration.</param>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            bool IsEnabled = configuration["Hangfire:IsEnabled"] != null && bool.Parse(configuration["Hangfire:IsEnabled"]);

            #region Configure

            if (IsEnabled)
            {
                var storageType = ResolveStorageType(configuration);

                services.AddHangfire(config =>
                {
                    // Storage registration: SQL Server path relies on JobStorage.Current set in
                    // MiddlewareWebCoreModule.PostInitialize(). Redis and InMemory use a temporary
                    // MemoryStorage here; PostInitialize() replaces it with the real storage.
                    // This avoids creating a RedisStorage (and its ConnectionMultiplexer) here
                    // only to have PostInitialize() create a second one, leaking the first connection.
                    if (storageType != HangfireStorageType.SqlServer)
                        config.UseMemoryStorage();

                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_110);
                    config.UseRecommendedSerializerSettings();
                    config.UseSimpleAssemblyNameTypeSerializer();
                    config.UseSerilogLogProvider();
                    config.UseConsole();
                    config.UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(5));
                });
            }

            #endregion Configure
        }

        /// <summary>
        /// Determina o tipo de armazenamento do Hangfire com base na configuracao.
        /// SQL Server habilitado -> SQL Server
        /// Nao SQL Server + Redis habilitado -> Redis
        /// Nao SQL Server + Redis desabilitado -> InMemory
        /// </summary>
        public static HangfireStorageType ResolveStorageType(IConfiguration configuration)
        {
            bool.TryParse(configuration["Hangfire:IsInMemoryDatabase"], out bool forceInMemory);

            if (forceInMemory)
                return HangfireStorageType.InMemory;

            var databaseProvider = configuration["Database:Provider"] ?? "SqlServer";
            var isSqlServer = databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) ||
                            databaseProvider.Equals("MSSQL", StringComparison.OrdinalIgnoreCase);

            if (isSqlServer)
                return HangfireStorageType.SqlServer;

            bool.TryParse(configuration["RedisCache:IsRedisEnabled"], out bool isRedisEnabledLegacy);
            bool.TryParse(configuration["RedisCache:IsEnabled"], out bool isRedisEnabledNew);
            var isRedisEnabled = isRedisEnabledLegacy || isRedisEnabledNew;

            if (isRedisEnabled)
                return HangfireStorageType.Redis;

            return HangfireStorageType.InMemory;
        }
    }
}
