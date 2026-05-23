using Abp.Runtime.Caching;

namespace Eaf.Middleware.Authorization.TwoFactor
{
    /// <summary>
    /// Representa a classe TwoFactorCodeCacheExtensions.
    /// </summary>
    public static class TwoFactorCodeCacheExtensions
    {
        /// <summary>
        /// GetTwoFactorCodeCache.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public static ITypedCache<string, TwoFactorCodeCacheItem> GetTwoFactorCodeCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, TwoFactorCodeCacheItem>(TwoFactorCodeCacheItem.CacheName);
        }
    }
}