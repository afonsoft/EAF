using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Configuration;
using System;

namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Configura provedores de cache (Redis, SQL Server) baseado na configuração.
    /// </summary>
    internal static class CacheConfigurer
    {
        /// <summary>
        /// Configura o subsistema de cache baseado na configuração da aplicação.
        /// </summary>
        /// <param name="configuration">Configuração de startup do ABP.</param>
        /// <param name="appConfiguration">Raiz da configuração da aplicação.</param>
        /// <param name="iocManager">Gerenciador de IoC.</param>
        public static void Configure(
            IAbpStartupConfiguration configuration,
            IConfigurationRoot appConfiguration,
            IIocManager iocManager)
        {
            configuration.Caching.ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            });

            if (IsRedisEnabled(appConfiguration))
            {
                configuration.IocManager.RegisterIfNot(typeof(AbpRedisCacheOptions));

                configuration.Caching.UseRedis(options =>
                {
                    options.ConnectionString = appConfiguration["RedisCache:ConnectionString"];
                    options.DatabaseId = appConfiguration.GetValue<int>("RedisCache:DatabaseId");
                });

                iocManager.Register<IAbpPerRequestRedisCache, AbpPerRequestRedisCache>();
                iocManager.Register<IAbpPerRequestRedisCacheManager, AbpPerRequestRedisCacheManager>();
            }

            if (IsSqlServerCacheEnabled(appConfiguration))
            {
                configuration.Caching.UseSqlServer(options =>
                {
                    options.ConnectionString = appConfiguration["SqlServerCache:ConnectionString"] ?? configuration.DefaultNameOrConnectionString;
                    options.SchemaName = appConfiguration["SqlServerCache:SchemaName"] ?? null;
                    options.TableName = appConfiguration["SqlServerCache:TableName"] ?? "EafCache";
                });
            }
        }

        private static bool IsRedisEnabled(IConfigurationRoot appConfiguration)
        {
            return (appConfiguration["RedisCache:IsRedisEnabled"] != null && bool.Parse(appConfiguration["RedisCache:IsRedisEnabled"]))
                || (appConfiguration["RedisCache:IsEnabled"] != null && bool.Parse(appConfiguration["RedisCache:IsEnabled"]));
        }

        private static bool IsSqlServerCacheEnabled(IConfigurationRoot appConfiguration)
        {
            return (appConfiguration["SqlServer:IsSqlEnabled"] != null && bool.Parse(appConfiguration["SqlServer:IsSqlEnabled"]))
                || (appConfiguration["SqlServerCache:IsEnabled"] != null && bool.Parse(appConfiguration["SqlServerCache:IsEnabled"]));
        }
    }
}
