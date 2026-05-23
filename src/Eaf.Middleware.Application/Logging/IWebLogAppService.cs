using Abp.Application.Services;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Logging.Dto;

namespace Eaf.Middleware.Logging
{
    /// <summary>
    /// Representa a interface IWebLogAppService.
    /// </summary>
    public interface IWebLogAppService : IApplicationService
    {
        FileDto DownloadWebLogs();

        GetLatestWebLogsOutput GetLatestWebLogs();
    }
}