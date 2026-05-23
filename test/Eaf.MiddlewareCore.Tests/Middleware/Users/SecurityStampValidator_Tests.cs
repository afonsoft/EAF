using Abp.Authorization;
using Eaf.MiddlewareCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Users
{
    public class SecurityStampValidator_Tests : EafMiddlewareTestBase
    {
        [Fact]
        public void Should_Resolve_EafSecurityStampValidator()
        {
            (Resolve<ISecurityStampValidator>() is AbpSecurityStampValidator<Tenant, Role, User>).ShouldBeTrue();
            (Resolve<SecurityStampValidator<User>>() is AbpSecurityStampValidator<Tenant, Role, User>).ShouldBeTrue();
        }
    }
}