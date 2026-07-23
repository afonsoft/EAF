using Abp.Dependency;
using Abp.Runtime.Caching;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Implementação do armazenamento de refresh tokens usando o cache distribuído do ABP.
    /// </summary>
    public class RefreshTokenStore : IRefreshTokenStore, ISingletonDependency
    {
        private readonly ITypedCache<string, RefreshTokenInfo> _cache;

        /// <summary>
        /// Cria uma nova instância do <see cref="RefreshTokenStore"/>.
        /// </summary>
        /// <param name="cacheManager">Gerenciador de cache do ABP.</param>
        public RefreshTokenStore(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, RefreshTokenInfo>("EafRefreshTokens");
        }

        /// <inheritdoc />
        public async Task<RefreshTokenInfo> GetAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            return await _cache.GetOrDefaultAsync(token);
        }

        /// <inheritdoc />
        public async Task SetAsync(RefreshTokenInfo refreshToken)
        {
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.Token))
                return;

            await _cache.SetAsync(
                refreshToken.Token,
                refreshToken,
                slidingExpireTime: null,
                absoluteExpireTime: new DateTimeOffset(refreshToken.ExpireDate));
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return;

            await _cache.RemoveAsync(token);
        }
    }
}
