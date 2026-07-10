using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Caching;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Sessions;
using Eaf.Middleware.Sessions.Dto;
using Eaf.Middleware.UiCustomization;
using Eaf.Middleware.UiCustomization.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions
{
    /// <summary>
    /// Testes BDD para SessionAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class SessionAppServiceBddTests
    {
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;
        private readonly SessionAppService _sut;

        public SessionAppServiceBddTests()
        {
            _uiThemeCustomizerFactory = Substitute.For<IUiThemeCustomizerFactory>();
            _sut = new SessionAppService(_uiThemeCustomizerFactory);
        }

        private static UserManager CreateUserManager()
        {
            var userStore = Substitute.For<UserStore>(new object[10]);
            var userRepository = Substitute.For<IRepository<User, long>>();
            var optionsAccessor = Options.Create(new IdentityOptions());
            var passwordHasher = Substitute.For<IPasswordHasher<User>>();
            var userValidators = Array.Empty<IUserValidator<User>>();
            var passwordValidators = Array.Empty<IPasswordValidator<User>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var services = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<UserManager>>();
            var roleManager = CreateRoleManager();
            var permissionManager = Substitute.For<IPermissionManager>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var cacheManager = Substitute.For<ICacheManager>();
            var settingManager = Substitute.For<ISettingManager>();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var userOrganizationUnitRepository = Substitute.For<IRepository<UserOrganizationUnit, long>>();
            var organizationUnitSettings = Substitute.For<IOrganizationUnitSettings>();
            var userLoginRepository = Substitute.For<IRepository<UserLogin, long>>();

            var userManager = Substitute.For<UserManager>(new object[]
            {
                userStore, userRepository, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                keyNormalizer, errors, services, logger, roleManager, permissionManager, unitOfWorkManager,
                cacheManager, settingManager, localizationManager, organizationUnitRepository,
                userOrganizationUnitRepository, organizationUnitSettings, userLoginRepository
            });

            return userManager;
        }

        private static RoleManager CreateRoleManager()
        {
            var roleStore = Substitute.For<RoleStore>(new object[]
            {
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            });
            var roleValidators = Array.Empty<IRoleValidator<Role>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var logger = Substitute.For<ILogger<RoleManager>>();
            var permissionManager = Substitute.For<IPermissionManager>();
            var roleManagementConfig = Substitute.For<IRoleManagementConfig>();
            var cacheManager = Substitute.For<ICacheManager>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var organizationUnitRoleRepository = Substitute.For<IRepository<OrganizationUnitRole, long>>();

            return Substitute.For<RoleManager>(new object[]
            {
                roleStore, roleValidators, keyNormalizer, errors, logger, permissionManager,
                roleManagementConfig, cacheManager, unitOfWorkManager, localizationManager,
                organizationUnitRepository, organizationUnitRoleRepository
            });
        }

        #region Construtor

        [Fact]
        public void Dado_UiThemeCustomizerFactory_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region UpdateUserSignInToken

        [Fact]
        public async Task Dado_UsuarioNaoLogado_Quando_UpdateUserSignInToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.LocalizationManager = localizationManager;

            // Quando / Então
            await Should.ThrowAsync<System.Exception>(() => _sut.UpdateUserSignInToken());
        }

        [Fact]
        public async Task Dado_UserIdZero_Quando_UpdateUserSignInToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(0L);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.LocalizationManager = localizationManager;

            // Quando / Então
            await Should.ThrowAsync<AbpException>(() => _sut.UpdateUserSignInToken());
        }

        #endregion

        #region GetCurrentLoginInformations

        [Fact]
        public async Task Dado_UsuarioSemTenant_Quando_GetCurrentLoginInformations_Entao_DeveRetornarInformacoesAplicacaoEUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();
            SetupUiThemeCustomizer();

            // Quando
            var result = await _sut.GetCurrentLoginInformations();

            // Então
            result.ShouldNotBeNull();
            result.Application.ShouldNotBeNull();
            result.Application.Currency.ShouldBe("BRL");
            result.Application.CurrencySign.ShouldBe("R$");
            result.Theme.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.Tenant.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioSemLogin_Quando_GetCurrentLoginInformations_Entao_DeveRetornarAplicacaoETema()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.ObjectMapper = CreateObjectMapper();
            SetupUiThemeCustomizer();

            // Quando
            var result = await _sut.GetCurrentLoginInformations();

            // Então
            result.ShouldNotBeNull();
            result.Application.ShouldNotBeNull();
            result.Theme.ShouldNotBeNull();
            result.User.ShouldBeNull();
            result.Tenant.ShouldBeNull();
        }

        #endregion

        #region UpdateUserSignInToken

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_UpdateUserSignInToken_Entao_DeveRetornarTokenValido()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.UpdateUserSignInToken();

            // Então
            result.ShouldNotBeNull();
            result.SignInToken.ShouldNotBeNull();
            result.EncodedUserId.ShouldNotBeNull();
            result.EncodedTenantId.ShouldBeEmpty();
        }

        #endregion

        #region GetCurrentLoginInformationsTenant

        [Fact]
        public async Task Dado_UsuarioEmTenant_Quando_GetCurrentLoginInformations_Entao_DeveRetornarTenantEUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1 };
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.Tenants.Returns(new List<Tenant> { tenant }.AsAsyncQueryable());

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns(1);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.TenantManager = tenantManager;
            _sut.ObjectMapper = CreateObjectMapper();
            SetupUiThemeCustomizer();

            // Quando
            var result = await _sut.GetCurrentLoginInformations();

            // Então
            result.ShouldNotBeNull();
            result.Tenant.ShouldNotBeNull();
            result.Tenant.Id.ShouldBe(1);
            result.User.ShouldNotBeNull();
        }

        #endregion

        #region UpdateUserSignInTokenTenant

        [Fact]
        public async Task Dado_UsuarioLogadoEmTenant_Quando_UpdateUserSignInToken_Entao_DeveRetornarTenantIdCodificado()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", TenantId = 1 };
            var userManager = CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns(1);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.UpdateUserSignInToken();

            // Então
            result.ShouldNotBeNull();
            result.SignInToken.ShouldNotBeNull();
            result.EncodedUserId.ShouldNotBeNull();
            result.EncodedTenantId.ShouldNotBeEmpty();
        }

        #endregion

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<UserLoginInfoDto>(Arg.Any<object>()).Returns(ci =>
            {
                var user = (User)ci.Arg<object>();
                return new UserLoginInfoDto { Id = user.Id, UserName = user.UserName };
            });
            objectMapper.Map<TenantLoginInfoDto>(Arg.Any<object>()).Returns(ci =>
            {
                var tenant = (Tenant)ci.Arg<object>();
                return new TenantLoginInfoDto { Id = tenant.Id, Name = tenant.TenancyName };
            });
            return objectMapper;
        }

        private void SetupUiThemeCustomizer()
        {
            var customizer = Substitute.For<IUiCustomizer>();
            customizer.GetUiSettings().Returns(new UiCustomizationSettingsDto());
            _uiThemeCustomizerFactory.GetCurrentUiCustomizer().Returns(customizer);
        }
    }
}
