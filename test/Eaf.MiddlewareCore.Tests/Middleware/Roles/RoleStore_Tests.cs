using Abp.Domain.Uow;
using Eaf.MiddlewareCore.SampleApp.Core;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Roles
{
    public class RoleStore_Tests : EafMiddlewareTestBase
    {
        private readonly RoleStore _roleStore;

        public RoleStore_Tests()
        {
            _roleStore = Resolve<RoleStore>();
        }

        [Fact]
        public async Task Should_Get_Role_Claims()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var role = await _roleStore.FindByNameAsync("ADMIN", default);
                role.ShouldNotBeNull();

                var claims = await _roleStore.GetClaimsAsync(role);

                claims.ShouldNotBeNull();

                await uow.CompleteAsync();
            }
        }
    }
}