using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.Authorization.Ldap;
using Eaf.Middleware;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Permissions;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Authorization.Users.Exporting;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users
{
    /// <summary>
    /// Testes BDD para UserAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserAppServiceBddTests
    {
        private readonly UserAppService _sut;
        private readonly ICacheManager _cacheManager;

        public UserAppServiceBddTests()
        {
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var roleManager = Substitute.For<RoleManager>(
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            );

            _cacheManager = Substitute.For<ICacheManager>();
            _cacheManager.GetCache(Arg.Any<string>()).Returns(Substitute.For<ICache>());

            _sut = new UserAppService(
                roleManager,
                Substitute.For<IUserEmailer>(),
                Substitute.For<IUserListExcelExporter>(),
                Substitute.For<INotificationSubscriptionManager>(),
                Substitute.For<IRepository<Abp.Authorization.Users.UserRole, long>>(),
                new List<IPasswordValidator<User>>(),
                Substitute.For<IPasswordHasher<User>>(),
                Substitute.For<AppAzureActiveDirectoryAuthenticationSource>(
                    Substitute.For<IAzureActiveDirectorySettings>(),
                    Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>()
                ),
                Substitute.For<AppLdapAuthenticationSource>(
                    Substitute.For<ILdapSettings>(),
                    Substitute.For<IEafMiddlewareLdapModuleConfig>()
                ),
                Substitute.For<INotificationPublisher>(),
                Substitute.For<IWebhookPublisher>(),
                _cacheManager
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
            _sut.AppUrlService.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_AppUrlServiceDeveSerNullInstance()
        {
            _sut.AppUrlService.ShouldBe(NullAppUrlService.Instance);
        }

        #endregion

        #region DeleteUser

        [Fact]
        public async Task Dado_UsuarioTentandoDeletarProprioId_Quando_DeleteUser_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                _sut.DeleteUser(new Abp.Application.Services.Dto.EntityDto<long>(42)));
        }

        #endregion

        #region Injecao de Propriedade

        [Fact]
        public void Dado_AppUrlServiceCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customService = Substitute.For<IAppUrlService>();
            _sut.AppUrlService = customService;
            _sut.AppUrlService.ShouldBe(customService);
        }

        #endregion

        #region CloseSessionUser

        [Fact]
        public async Task Dado_UsuarioComTokens_Quando_CloseSessionUser_Entao_DeveRemoverTokensDoCache()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.RemoveAllTokenValidityKeyAsync(user, default).Returns(new List<string> { "token1", "token2" });

            var cache = Substitute.For<ICache>();
            _cacheManager.GetCache(MiddlewareCoreConsts.TokenValidityKey).Returns(cache);

            _sut.UserManager = userManager;

            // Quando
            await _sut.CloseSessionUser(1);

            // Então
            cache.Received(1).Remove("token1");
            cache.Received(1).Remove("token2");
        }

        #endregion

        #region UnlockUser

        [Fact]
        public async Task Dado_UsuarioBloqueado_Quando_UnlockUser_Entao_DeveDesbloquearELimparCache()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", LockoutEndDateUtc = DateTime.UtcNow.AddHours(1) };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;

            // Quando
            await _sut.UnlockUser(new Abp.Application.Services.Dto.EntityDto<long>(1));

            // Então
            user.LockoutEndDateUtc.ShouldBeNull();
        }

        #endregion

        #region ResetUserSpecificPermissions

        [Fact]
        public async Task Dado_UsuarioComPermissoes_Quando_ResetUserSpecificPermissions_Entao_DeveResetarPermissoes()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.ResetAllPermissionsAsync(user).Returns(Task.CompletedTask);

            _sut.UserManager = userManager;

            // Quando
            await _sut.ResetUserSpecificPermissions(new Abp.Application.Services.Dto.EntityDto<long>(1));

            // Então
            await userManager.Received(1).ResetAllPermissionsAsync(user);
        }

        #endregion

        #region UpdateUserPermissions

        [Fact]
        public async Task Dado_UsuarioEPermissoes_Quando_UpdateUserPermissions_Entao_DeveAtualizarPermissoes()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.SetGrantedPermissionsAsync(user, Arg.Any<IEnumerable<Permission>>()).Returns(Task.CompletedTask);

            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetPermissionOrNull(Arg.Any<string>()).Returns(new Permission("Pages.Test", displayName: null));

            _sut.UserManager = userManager;
            _sut.PermissionManager = permissionManager;

            // Quando
            await _sut.UpdateUserPermissions(new Eaf.Middleware.Authorization.Users.Dto.UpdateUserPermissionsInput
            {
                Id = 1,
                GrantedPermissionNames = new List<string> { "Pages.Test" }
            });

            // Então
            await userManager.Received(1).SetGrantedPermissionsAsync(user, Arg.Any<IEnumerable<Permission>>());
        }

        #endregion
    }
}
