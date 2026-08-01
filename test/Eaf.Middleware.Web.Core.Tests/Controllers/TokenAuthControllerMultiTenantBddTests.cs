using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Web.Controllers;
using Eaf.Middleware.Web.Core.Tests.Identity;
using Eaf.Middleware.Web.Models.TokenAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public partial class TokenAuthControllerBddTests
    {
        [Fact]
        public async Task Dado_UsuarioHostComMemberships_Quando_GetAvailableTenants_Entao_DeveRetornarListaDeTenants()
        {
            // Dado
            var hostUser = IdentityTestHelper.CreateUser("admin", 1, null, "host-stamp");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, hostUser.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>((Eaf.Middleware.MultiTenancy.Tenant?)null, hostUser, identity);

            var userManager = CriarUserManagerSubstituto(hostUser);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var tenantUserManager = Substitute.For<ITenantUserManager>();
            tenantUserManager.GetMembershipsAsync(hostUser.Id).Returns(Task.FromResult<IReadOnlyList<UserTenantMembership>>(new List<UserTenantMembership>
            {
                new UserTenantMembership { UserId = hostUser.Id, TenantId = 1, TenantUserId = 2, IsDefault = true }
            }));

            var iocManager = Substitute.For<IIocManager>();
            iocManager.Resolve<ITenantUserManager>().Returns(tenantUserManager);

            var controller = CriarController(userManager, roleManager, logInManager, iocManager: iocManager);
            controller.AbpSession = CriarAbpSession(hostUser);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);
            ConfigurarTenantCache(controller, "Default");

            // Quando
            var result = await controller.GetAvailableTenants(new AvailableTenantsModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].TenantId.ShouldBe(1);
            result[0].TenancyName.ShouldBe("Default");
            result[0].IsDefault.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioDeTenant_Quando_GetAvailableTenants_Entao_DeveLancarExcecao()
        {
            // Dado
            var tenantUser = IdentityTestHelper.CreateUser("admin", 1, 1, "tenant-stamp");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, tenantUser.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, tenantUser, identity);

            var userManager = CriarUserManagerSubstituto(tenantUser);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(tenantUser);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.GetAvailableTenants(new AvailableTenantsModel
                {
                    UserNameOrEmailAddress = "admin",
                    Password = "password"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioHostComMembership_Quando_SelectTenant_Entao_DeveRetornarAccessToken()
        {
            // Dado
            var hostUser = IdentityTestHelper.CreateUser("admin", 1, null, "host-stamp");
            var shadowUser = IdentityTestHelper.CreateUser("admin", 2, 1, "shadow-stamp");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, hostUser.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>((Eaf.Middleware.MultiTenancy.Tenant?)null, hostUser, identity);

            var userManager = CriarUserManagerSubstituto(hostUser);
            userManager.FindByIdAsync("2").Returns(shadowUser);

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var tenantUserManager = Substitute.For<ITenantUserManager>();
            tenantUserManager.GetMembershipsAsync(hostUser.Id).Returns(Task.FromResult<IReadOnlyList<UserTenantMembership>>(new List<UserTenantMembership>
            {
                new UserTenantMembership
                {
                    UserId = hostUser.Id,
                    TenantId = 1,
                    TenantUserId = shadowUser.Id,
                    IsDefault = true
                }
            }));

            var principalFactory = Substitute.For<UserClaimsPrincipalFactory>(
                userManager,
                roleManager,
                Options.Create(new IdentityOptions()),
                IdentityTestHelper.CreateUnitOfWorkManager()
            );
            principalFactory.CreateAsync(shadowUser).Returns(Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, shadowUser.Id.ToString())
            }))));

            var iocManager = Substitute.For<IIocManager>();
            iocManager.Resolve<ITenantUserManager>().Returns(tenantUserManager);
            iocManager.Resolve<UserClaimsPrincipalFactory>().Returns(principalFactory);

            var controller = CriarController(userManager, roleManager, logInManager, iocManager: iocManager);
            controller.AbpSession = CriarAbpSession(hostUser);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);
            ConfigurarTenantCache(controller, "Default");

            // Quando
            var result = await controller.SelectTenant(new SelectTenantModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password",
                TenantId = 1
            });

            // Então
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(shadowUser.Id);
        }
    }
}
