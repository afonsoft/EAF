using Abp.Application.Services;
using Abp.Domain.Repositories;
using Eaf.MiddlewareCore.SampleApp.Core;

namespace Eaf.MiddlewareCore.SampleApp.Application.Users
{
    public class UserAppService : AsyncCrudAppService<User, UserDto, long>, IUserAppService
    {
        public UserAppService(IRepository<User, long> repository)
            : base(repository)
        {
        }
    }
}