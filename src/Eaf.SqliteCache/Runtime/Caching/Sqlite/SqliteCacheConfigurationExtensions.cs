using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using System;

namespace Abp.Runtime.Caching.Sqlite
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class SqliteCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Sqlite as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseSqlite(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseSqlite(options => { });
        }

        /// <summary>
        /// Configures caching to use Sqlite as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseSqlite(this ICachingConfiguration cachingConfiguration, Action<EafSqliteCacheOptions> optionsAction)
        {
            if (cachingConfiguration == null)
            {
                throw new ArgumentNullException(nameof(cachingConfiguration));
            }

            var iocManager = cachingConfiguration.AbpConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, EafSqliteCacheManager>();

            optionsAction(iocManager.Resolve<EafSqliteCacheOptions>());
        }
    }
}