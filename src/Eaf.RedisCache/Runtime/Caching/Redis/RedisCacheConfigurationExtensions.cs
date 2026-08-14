using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;

namespace Eaf.Runtime.Caching.Redis
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/> to configure Redis cache.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Configura o cache para usar Redis.
        /// </summary>
        /// <param name="cachingConfiguration">A configuração de caching.</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseRedis(options => { });
        }

        /// <summary>
        /// Configura o cache para usar Redis.
        /// </summary>
        /// <param name="cachingConfiguration">A configuração de caching.</param>
        /// <param name="optionsAction">Ação para configurar <see cref="EafRedisCacheOptions"/>.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="cachingConfiguration"/> for nulo.</exception>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<EafRedisCacheOptions> optionsAction)
        {
            if (cachingConfiguration == null)
            {
                throw new ArgumentNullException(nameof(cachingConfiguration));
            }

            var iocManager = cachingConfiguration.AbpConfiguration.IocManager;

            iocManager.RegisterIfNot<IDistributedCache, RedisCache>();
            iocManager.RegisterIfNot<ICacheManager, EafRedisCacheManager>();

            optionsAction(iocManager.Resolve<EafRedisCacheOptions>());
        }
    }
}
