using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a interface IGoogleAppService.
    /// </summary>
    public interface IGoogleAppService : IApplicationService
    {
        [Produces("application/json", "application/json-patch+json", "text/json")]
        Task<string> GetAnalytics();

        [Produces("application/json", "application/json-patch+json", "text/json")]
        Task<string> GetRecaptchaSiteKey();

        [Produces("application/json", "application/json-patch+json", "text/json")]
        Task<string> GetTagManager();
    }
}