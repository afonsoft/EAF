using Abp.Application.Services;
using Eaf.Middleware.Configuration.Host.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration.Host
{
    /// <summary>
    /// Representa a interface IHostSettingsAppService.
    /// </summary>
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettingsAnonymous();

        Task<HostSettingsEditDto> GetAllSettings();

        Task SendTestEmail(SendTestEmailInput input);

        Task UpdateAllSettings(HostSettingsEditDto input);
    }
}