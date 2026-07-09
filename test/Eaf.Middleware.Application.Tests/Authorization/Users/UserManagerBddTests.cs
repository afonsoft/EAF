using Abp.Authorization;
using Abp.UI;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserManagerBddTests
    {
        [Fact]
        public async Task Dado_UsuarioAdmin_Quando_SetRolesAsyncSemRoleAdmin_Entao_DeveLancarExcecao()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var admin = new User { UserName = "admin", Name = "admin" };

            userManager.When(x => x.SetRolesAsync(Arg.Any<User>(), Arg.Any<string[]>())).CallBase();

            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await userManager.SetRolesAsync(admin, new[] { StaticRoleNames.Host.User })
            );

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioAdmin_Quando_SetGrantedPermissionsAsyncSemPermissoesNecessarias_Entao_DeveLancarExcecao()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var admin = new User { UserName = "admin", Name = "admin" };
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Roles_Edit, displayName: null)
            };

            userManager.When(x => x.SetGrantedPermissionsAsync(Arg.Any<User>(), Arg.Any<IEnumerable<Permission>>())).CallBase();

            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await userManager.SetGrantedPermissionsAsync(admin, permissions)
            );

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioComPermissaoIncompleta_Quando_SetGrantedPermissionsAsync_Entao_DeveLancarExcecao()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var admin = new User { UserName = "admin", Name = "admin" };
            var permissions = new List<Permission>
            {
                new Permission(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions, displayName: null)
            };

            userManager.When(x => x.SetGrantedPermissionsAsync(Arg.Any<User>(), Arg.Any<IEnumerable<Permission>>())).CallBase();

            var exception = await Should.ThrowAsync<UserFriendlyException>(
                async () => await userManager.SetGrantedPermissionsAsync(admin, permissions)
            );

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioComum_Quando_CheckDuplicateUsernameOrEmailAddressAsync_Entao_DeveRetornarSucesso()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var user = new User { Id = 1, UserName = "user1", EmailAddress = "user1@example.com" };

            userManager.FindByNameAsync("user1").Returns(Task.FromResult<User?>(null));
            userManager.FindByEmailAsync("user1@example.com").Returns(Task.FromResult<User?>(null));
            userManager.When(x => x.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())).CallBase();

            var result = await userManager.CheckDuplicateUsernameOrEmailAddressAsync(1, "user1", "user1@example.com", null);

            result.Succeeded.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioComNomeDuplicado_Quando_CheckDuplicateUsernameOrEmailAddressAsync_Entao_DeveRetornarFalha()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var existing = new User { Id = 2, UserName = "user1", EmailAddress = "other@example.com" };

            userManager.FindByNameAsync("user1").Returns(Task.FromResult<User?>(existing));
            userManager.When(x => x.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())).CallBase();

            var result = await userManager.CheckDuplicateUsernameOrEmailAddressAsync(1, "user1", "user1@example.com", null);

            result.Succeeded.ShouldBeFalse();
            result.Errors.First().Code.ShouldBe("1");
        }
    }
}
