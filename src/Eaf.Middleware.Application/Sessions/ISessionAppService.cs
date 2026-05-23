using Abp.Application.Services;
using Eaf.Middleware.Sessions.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Sessions
{
    /// <summary>
    /// Representa a interface ISessionAppService.
    /// </summary>
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}