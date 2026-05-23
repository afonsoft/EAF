using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Common.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Common
{
    /// <summary>
    /// Representa a interface ICommonLookupAppService.
    /// </summary>
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);
    }
}