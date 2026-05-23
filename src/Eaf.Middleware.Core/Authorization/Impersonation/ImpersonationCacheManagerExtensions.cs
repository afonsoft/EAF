using Abp.Runtime.Caching;

namespace Eaf.Middleware.Authorization.Impersonation
{
    /// <summary>
    /// Representa a classe ImpersonationCacheManagerExtensions.
    /// </summary>
    public static class ImpersonationCacheManagerExtensions
    {
        /// <summary>
        /// GetImpersonationCache.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public static ITypedCache<string, ImpersonationCacheItem> GetImpersonationCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, ImpersonationCacheItem>(ImpersonationCacheItem.CacheName);
        }
    }
}