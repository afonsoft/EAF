#nullable disable

using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    /// <summary>
    /// Testes BDD para LogInManager exercitando caminhos reais de login.
    /// </summary>
    public class LogInManagerBddTests
    {
        private static LogInManager CreateLogInManager(User user = null, Action<UserManager> configureUserManager = null, bool isEmailConfirmationRequired = false)
        {
            user ??= new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@example.com",
                IsActive = true,
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = true,
                IsTwoFactorEnabled = false
            };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByNameOrEmailAsync(Arg.Any<int?>(), Arg.Any<string>()).Returns(user);
            userManager.FindAsync(Arg.Any<int?>(), Arg.Any<UserLoginInfo>()).Returns(user);
            configureUserManager?.Invoke(userManager);

            var roleManager = ManagerTestHelper.CreateRoleManager();
            var unitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            var claimsPrincipalFactory = new UserClaimsPrincipalFactory(
                userManager,
                roleManager,
                Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                unitOfWorkManager
            );

            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            tenantRepository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Tenant, bool>>>())
                .Returns(tenant);

            var settingManager = Substitute.For<ISettingManager>();
            var settingValue = isEmailConfirmationRequired ? "true" : "false";
            settingManager.GetSettingValueForTenantAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(settingValue);
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns("false");

            var multiTenancyConfig = Substitute.For<IMultiTenancyConfig>();
            multiTenancyConfig.IsEnabled.Returns(false);

            var userManagementConfig = Substitute.For<IUserManagementConfig>();
            userManagementConfig.ExternalAuthenticationSources.Returns(new TypeList());

            var userLoginAttemptRepository = Substitute.For<IRepository<UserLoginAttempt, long>>();

            var iocResolver = Substitute.For<IIocResolver>();
            iocResolver.Resolve(typeof(ILocalizationContext)).Returns(Substitute.For<ILocalizationContext>());

            return new LogInManager(
                userManager,
                multiTenancyConfig,
                tenantRepository,
                unitOfWorkManager,
                settingManager,
                userLoginAttemptRepository,
                userManagementConfig,
                iocResolver,
                roleManager,
                Substitute.For<IPasswordHasher<User>>(),
                claimsPrincipalFactory
            );
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeAbpLogInManager()
        {
            typeof(LogInManager).BaseType.Name.ShouldContain("AbpLogInManager");
        }

        [Fact]
        public async Task Dado_CredenciaisValidas_Quando_LoginAsync_Entao_DeveRetornarSucesso()
        {
            // Dado
            var sut = CreateLogInManager();

            // Quando
            var result = await sut.LoginAsync("admin", "password", "Default", shouldLockout: false);

            // Então
            result.ShouldNotBeNull();
            result.Result.ShouldBe(AbpLoginResultType.Success);
            result.User.ShouldNotBeNull();
            result.Tenant.ShouldNotBeNull();
            result.Identity.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_LoginExternoValido_Quando_LoginAsyncNoPass_Entao_DeveRetornarSucesso()
        {
            // Dado
            var sut = CreateLogInManager();
            var login = new UserLoginInfo("EAF", "provider-key", "EAF");

            // Quando
            var result = await sut.LoginAsync(login, "Default");

            // Então
            result.ShouldNotBeNull();
            result.Result.ShouldBe(AbpLoginResultType.Success);
            result.User.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_LoginExternoInvalido_Quando_LoginAsyncNoPass_Entao_DeveRetornarUsuarioInvalido()
        {
            // Dado
            var sut = CreateLogInManager(configureUserManager: userManager =>
            {
                userManager.FindAsync(Arg.Any<int?>(), Arg.Any<UserLoginInfo>()).Returns((User)null);
            });

            var login = new UserLoginInfo("EAF", "missing-key", "EAF");

            // Quando
            var result = await sut.LoginAsync(login, "Default");

            // Então
            result.Result.ShouldBe(AbpLoginResultType.UnknownExternalLogin);
        }

        [Fact]
        public async Task Dado_SenhaIncorreta_Quando_LoginAsync_Entao_DeveRetornarSenhaInvalida()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                IsActive = true,
                IsEmailConfirmed = true
            };
            var sut = CreateLogInManager(user, configureUserManager: userManager =>
            {
                userManager.CheckPasswordAsync(user, "wrongpassword").Returns(false);
            });

            // Quando
            var result = await sut.LoginAsync("admin", "wrongpassword", "Default", shouldLockout: false);

            // Então
            result.Result.ShouldBe(AbpLoginResultType.InvalidPassword);
        }

        [Fact]
        public async Task Dado_UsuarioBloqueado_Quando_LoginAsync_Entao_DeveRetornarLockedOut()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                IsActive = true,
                IsEmailConfirmed = true
            };
            var sut = CreateLogInManager(user, configureUserManager: userManager =>
            {
                userManager.IsLockedOutAsync(user).Returns(true);
            });

            // Quando
            var result = await sut.LoginAsync("admin", "password", "Default", shouldLockout: false);

            // Então
            result.Result.ShouldBe(AbpLoginResultType.LockedOut);
        }

        [Fact]
        public async Task Dado_UsuarioInativo_Quando_LoginAsync_Entao_DeveRetornarUsuarioInativo()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                IsActive = false,
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = true
            };

            var sut = CreateLogInManager(user);

            // Quando
            var result = await sut.LoginAsync("admin", "password", "Default", shouldLockout: false);

            // Então
            result.Result.ShouldBe(AbpLoginResultType.UserIsNotActive);
        }

        [Fact]
        public async Task Dado_EmailNaoConfirmadoExigido_Quando_LoginAsync_Entao_DeveRetornarEmailNaoConfirmado()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                IsActive = true,
                IsEmailConfirmed = false,
                IsPhoneNumberConfirmed = true
            };

            var sut = CreateLogInManager(user, isEmailConfirmationRequired: true);

            // Quando
            var result = await sut.LoginAsync("admin", "password", "Default", shouldLockout: false);

            // Então
            result.Result.ShouldBe(AbpLoginResultType.UserEmailIsNotConfirmed);
        }

        [Fact]
        public async Task Dado_UsuarioValido_Quando_CreateLoginResultAsync_Entao_DeveRetornarSucesso()
        {
            // Dado
            var user = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                IsActive = true,
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = true
            };
            var tenant = new Tenant("Default", "Default") { Id = 1 };
            var sut = CreateLogInManager(user);

            // Quando
            var result = await sut.CreateLoginResultAsync(user, tenant);

            // Então
            result.ShouldNotBeNull();
            result.Result.ShouldBe(AbpLoginResultType.Success);
            result.User.ShouldBe(user);
            result.Tenant.ShouldBe(tenant);
        }
    }
}
