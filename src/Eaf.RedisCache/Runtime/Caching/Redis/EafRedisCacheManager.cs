using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Castle.Core.Logging;

namespace Eaf.Runtime.Caching.Redis
{
    /// <summary>
    /// Gerenciador de caches Redis do EAF.
    /// </summary>
    public class EafRedisCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Obtém ou define o logger do gerenciador de cache.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafRedisCacheManager"/>.
        /// </summary>
        /// <param name="iocManager">Gerenciador de IoC.</param>
        /// <param name="configuration">Configuração de caching.</param>
        public EafRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<EafRedisCache>(DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// Cria uma implementação de cache com o nome especificado.
        /// </summary>
        /// <param name="name">Nome do cache.</param>
        /// <returns>Instância de <see cref="ICache"/>.</returns>
        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<EafRedisCache>(new { name, Logger });
        }

        /// <summary>
        /// Libera todos os caches gerenciados.
        /// </summary>
        protected override void DisposeCaches()
        {
            foreach (var cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}
