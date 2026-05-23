using Eaf.Middleware.Authorization.Roles;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authorization
{
    public class StaticRoleNamesTests
    {
        [Fact]
        public void Host_Roles()
        {
            StaticRoleNames.Host.Admin.ShouldBe("Admin");
            StaticRoleNames.Host.User.ShouldBe("User");
        }

        [Fact]
        public void Tenant_Roles()
        {
            StaticRoleNames.Tenants.Admin.ShouldBe("Admin");
            StaticRoleNames.Tenants.User.ShouldBe("User");
        }
    }
}
