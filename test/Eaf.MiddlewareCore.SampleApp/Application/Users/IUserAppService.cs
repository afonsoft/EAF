using Abp.Application.Services;

namespace Eaf.MiddlewareCore.SampleApp.Application.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long>
    {
    }
}