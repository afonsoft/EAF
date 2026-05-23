using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Configuration.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a interface IDynamicSettingsAppService.
    /// </summary>
    public interface IDynamicSettingsAppService : IApplicationService
    {
        string DynamicSettingsCacheKey();

        Task<PagedResultDto<SettingsDto>> GetAll(SettingsInputDto input);

        Task<SettingsDto> Get(string name);

        Task Set(SettingsDto input);

        Task Delete(SettingsDto input);
    }
}