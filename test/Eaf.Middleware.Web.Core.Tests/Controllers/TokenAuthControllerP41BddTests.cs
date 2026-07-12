using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Memory;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.TwoFactor;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Security.Recaptcha;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Authentication;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Controllers;
using Eaf.Middleware.Web.Core.Tests.Identity;
using Eaf.Middleware.Web.Models.TokenAuth;
using Eaf.Middleware.Web.Notifications;
using Eaf.Security;
using Eaf.WebHooks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public partial class TokenAuthControllerBddTests
    {
        #region Authenticate

        [Fact]
        public async Task Dado_CaptchaHabilitado_Quando_Authenticate_Entao_DeveValidarCaptchaERetornarAccessToken()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerCaptcha();
            controller.RecaptchaValidator = NullRecaptchaValidator.Instance;
            ConfigurarTokenAuthConfiguration(controller);

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password",
                CaptchaResponse = "captcha-response"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
        }

        #endregion

        #region External Authenticate

        [Fact]
        public async Task Dado_LoginExternoSuccess_SingleSignIn_Quando_ExternalAuthenticate_Entao_DeveRetornarAccessTokenComReturnUrlModificado()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.Name = "Admin";
            user.Surname = "User";
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "testuser@example.com",
                Picture = string.Empty
            };

            var externalAuthManager = CriarExternalAuthManager(externalUserInfo);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);

            var result = await controller.ExternalAuthenticate(new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ProviderKey = "provider-key",
                ProviderAccessCode = "access-code",
                SingleSignIn = true,
                ReturnUrl = "https://example.com"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.ReturnUrl.ShouldContain("accessToken=");
            result.ReturnUrl.ShouldContain("userId=");
        }

        [Fact]
        public async Task Dado_LoginExternoComResultadoInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var invalidResult = new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidPassword, tenant, user);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, invalidResult);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "testuser@example.com"
            };

            var externalAuthManager = CriarExternalAuthManager(externalUserInfo);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.ExternalAuthenticate(new ExternalAuthenticateModel
                {
                    AuthProvider = "Microsoft",
                    ProviderKey = "provider-key",
                    ProviderAccessCode = "access-code"
                }));

            exception.ShouldNotBeNull();
        }

        #endregion

        #region IsSchemeEnabled

        [Fact]
        public void Dado_AuthZeroHabilitado_Quando_IsSchemeEnabled_Entao_DeveRetornarTrue()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.SettingManager = CriarSettingManagerExternal();
            SetField(controller, "_settingManager", CriarSettingManagerExternal());

            var method = typeof(TokenAuthController).GetMethod("IsSchemeEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var scheme = new ExternalLoginProviderInfo(
                name: "AuthZero",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            var result = method.Invoke(controller, new object[] { scheme });
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_AuthZeroDesabilitado_Quando_IsSchemeEnabled_Entao_DeveRetornarFalse()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled)
                    return "false";
                return "true";
            });
            controller.SettingManager = settingManager;
            SetField(controller, "_settingManager", settingManager);

            var method = typeof(TokenAuthController).GetMethod("IsSchemeEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var scheme = new ExternalLoginProviderInfo(
                name: "AuthZero",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            var result = method.Invoke(controller, new object[] { scheme });
            result.ShouldBe(false);
        }

        [Fact]
        public void Dado_SessaoHostSemTenant_Quando_IsSchemeEnabled_Entao_DeveRetornarTrue()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            controller.AbpSession = abpSession;

            var method = typeof(TokenAuthController).GetMethod("IsSchemeEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var scheme = new ExternalLoginProviderInfo(
                name: "Google",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            var result = method.Invoke(controller, new object[] { scheme });
            result.ShouldBe(true);
        }

        #endregion

        #region IsTwoFactorAuthRequired

        [Fact]
        public async Task Dado_TwoFactorLoginDesabilitado_Quando_IsTwoFactorAuthRequired_Entao_DeveRetornarFalse()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            userManager.GetValidTwoFactorProvidersAsync(Arg.Any<User>()).Returns(Task.FromResult<IList<string>>(new List<string> { "Email" }));
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var method = typeof(TokenAuthController).GetMethod("IsTwoFactorAuthRequiredAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<bool>)method.Invoke(controller, new object[] { loginResult, new AuthenticateModel() });
            var result = await task;
            result.ShouldBe(false);
        }

        [Fact]
        public async Task Dado_UsuarioSemTwoFactorHabilitado_Quando_IsTwoFactorAuthRequired_Entao_DeveRetornarFalse()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = false;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            userManager.GetValidTwoFactorProvidersAsync(Arg.Any<User>()).Returns(Task.FromResult<IList<string>>(new List<string> { "Email" }));
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();

            var method = typeof(TokenAuthController).GetMethod("IsTwoFactorAuthRequiredAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<bool>)method.Invoke(controller, new object[] { loginResult, new AuthenticateModel() });
            var result = await task;
            result.ShouldBe(false);
        }

        [Fact]
        public async Task Dado_UsuarioSemProvidersValidos_Quando_IsTwoFactorAuthRequired_Entao_DeveRetornarFalse()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            userManager.GetValidTwoFactorProvidersAsync(Arg.Any<User>()).Returns(Task.FromResult<IList<string>>(new List<string>()));
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();

            var method = typeof(TokenAuthController).GetMethod("IsTwoFactorAuthRequiredAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<bool>)method.Invoke(controller, new object[] { loginResult, new AuthenticateModel() });
            var result = await task;
            result.ShouldBe(false);
        }

        #endregion

        #region LogOut

        [Fact]
        public async Task Dado_ErrosNosTresCaminhosDeLogOut_Quando_LogOut_Entao_DeveCapturarExcecoesSemLancar()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(Task.FromResult<User>(null));

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(user.Id);
            abpSession.TenantId.Returns(user.TenantId);
            controller.AbpSession = abpSession;

            var principalAccessor = GetField<IPrincipalAccessor>(controller, "_principalAccessor");
            principalAccessor.Principal.Returns(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key"),
                    new Claim(MiddlewareCoreConsts.UserIdentifier, $"{user.Id}@2")
                })));

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key"),
                    new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                    new Claim(AbpClaimTypes.TenantId, "3")
                }, "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            await Should.NotThrowAsync(controller.LogOut());
        }

        #endregion

        #region CreateJwtClaims

        [Fact]
        public async Task Dado_ErroAoSalvarClaimsAdicionais_Quando_CreateJwtClaims_Entao_DeveRetornarClaimsMesmoAssim()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);

            var saveCount = 0;
            var uow = Substitute.For<IUnitOfWork>();
            uow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            uow.DisableFilter(Arg.Any<string[]>()).Returns(Substitute.For<IDisposable>());
            uow.CompleteAsync().Returns(Task.CompletedTask);
            uow.SaveChangesAsync().Returns(_ =>
            {
                saveCount++;
                if (saveCount == 2)
                    return Task.FromException<int>(new Exception("save error"));
                return Task.FromResult(0);
            });

            var uowManager = Substitute.For<IUnitOfWorkManager>();
            uowManager.Current.Returns(uow);
            controller.UnitOfWorkManager = uowManager;

            var method = typeof(TokenAuthController).GetMethod("CreateJwtClaims", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<IEnumerable<Claim>>)method.Invoke(controller, new object[] { identity, user, "" });
            var claims = await task;

            claims.ShouldNotBeNull();
            claims.ShouldContain(c => c.Type == MiddlewareCoreConsts.TokenValidityKey);
        }

        #endregion

        #region UpdateExternalUserAsync

        [Fact]
        public async Task Dado_ErroAoAtualizarUsuarioEFoto_Quando_UpdateExternalUserAsync_Entao_DeveCapturarExcecoesSemLancar()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.Name = "Old";
            user.Surname = "Old";
            user.ExternalAuthProviderformation = "Old";

            var userManager = CriarUserManagerSubstituto(user);
            userManager.UpdateAsync(user).Returns(Task.FromException<IdentityResult>(new Exception("update error")));

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            binaryObjectManager.GetOrNullAsync(Arg.Any<Guid>()).Returns(Task.FromResult<BinaryObject>(null));
            binaryObjectManager.SaveAsync(Arg.Any<BinaryObject>()).Returns(Task.FromException(new Exception("save error")));
            binaryObjectManager.DeleteAsync(Arg.Any<Guid>()).Returns(Task.CompletedTask);
            SetField(controller, "_binaryObjectManager", binaryObjectManager);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Name = "New Name",
                Surname = "New Surname",
                Provider = "Microsoft",
                Picture = "ZmFrZQ=="
            };

            var method = typeof(TokenAuthController).GetMethod("UpdateExternalUserAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task)method.Invoke(controller, new object[] { user, externalUserInfo });
            await Should.NotThrowAsync(() => task);
        }

        #endregion

        #region RegisterExternalUserAsync

        [Fact]
        public async Task Dado_UsuarioExternoExistente_Quando_RegisterExternalUserAsync_Entao_DeveAtualizarUsuario()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.ProfilePictureId = null;

            var userManager = CriarUserManagerRealComStoreSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            SetField(controller, "_binaryObjectManager", CriarBinaryObjectManager(null));

            var externalLoginInfoManager = CriarExternalLoginInfoManager();
            SetField(controller, "_iocManager", CriarIocManager(externalLoginInfoManager));

            var externalUserInfo = new ExternalAuthUserInfo
            {
                EmailAddress = "testuser@example.com",
                Name = "Updated Name",
                Surname = "Surname",
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Picture = "ZmFrZQ=="
            };

            var method = typeof(TokenAuthController).GetMethod("RegisterExternalUserAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<User>)method.Invoke(controller, new object[] { externalUserInfo });
            var result = await task;

            result.ShouldNotBeNull();
            result.UserName.ShouldBe("testuser");
            result.EmailAddress.ShouldBe("testuser@example.com");
            result.ProfilePictureId.ShouldNotBeNull();
        }

        private static UserManager CriarUserManagerRealComStoreSubstituto(User user)
        {
            var userStore = Substitute.For<UserStore>(new object[10]);
            userStore.FindByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(user));
            userStore.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(user));
            userStore.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(user));
            userStore.FindByNameOrEmailAsync(Arg.Any<string>()).Returns(Task.FromResult(user));
            userStore.FindByNameOrEmailAsync(Arg.Any<int?>(), Arg.Any<string>()).Returns(Task.FromResult(user));
            userStore.GetUserNameFromDatabaseAsync(Arg.Any<long>()).Returns(Task.FromResult("olduser"));
            userStore.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(IdentityResult.Success));

            var userRepository = Substitute.For<IRepository<User, long>>();

            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            keyNormalizer.NormalizeName(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>()?.ToUpperInvariant() ?? "TEST");
            keyNormalizer.NormalizeEmail(Arg.Any<string>()).Returns(callInfo => callInfo.Arg<string>()?.ToUpperInvariant() ?? "TEST");

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var unitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            return new UserManager(
                userStore,
                userRepository,
                Options.Create(new IdentityOptions()),
                Substitute.For<IPasswordHasher<User>>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                keyNormalizer,
                new IdentityErrorDescriber(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager>>(),
                roleManager,
                Substitute.For<IPermissionManager>(),
                unitOfWorkManager,
                Substitute.For<ICacheManager>(),
                Substitute.For<ISettingManager>(),
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<UserOrganizationUnit, long>>(),
                Substitute.For<IOrganizationUnitSettings>(),
                Substitute.For<IRepository<UserLogin, long>>()
            );
        }

        #endregion
    }
}
