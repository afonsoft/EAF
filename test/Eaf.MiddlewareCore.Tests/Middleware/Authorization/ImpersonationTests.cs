using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Shouldly;
using System.Security.Claims;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authorization
{
    public class ImpersonationTests
    {
        [Fact]
        public void EafUserToken_CanBeCreated()
        {
            var t = new EafUserToken();
            t.ShouldNotBeNull();
        }

        [Fact]
        public void ImpersonationCacheItem_DefaultCtor()
        {
            var item = new ImpersonationCacheItem();
            item.ImpersonatorTenantId.ShouldBeNull();
            item.ImpersonatorUserId.ShouldBe(0);
            item.IsBackToImpersonator.ShouldBeFalse();
            item.TargetTenantId.ShouldBeNull();
            item.TargetUserId.ShouldBe(0);
        }

        [Fact]
        public void ImpersonationCacheItem_ParameterizedCtor()
        {
            var item = new ImpersonationCacheItem(1, 10, true);
            item.TargetTenantId.ShouldBe(1);
            item.TargetUserId.ShouldBe(10);
            item.IsBackToImpersonator.ShouldBeTrue();
        }

        [Fact]
        public void ImpersonationCacheItem_CacheName_IsConstant()
        {
            ImpersonationCacheItem.CacheName.ShouldBe("AppImpersonationCache");
        }

        [Fact]
        public void UserAndIdentity_Ctor_SetsProperties()
        {
            var user = new User { Name = "n" };
            var identity = new ClaimsIdentity();
            var ui = new UserAndIdentity(user, identity);
            ui.User.ShouldBe(user);
            ui.Identity.ShouldBe(identity);
        }
    }
}
