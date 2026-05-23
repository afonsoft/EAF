using Castle.Core.Logging;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching;

namespace Eaf.Runtime.Caching.SqlServer
{
    /// <summary>
    /// Representa a classe EafSqlServerCacheManager.
    /// </summary>
    public class EafSqlServerCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;
        /// <summary>
        /// Obtém ou define Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EafSqlServerCacheManager"/> class.
        /// </summary>
        public EafSqlServerCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<EafSqlServerCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<EafSqlServerCache>(new { name, Logger });
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}