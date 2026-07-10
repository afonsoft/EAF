using Abp;
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Abp.Domain.Repositories;
using Abp.Authorization.Users;

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

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_GetUserByLoginAsync_Entao_DeveRetornarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "user1", NormalizedUserName = "USER1" };
            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);

            // Quando
            var result = await userManager.GetUserByLoginAsync("user1", null);

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_GetUserByLoginAsync_Entao_DeveRetornarNulo()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns((User)null!);

            // Quando
            var result = await userManager.GetUserByLoginAsync("missing", null);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_GetUserAsync_Entao_DeveRetornarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "user1" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns((User?)user);
            userManager.When(x => x.GetUserOrNullAsync(Arg.Any<UserIdentifier>())).CallBase();

            // Quando
            var result = await userManager.GetUserAsync(new UserIdentifier(null, 1));

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_GetUserAsync_Entao_DeveLancarExcecao()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns((User?)null);
            userManager.When(x => x.GetUserOrNullAsync(Arg.Any<UserIdentifier>())).CallBase();

            // Quando/Então
            var exception = await Should.ThrowAsync<Exception>(() => userManager.GetUserAsync(new UserIdentifier(null, 1)));
            exception.Message.ShouldContain("There is no user");
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_GetUserOrNullAsync_Entao_DeveRetornarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "user1" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns((User?)user);
            userManager.When(x => x.GetUserOrNullAsync(Arg.Any<UserIdentifier>())).CallBase();

            // Quando
            var result = await userManager.GetUserOrNullAsync(new UserIdentifier(null, 1));

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_GetUserByLoginAsyncComTenant_Entao_DeveRetornarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "user1", NormalizedUserName = "USER1" };
            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(user);

            // Quando
            var result = await userManager.GetUserByLoginAsync("user1", 1);

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_UsuarioValido_Quando_UpdateWithValidateAsync_Entao_DeveRetornarSucesso()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", EmailAddress = "admin@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.CheckDuplicateUsernameOrEmailAddressAsync(
                Arg.Any<long?>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>()).Returns(IdentityResult.Success);

            // Quando
            var result = await userManager.UpdateWithValidateAsync(user);

            // Então
            result.Succeeded.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_EmailDuplicadoMesmoAuthSource_Quando_CheckDuplicateUsernameOrEmailAddressAsync_Entao_DeveRetornarFalhaCodigo2()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var existing = new User { Id = 2, UserName = "other", EmailAddress = "user1@example.com", AuthenticationSource = "LDAP" };

            userManager.FindByNameAsync("user1").Returns(Task.FromResult<User?>(null));
            userManager.FindByEmailAsync("user1@example.com").Returns(Task.FromResult<User?>(existing));
            userManager.When(x => x.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())).CallBase();

            var result = await userManager.CheckDuplicateUsernameOrEmailAddressAsync(1, "user1", "user1@example.com", "LDAP");

            result.Succeeded.ShouldBeFalse();
            result.Errors.First().Code.ShouldBe("2");
        }

        [Fact]
        public async Task Dado_UsuarioComTokens_Quando_RemoveAllTokenValidityKeyAsync_Entao_DeveRetornarTokensRemovidos()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            user.Tokens = new List<UserToken>
            {
                CreateToken(1, "TokenValidityKeyProvider", "token1"),
                CreateToken(1, "TokenValidityKeyProvider", "token2"),
                CreateToken(1, "OtherProvider", "token3")
            };

            static UserToken CreateToken(long userId, string loginProvider, string name)
            {
                var token = Substitute.For<UserToken>();
                token.UserId.Returns(userId);
                token.LoginProvider.Returns(loginProvider);
                token.Name.Returns(name);
                return token;
            }

            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userManager.When(x => x.RemoveAllTokenValidityKeyAsync(user, default)).CallBase();
            userRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            // Quando
            var result = await userManager.RemoveAllTokenValidityKeyAsync(user, default);

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldContain("token1");
            result.ShouldContain("token2");
        }
    }
}
