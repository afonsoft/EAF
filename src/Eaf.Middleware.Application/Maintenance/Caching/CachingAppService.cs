using Abp.Application.Services.Dto;
using Abp.Authorization;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Maintenance.Caching.Dto;
using Abp.Runtime.Caching;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Maintenance.Caching
{
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Maintenance)]
    public class CachingAppService : MiddlewareAppServiceBase, ICachingAppService
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// CachingAppService.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public CachingAppService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// ClearAllCaches.
        /// </summary>
        public async Task ClearAllCaches()
        {
            var caches = _cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }
        }

        /// <summary>
        /// ClearCache.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task ClearCache(EntityDto<string> input)
        {
            var cache = _cacheManager.GetCache(input.Id);
            await cache.ClearAsync();
        }

        /// <summary>
        /// GetAllCaches.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public ListResultDto<CacheDto> GetAllCaches()
        {
            var caches = _cacheManager.GetAllCaches()
                                        .Select(cache => new CacheDto
                                        {
                                            Name = cache.Name
                                        })
                                        .ToList();

            return new ListResultDto<CacheDto>(caches);
        }
    }
}