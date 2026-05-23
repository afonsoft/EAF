using Abp.Application.Services;
using Eaf.Middleware.Configuration.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a interface IUiCustomizationSettingsAppService.
    /// </summary>
    public interface IUiCustomizationSettingsAppService : IApplicationService
    {
        Task<List<ThemeSettingsDto>> GetUiManagementSettings();

        Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings);

        Task UpdateUiManagementSettings(ThemeSettingsDto settings);

        Task UseSystemDefaultSettings();
    }
}