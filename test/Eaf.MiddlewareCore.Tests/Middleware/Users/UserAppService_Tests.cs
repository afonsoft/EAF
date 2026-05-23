using Abp.Application.Services.Dto;
using Eaf.MiddlewareCore.SampleApp.Application.Users;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Users
{
    public class UserAppService_Tests : EafMiddlewareTestBase
    {
        private readonly IUserAppService _userAppService;

        public UserAppService_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
        }

        [Fact]
        public async Task Should_Get_All_Users()
        {
            var users = await _userAppService.GetAllAsync(new PagedAndSortedResultRequestDto());
            users.TotalCount.ShouldBeGreaterThan(0);
            users.Items.Count.ShouldBeGreaterThan(0);
        }
    }
}