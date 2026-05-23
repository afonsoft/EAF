using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using System;

namespace Eaf.Runtime.Caching.SqlServer
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class OracleCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Oracle as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseSqlServer(this ICachingConfiguration cachingConfiguration, Action<SqlServerCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.AbpConfiguration.IocManager;

            iocManager.RegisterIfNot<IDistributedCache, SqlServerCache>();
            iocManager.RegisterIfNot<ICacheManager, EafSqlServerCacheManager>();
            optionsAction(iocManager.Resolve<SqlServerCacheOptions>());
        }
    }
}