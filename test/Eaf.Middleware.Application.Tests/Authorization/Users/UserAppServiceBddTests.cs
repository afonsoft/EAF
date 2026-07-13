using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Notifications;
using Abp.ObjectMapping;
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
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Authorization.Permissions;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
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
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly RoleManager _roleManager;
        private readonly IRepository<Abp.Authorization.Users.UserRole, long> _userRoleRepository;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly ICache _usersCache;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AppLdapAuthenticationSource _appLdapAuthenticationSource;
        private readonly AppAzureActiveDirectoryAuthenticationSource _appAzureActiveDirectoryAuthenticationSource;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserAppServiceBddTests()
        {
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            _roleManager = Substitute.For<RoleManager>(
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
            _usersCache = Substitute.For<ICache>();
            _cacheManager.GetCache(Arg.Any<string>()).Returns(_usersCache);

            _userRoleRepository = Substitute.For<IRepository<Abp.Authorization.Users.UserRole, long>>();
            _userRoleRepository.GetAllAsync().Returns(_ => Task.FromResult(_userRoleRepository.GetAll()));
            _userListExcelExporter = Substitute.For<IUserListExcelExporter>();

            _passwordValidator = Substitute.For<IPasswordValidator<User>>();
            _passwordValidator.ValidateAsync(Arg.Any<UserManager>(), Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);

            _passwordHasher = Substitute.For<IPasswordHasher<User>>();
            _passwordHasher.HashPassword(Arg.Any<User>(), Arg.Any<string>()).Returns("hashed");

            _appAzureActiveDirectoryAuthenticationSource = Substitute.For<AppAzureActiveDirectoryAuthenticationSource>(
                Substitute.For<IAzureActiveDirectorySettings>(),
                Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>()
            );

            _appLdapAuthenticationSource = Substitute.For<AppLdapAuthenticationSource>(
                Substitute.For<ILdapSettings>(),
                Substitute.For<IEafMiddlewareLdapModuleConfig>()
            );

            _unitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();

            _sut = new UserAppService(
                _roleManager,
                Substitute.For<IUserEmailer>(),
                _userListExcelExporter,
                Substitute.For<INotificationSubscriptionManager>(),
                _userRoleRepository,
                new List<IPasswordValidator<User>> { _passwordValidator },
                _passwordHasher,
                _appAzureActiveDirectoryAuthenticationSource,
                _appLdapAuthenticationSource,
                Substitute.For<INotificationPublisher>(),
                Substitute.For<IWebhookPublisher>(),
                _cacheManager
            )
            {
                UnitOfWorkManager = _unitOfWorkManager
            };
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

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_DeleteUser_Entao_DeveDeletarELimparCache()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            _sut.AbpSession = abpSession;

            var user = new User { Id = 2, UserName = "user2" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);
            userManager.DeleteAsync(user).Returns(IdentityResult.Success);

            _sut.UserManager = userManager;

            // Quando
            await _sut.DeleteUser(new Abp.Application.Services.Dto.EntityDto<long>(2));

            // Então
            await userManager.Received(1).DeleteAsync(user);
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
            await cache.Received(1).RemoveAsync("token1");
            await cache.Received(1).RemoveAsync("token2");
        }

        [Fact]
        public async Task Dado_UsuarioInexistente_Quando_CloseSessionUser_Entao_DeveLancarArgumentNullException()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(99).Returns((User)null);
            _sut.UserManager = userManager;

            await Should.ThrowAsync<ArgumentNullException>(() => _sut.CloseSessionUser(99));
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

        #region GetUserPermissionsForEdit

        [Fact]
        public async Task Dado_UsuarioEPermissoes_Quando_GetUserPermissionsForEdit_Entao_DeveRetornarPermissoesMapeadas()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.GetGrantedPermissionsAsync(user).Returns(new List<Permission>());

            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetAllPermissionsAsync().Returns(new List<Permission> { new Permission("Pages.Test", displayName: null) });

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<FlatPermissionDto>>(Arg.Any<object>()).Returns(new List<FlatPermissionDto> { new FlatPermissionDto { Name = "Pages.Test" } });

            _sut.UserManager = userManager;
            _sut.PermissionManager = permissionManager;
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetUserPermissionsForEdit(new Abp.Application.Services.Dto.EntityDto<long>(1));

            // Então
            result.ShouldNotBeNull();
            result.Permissions.Count.ShouldBe(1);
            result.Permissions[0].Name.ShouldBe("Pages.Test");
        }

        #endregion

        #region GetUserForEdit

        [Fact]
        public async Task Dado_NovoUsuario_Quando_GetUserForEdit_Entao_DeveRetornarUsuarioPadraoERolesPadroesMarcados()
        {
            // Dado
            var roles = new List<Role>
            {
                new Role(null, "admin", "Admin") { Id = 1, IsDefault = true },
                new Role(null, "user", "User") { Id = 2, IsDefault = false }
            }.AsAsyncQueryable();
            _roleManager.Roles.Returns(roles);

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns("False");
            _sut.SettingManager = settingManager;

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<UserEditDto>(Arg.Any<User>()).Returns(new UserEditDto());
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetUserForEdit(new NullableIdDto<long>());

            // Então
            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.IsActive.ShouldBeTrue();
            result.Roles.ShouldNotBeEmpty();
            result.Roles.ShouldContain(r => r.RoleName == "admin" && r.IsAssigned);
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_GetUserForEdit_Entao_DeveRetornarUsuarioMapeadoERolesAtribuidos()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.IsInRoleAsync(user, Arg.Any<string>()).Returns(true);

            var roles = new List<Role>
            {
                new Role(null, "admin", "Admin") { Id = 1, IsDefault = true }
            }.AsAsyncQueryable();
            _roleManager.Roles.Returns(roles);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<UserEditDto>(Arg.Any<User>()).Returns(new UserEditDto { UserName = "admin" });
            _sut.ObjectMapper = objectMapper;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetUserForEdit(new NullableIdDto<long> { Id = 1 });

            // Então
            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.UserName.ShouldBe("admin");
            result.Roles.ShouldContain(r => r.RoleName == "admin" && r.IsAssigned);
        }

        #endregion

        #region GetActiveDirectoryUsers

        [Fact]
        public async Task Dado_FonteAzureDesabilitada_Quando_GetActiveDirectoryUsers_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>());
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetActiveDirectoryUsers("john");

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        #endregion

        #region GetLdapUsers

        [Fact]
        public async Task Dado_NomeVazio_Quando_GetLdapUsers_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>());
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetLdapUsers("");

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        #endregion

        #region CreateUsersByActiveDirectory

        [Fact]
        public async Task Dado_ListaDeUserNamesVazia_Quando_CreateUsersByActiveDirectory_Entao_DeveRetornarSemErro()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new string[] { },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            // Quando
            await _sut.CreateUsersByActiveDirectory(input);

            // Então
            _sut.AbpSession.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UserNameVazioNaLista_Quando_CreateUsersByActiveDirectory_Entao_DeveIgnorarUserNameVazio()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new[] { "" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            await _sut.CreateUsersByActiveDirectory(input);

            _sut.AbpSession.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UserNameJaExistente_Quando_CreateUsersByActiveDirectory_Entao_DeveIgnorarUsuarioExistente()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(new User { Id = 2, UserName = "existing", NormalizedUserName = "EXISTING" });
            _sut.UserManager = userManager;

            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new[] { "existing" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            await _sut.CreateUsersByActiveDirectory(input);

            await userRepository.Received(1).FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>());
            await _appAzureActiveDirectoryAuthenticationSource.DidNotReceiveWithAnyArgs().GetUserAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_UserNameComDominioJaExistente_Quando_CreateUsersByActiveDirectory_Entao_DeveIgnorarUsuarioExistente()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(new User { Id = 3, UserName = "existing", NormalizedUserName = "EXISTING" });
            _sut.UserManager = userManager;

            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new[] { "existing@tenant.com" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            await _sut.CreateUsersByActiveDirectory(input);

            await userRepository.Received(1).FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>());
        }

        [Fact]
        public async Task Dado_UserNameNovoDoAzureAD_Quando_CreateUsersByActiveDirectory_Entao_DeveCriarUsuarioComRoles()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns((User)null);
            userManager.CreateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            _appAzureActiveDirectoryAuthenticationSource.GetUserAsync("user@tenant.com")
                .Returns(new User { UserName = "user", Name = "", Surname = "", EmailAddress = "" });

            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new[] { "user@tenant.com" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            // Quando
            await _sut.CreateUsersByActiveDirectory(input);

            // Então
            await _appAzureActiveDirectoryAuthenticationSource.Received(1).GetUserAsync("user@tenant.com");
            await userManager.Received(1).CreateAsync(Arg.Any<User>());
        }

        #endregion

        #region CreateUsersByLdap

        [Fact]
        public async Task Dado_ListaDeUserNamesVazia_Quando_CreateUsersByLdap_Entao_DeveRetornarSemErro()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var input = new CreateLdapUserInput
            {
                UserNames = new string[] { },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            // Quando
            await _sut.CreateUsersByLdap(input);

            // Então
            _sut.AbpSession.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UserNamesLdapValidos_Quando_CreateUsersByLdap_Entao_DeveCriarUsuariosDoLdap()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var user = new User { Id = 1, UserName = "john", Name = "John", Surname = "User", EmailAddress = "john@example.com" };
            _appLdapAuthenticationSource.CreateUserAsync("john", Arg.Any<Tenant>()).Returns(user);

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.CreateAsync(user).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var input = new CreateLdapUserInput
            {
                UserNames = new[] { "john" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            await _sut.CreateUsersByLdap(input);

            await _appLdapAuthenticationSource.Received(1).CreateUserAsync("john", Arg.Any<Tenant>());
            await userManager.Received(1).CreateAsync(user);
        }

        [Fact]
        public async Task Dado_UserNamesLdapComVazioEExistente_Quando_CreateUsersByLdap_Entao_DeveIgnorar()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var userManager = ManagerTestHelper.CreateUserManager(out var userRepository);
            userRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>())
                .Returns(new User { Id = 2, UserName = "existing", NormalizedUserName = "EXISTING" });
            userManager.CreateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            var input = new CreateLdapUserInput
            {
                UserNames = new[] { "", "existing" },
                AssignedRoleNames = new[] { "admin" },
                IsActive = true
            };

            await _sut.CreateUsersByLdap(input);

            await _appLdapAuthenticationSource.DidNotReceiveWithAnyArgs().CreateUserAsync(Arg.Any<string>(), Arg.Any<Tenant>());
            await userManager.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
        }

        #endregion

        #region CreateOrUpdateUser


        [Fact]
        public async Task Dado_NovoUsuario_Quando_CreateOrUpdateUser_Entao_DeveCriarUsuarioLimparCacheENotificar()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());
            currentUow.SaveChangesAsync().Returns(Task.CompletedTask);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var newUser = new User { Id = 0, UserName = "newuser", Name = "New", Surname = "User", EmailAddress = "new@example.com" };
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<User>(Arg.Any<object>()).Returns(newUser);
            _sut.ObjectMapper = objectMapper;

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.CreateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    UserName = "newuser",
                    Name = "New",
                    Surname = "User",
                    EmailAddress = "new@example.com",
                    IsActive = true
                },
                AssignedRoleNames = new[] { "admin" },
                SetRandomPassword = true,
                SendActivationEmail = false
            };

            // Quando
            await _sut.CreateOrUpdateUser(input);

            // Então
            await userManager.Received(1).CreateAsync(Arg.Any<User>());
            await currentUow.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_CreateOrUpdateUser_Entao_DeveAtualizarUsuarioRolesCache()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var existingUser = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(existingUser);
            userManager.ChangePasswordAsync(existingUser, Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.SetRolesAsync(existingUser, Arg.Any<string[]>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            var objectMapper = Substitute.For<IObjectMapper>();
            _sut.ObjectMapper = objectMapper;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Id = 1,
                    UserName = "admin",
                    Name = "Admin",
                    Surname = "User",
                    EmailAddress = "admin@example.com",
                    IsActive = true,
                    Password = ""
                },
                AssignedRoleNames = new[] { "admin" },
                SetRandomPassword = false,
                SendActivationEmail = false
            };

            // Quando
            await _sut.CreateOrUpdateUser(input);

            // Então
            await userManager.Received(1).CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            await userManager.Received(1).SetRolesAsync(existingUser, Arg.Any<string[]>());
        }

        #endregion

        #region CreateOrUpdateUser

        [Fact]
        public async Task Dado_NovoUsuarioComEmailAtivacao_Quando_CreateOrUpdateUser_Entao_DeveCriarEEnviarEmail()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());
            currentUow.SaveChangesAsync().Returns(Task.CompletedTask);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var newUser = new User { Id = 0, UserName = "newuser", Name = "New", Surname = "User", EmailAddress = "new@example.com" };
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<User>(Arg.Any<object>()).Returns(newUser);
            _sut.ObjectMapper = objectMapper;

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.CreateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreateEmailActivationUrlFormat(Arg.Any<int?>()).Returns("https://localhost/activate");
            _sut.AppUrlService = appUrlService;

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    UserName = "newuser",
                    Name = "New",
                    Surname = "User",
                    EmailAddress = "new@example.com",
                    IsActive = true,
                    Password = "Password123!"
                },
                AssignedRoleNames = new[] { "admin" },
                SetRandomPassword = false,
                SendActivationEmail = true
            };

            // Quando
            await _sut.CreateOrUpdateUser(input);

            // Então
            await userManager.Received(1).CreateAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task Dado_UsuarioExistenteComEmailAtivacao_Quando_CreateOrUpdateUser_Entao_DeveAtualizarEEnviarEmail()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var existingUser = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(existingUser);
            userManager.ChangePasswordAsync(existingUser, Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.SetRolesAsync(existingUser, Arg.Any<string[]>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            var objectMapper = Substitute.For<IObjectMapper>();
            _sut.ObjectMapper = objectMapper;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreateEmailActivationUrlFormat(Arg.Any<int?>()).Returns("https://localhost/activate");
            _sut.AppUrlService = appUrlService;

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Id = 1,
                    UserName = "admin",
                    Name = "Admin",
                    Surname = "User",
                    EmailAddress = "admin@example.com",
                    IsActive = true,
                    Password = "Password123!"
                },
                AssignedRoleNames = new[] { "admin" },
                SetRandomPassword = false,
                SendActivationEmail = true
            };

            // Quando
            await _sut.CreateOrUpdateUser(input);

            // Então
            await userManager.Received(1).CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            await userManager.Received(1).SetRolesAsync(existingUser, Arg.Any<string[]>());
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_CreateOrUpdateUserComSenhaAleatoriaEEAtivacao_Entao_DeveAtualizarSenhaEEnviarEmail()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var existingUser = new User { Id = 1, UserName = "john", Name = "John", Surname = "User", EmailAddress = "john@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(existingUser);
            userManager.CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.SetRolesAsync(existingUser, Arg.Any<string[]>()).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            var objectMapper = Substitute.For<IObjectMapper>();
            _sut.ObjectMapper = objectMapper;

            _roleManager.GetRoleByNameAsync("user").Returns(new Role(null, "user", "User") { Id = 1 });

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreateEmailActivationUrlFormat(Arg.Any<int?>()).Returns("https://localhost/activate");
            _sut.AppUrlService = appUrlService;

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    Id = 1,
                    UserName = "john",
                    Name = "John",
                    Surname = "User",
                    EmailAddress = "john@example.com",
                    IsActive = true,
                    Password = ""
                },
                AssignedRoleNames = new[] { "user" },
                SetRandomPassword = true,
                SendActivationEmail = true
            };

            // Quando
            await _sut.CreateOrUpdateUser(input);

            // Então
            await userManager.Received(1).CheckDuplicateUsernameOrEmailAddressAsync(Arg.Any<long?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            await userManager.Received(1).SetRolesAsync(existingUser, Arg.Any<string[]>());
        }

        [Fact]
        public async Task Dado_NovoUsuarioComSenhaValida_Quando_CreateOrUpdateUser_Entao_DeveValidarSenhaEHashSenha()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var newUser = new User { Id = 0, UserName = "newuser", Name = "New", Surname = "User", EmailAddress = "new@example.com" };
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<User>(Arg.Any<object>()).Returns(newUser);
            _sut.ObjectMapper = objectMapper;

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.CreateAsync(newUser).Returns(IdentityResult.Success);
            _sut.UserManager = userManager;

            _roleManager.GetRoleByNameAsync("admin").Returns(new Role(null, "admin", "Admin") { Id = 1 });

            var input = new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    UserName = "newuser",
                    Name = "New",
                    Surname = "User",
                    EmailAddress = "new@example.com",
                    IsActive = true,
                    Password = "Password123!"
                },
                AssignedRoleNames = new[] { "admin" },
                SetRandomPassword = false,
                SendActivationEmail = false
            };

            await _sut.CreateOrUpdateUser(input);

            await _passwordValidator.Received(1).ValidateAsync(userManager, newUser, "Password123!");
            _passwordHasher.Received(1).HashPassword(newUser, "Password123!");
        }

        #endregion

        #region GetUsers

        [Fact]
        public async Task Dado_UsuariosCadastrados_Quando_GetUsers_Entao_DeveRetornarListaPaginadaMapeada()
        {
            // Dado
            var users = new List<User>
            {
                new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" },
                new User { Id = 2, UserName = "user2", Name = "User", Surname = "Two", EmailAddress = "user2@example.com" }
            };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(users.AsAsyncQueryable());
            _sut.UserManager = userManager;

            var userRoles = new List<Abp.Authorization.Users.UserRole>
            {
                new Abp.Authorization.Users.UserRole(1, 1, 1),
                new Abp.Authorization.Users.UserRole(1, 2, 2)
            };
            _userRoleRepository.GetAll().Returns(userRoles.AsAsyncQueryable());

            _roleManager.GetRoleByIdAsync(1).Returns(new Role(null, "admin", "Admin") { Id = 1 });
            _roleManager.GetRoleByIdAsync(2).Returns(new Role(null, "user", "User") { Id = 2 });

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>
            {
                new UserListDto { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", Roles = new List<UserListRoleDto>() },
                new UserListDto { Id = 2, UserName = "user2", Name = "User", Surname = "Two", Roles = new List<UserListRoleDto>() }
            });
            objectMapper.Map<List<UserListRoleDto>>(Arg.Any<object>()).Returns(ci =>
            {
                var src = ci.ArgAt<object>(0) as IEnumerable<Abp.Authorization.Users.UserRole>;
                return src?.Select(ur => new UserListRoleDto { RoleId = ur.RoleId }).ToList() ?? new List<UserListRoleDto>();
            });
            _sut.ObjectMapper = objectMapper;

            var input = new GetUsersInput
            {
                Sorting = "Name,Surname",
                MaxResultCount = 10,
                SkipCount = 0,
                Filter = ""
            };

            // Quando
            var result = await _sut.GetUsers(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
        }

        [Fact]
        public async Task Dado_FiltroPorNome_Quando_GetUsers_Entao_DeveRetornarApenasUsuariosFiltrados()
        {
            // Dado
            var users = new List<User>
            {
                new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" },
                new User { Id = 2, UserName = "user2", Name = "User", Surname = "Two", EmailAddress = "user2@example.com" }
            };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(users.AsAsyncQueryable());
            _sut.UserManager = userManager;

            _userRoleRepository.GetAll().Returns(new List<Abp.Authorization.Users.UserRole>().AsAsyncQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>
            {
                new UserListDto { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", Roles = new List<UserListRoleDto>() }
            });
            objectMapper.Map<List<UserListRoleDto>>(Arg.Any<object>()).Returns(new List<UserListRoleDto>());
            _sut.ObjectMapper = objectMapper;

            var input = new GetUsersInput
            {
                Sorting = "Name",
                MaxResultCount = 10,
                SkipCount = 0,
                Filter = "admin"
            };

            // Quando
            var result = await _sut.GetUsers(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.TotalCount.ShouldBe(1);
        }

        #endregion

        #region GetUsersToExcel

        [Fact]
        public async Task Dado_CacheComUsuarios_Quando_GetUsersToExcel_Entao_DeveRetornarArquivoExcel()
        {
            // Dado
            var users = new List<User>
            {
                new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" }
            };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(users.AsAsyncQueryable());
            _sut.UserManager = userManager;

            var userRoles = new List<Abp.Authorization.Users.UserRole>();
            _userRoleRepository.GetAll().Returns(userRoles.AsAsyncQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>
            {
                new UserListDto { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", Roles = new List<UserListRoleDto>() }
            });
            objectMapper.Map<List<UserListRoleDto>>(Arg.Any<object>()).Returns(new List<UserListRoleDto>());
            _sut.ObjectMapper = objectMapper;

            var expectedFile = new FileDto("users.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            _userListExcelExporter.ExportToFile(Arg.Any<List<UserListDto>>()).Returns(expectedFile);

            _usersCache.Get("ALL", Arg.Any<Func<string, object>>()).Returns(ci => ci.ArgAt<Func<string, object>>(1).Invoke("ALL"));

            // Quando
            var result = await _sut.GetUsersToExcel();

            // Então
            result.ShouldNotBeNull();
            result.ShouldBe(expectedFile);
            _usersCache.Received(1).Get("ALL", Arg.Any<Func<string, object>>());
        }

        [Fact]
        public async Task Dado_CacheAtingido_Quando_GetUsersToExcel_Entao_DeveUsarCacheSemConsultarRepositorio()
        {
            // Dado
            var users = new List<User>
            {
                new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", EmailAddress = "admin@example.com" }
            };

            var userManager = ManagerTestHelper.CreateUserManager();
            _sut.UserManager = userManager;

            _userRoleRepository.GetAll().Returns(new List<Abp.Authorization.Users.UserRole>().AsAsyncQueryable());

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<UserListDto>>(Arg.Any<object>()).Returns(new List<UserListDto>
            {
                new UserListDto { Id = 1, UserName = "admin", Name = "Admin", Surname = "User", Roles = new List<UserListRoleDto>() }
            });
            objectMapper.Map<List<UserListRoleDto>>(Arg.Any<object>()).Returns(new List<UserListRoleDto>());
            _sut.ObjectMapper = objectMapper;

            var expectedFile = new FileDto("users.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            _userListExcelExporter.ExportToFile(Arg.Any<List<UserListDto>>()).Returns(expectedFile);

            _usersCache.Get("ALL", Arg.Any<Func<string, List<User>>>()).Returns(users);

            // Quando
            var result = await _sut.GetUsersToExcel();

            // Então
            result.ShouldNotBeNull();
            result.ShouldBe(expectedFile);
            var _ = userManager.DidNotReceive().Users;
        }

        #endregion

        #region NotificationNewUser

        [Fact]
        public async Task Dado_ExcecoesNaNotificacaoEWebhook_Quando_NotificationNewUser_Entao_DeveCapturarSemLancar()
        {
            // Dado
            typeof(Abp.AbpServiceBase).GetProperty("LocalizationSourceName", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_sut, "Eaf");

            var notificationPublisher = Substitute.For<INotificationPublisher>();
            notificationPublisher.PublishAsync(
                Arg.Any<string>(),
                Arg.Any<NotificationData>(),
                Arg.Any<EntityIdentifier>(),
                Arg.Any<NotificationSeverity>(),
                Arg.Any<Abp.UserIdentifier[]>(),
                Arg.Any<Abp.UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>()
            ).Returns(Task.FromException(new Exception("Notification error")));

            var webhookPublisher = Substitute.For<IWebhookPublisher>();
            webhookPublisher.PublishAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<bool>(),
                Arg.Any<WebhookHeader>()
            ).Returns(Task.FromException(new Exception("Webhook error")));

            typeof(UserAppService).GetField("_notificationPublisher", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_sut, notificationPublisher);
            typeof(UserAppService).GetField("_webhookPublisher", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_sut, webhookPublisher);

            var user = new User { Id = 1, UserName = "testuser", Name = "Test", Surname = "User", TenantId = null };
            var method = typeof(UserAppService).GetMethod("NotificationNewUser", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando / Então
            var task = (Task)method.Invoke(_sut, new object[] { user });
            await Should.NotThrowAsync(() => task);

            await notificationPublisher.Received(1).PublishAsync(
                Arg.Any<string>(),
                Arg.Any<NotificationData>(),
                Arg.Any<EntityIdentifier>(),
                Arg.Any<NotificationSeverity>(),
                Arg.Any<Abp.UserIdentifier[]>(),
                Arg.Any<Abp.UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>()
            );

            await webhookPublisher.Received(1).PublishAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<bool>(),
                Arg.Any<WebhookHeader>()
            );
        }

        #endregion
    }
}
