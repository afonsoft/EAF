using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Eaf.MiddlewareCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using System;
using Xunit;


namespace Eaf.Middleware
{
    public class DependencyInjection_Tests : EafMiddlewareTestBase
    {
        [Fact]
        public void Should_Resolve_FeatureValueStore()
        {
            LocalIocManager.Resolve<IFeatureValueStore>();
            LocalIocManager.Resolve<AbpFeatureValueStore<Tenant, User>>();
            LocalIocManager.Resolve<FeatureValueStore>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_LazyRoleStore()
        {
            LocalIocManager.Resolve<Lazy<RoleStore>>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_LoginManager()
        {
            LocalIocManager.Resolve<AbpLogInManager<Tenant, Role, User>>();
            LocalIocManager.Resolve<LogInManager>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_PermissionChecker()
        {
            LocalIocManager.Resolve<IPermissionChecker>();
            LocalIocManager.Resolve<PermissionChecker<Role, User>>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_RoleManager()
        {
            LocalIocManager.Resolve<RoleManager<Role>>();
            LocalIocManager.Resolve<AbpRoleManager<Role, User>>();
            LocalIocManager.Resolve<RoleManager>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_RoleStore()
        {
            LocalIocManager.Resolve<IRoleStore<Role>>();
            LocalIocManager.Resolve<AbpRoleStore<Role, User>>();
            LocalIocManager.Resolve<RoleStore>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_SecurityStampValidator()
        {
            LocalIocManager.Resolve<AbpSecurityStampValidator<Tenant, Role, User>>();
            LocalIocManager.Resolve<SecurityStampValidator<User>>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_SignInManager()
        {
            LocalIocManager.Resolve<SignInManager<User>>();
            LocalIocManager.Resolve<AbpSignInManager<Tenant, Role, User>>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_TenantManager()
        {
            LocalIocManager.Resolve<AbpTenantManager<Tenant, User>>();
            LocalIocManager.Resolve<TenantManager>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_UserClaimsPrincipalFactory()
        {
            LocalIocManager.Resolve<UserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<AbpUserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<IUserClaimsPrincipalFactory<User>>();
            LocalIocManager.Resolve<UserClaimsPrincipalFactory>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_UserManager()
        {
            LocalIocManager.Resolve<UserManager<User>>();
            LocalIocManager.Resolve<AbpUserManager<Role, User>>();
            LocalIocManager.Resolve<UserManager>();
            Assert.NotNull(LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_UserStore()
        {
            LocalIocManager.Resolve<IUserStore<User>>();
            LocalIocManager.Resolve<AbpUserStore<Role, User>>();
            LocalIocManager.Resolve<UserStore>();
            Assert.NotNull(LocalIocManager);
        }
    }
}