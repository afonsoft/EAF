using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Maintenance.Caching.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Maintenance.Caching
{
    /// <summary>
    /// Representa a interface ICachingAppService.
    /// </summary>
    public interface ICachingAppService : IApplicationService
    {
        Task ClearAllCaches();

        Task ClearCache(EntityDto<string> input);

        ListResultDto<CacheDto> GetAllCaches();
    }
}