using Abp.Runtime.Caching;
using System;

namespace Eaf.Middleware.Storage
{
    /// <summary>
    /// Representa a classe TempFileCacheManager.
    /// </summary>
    public class TempFileCacheManager : ITempFileCacheManager
    {
        public const string TempFileCacheName = "TempFileCacheName";

        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// TempFileCacheManager.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TempFileCacheManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// GetFile.
        /// </summary>
        /// <param name="token">Parâmetro token.</param>
        /// <returns>Resultado da operação.</returns>
        public byte[] GetFile(string token)
        {
            return _cacheManager.GetCache(TempFileCacheName).Get(token, ep => ep) as byte[];
        }

        /// <summary>
        /// SetFile.
        /// </summary>
        /// <param name="token">Parâmetro token.</param>
        /// <param name="content">Parâmetro content.</param>
        public void SetFile(string token, byte[] content)
        {
            _cacheManager.GetCache(TempFileCacheName).Set(token, content, new TimeSpan(0, 0, 5, 0)); // expire time is 5 min by default
        }
    }
}