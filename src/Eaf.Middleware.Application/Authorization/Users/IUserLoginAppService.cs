using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Representa a interface IUserLoginAppService.
    /// </summary>
    public interface IUserLoginAppService : IApplicationService
    {
        Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts();
    }
}