using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.MultiTenancy;
using Abp.ObjectMapping;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Abp.Runtime.Security;
using Eaf.Middleware.Authorization.TwoFactor;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Abp.Zero.Configuration;
using Castle.Core.Logging;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Controllers;
using Eaf.Middleware.Web.Core.Tests.Identity;
using Eaf.Middleware.Web.Models.TokenAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public partial class TokenAuthControllerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(TokenAuthController).Name.ShouldBe("TokenAuthController");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            typeof(TokenAuthController).BaseType.Name.ShouldBe("MiddlewareControllerBase");
        }

        [Fact]
        public void Dado_UsuarioValido_Quando_GetAuthenticationProviders_Entao_DeveRetornarProviderModel()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            // Quando
            var result = controller.GetAuthenticationProviders("admin");

            // Então
            result.ShouldNotBeNull();
            result.UsernameOrEmailAddress.ShouldBe("admin");
            result.AuthenticationSource.ShouldBe("System");
            result.Tenant.ShouldNotBeNull();
            result.Tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_ProvidersConfigurados_Quando_GetExternalAuthenticationProviders_Entao_DeveRetornarListaMapeada()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var providerInfo = new ExternalLoginProviderInfo(
                name: "Test",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            var infoProvider = Substitute.For<IExternalLoginInfoProvider>();
            infoProvider.GetExternalLoginInfo().Returns(providerInfo);

            var externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();
            externalAuthConfiguration.ExternalLoginInfoProviders.Returns(new List<IExternalLoginInfoProvider> { infoProvider });

            controller.ObjectMapper = CriarObjectMapper();
            controller.SettingManager = CriarSettingManager();

            var field = typeof(TokenAuthController).GetField("_externalAuthConfiguration", BindingFlags.NonPublic | BindingFlags.Instance);
            field.ShouldNotBeNull();
            field.SetValue(controller, externalAuthConfiguration);

            // Quando
            var result = controller.GetExternalAuthenticationProviders();

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<ExternalLoginProviderInfoModel>>();
            result.Count.ShouldBe(1);
            result.First().Name.ShouldBe("Test");
        }

        private static TokenAuthController CriarController(
            UserManager userManager,
            RoleManager roleManager,
            LogInManager logInManager,
            IExternalAuthManager externalAuthManager = null,
            IImpersonationManager impersonationManager = null,
            ICacheManager cacheManager = null,
            ISettingManager settingManager = null,
            IIocManager iocManager = null)
        {
            settingManager ??= CriarSettingManager();
            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { Id = 1, Name = "Default", TenancyName = "Default" });

            var controller = new TokenAuthController(
                logInManager,
                new AbpLoginResultTypeHelper(),
                new TokenAuthConfiguration(),
                userManager,
                roleManager,
                tenantCache,
                cacheManager ?? Substitute.For<ICacheManager>(),
                impersonationManager ?? Substitute.For<IImpersonationManager>(),
                Options.Create(new IdentityOptions()),
                Substitute.For<ILogger>(),
                settingManager,
                externalAuthManager ?? Substitute.For<IExternalAuthManager>(),
                Substitute.For<IExternalAuthConfiguration>(),
                iocManager ?? Substitute.For<IIocManager>(),
                Substitute.For<IPasswordHasher<User>>(),
                Substitute.For<IEmailSender>(),
                Options.Create(new JwtBearerOptions()),
                Substitute.For<INotificationPublisher>(),
                Substitute.For<IBinaryObjectManager>(),
                Substitute.For<INotificationSubscriptionManager>(),
                Substitute.For<IWebhookPublisher>(),
                Substitute.For<IPrincipalAccessor>()
            );

            controller.SettingManager = settingManager;

            var settingManagerField = typeof(TokenAuthController).GetField("_settingManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            settingManagerField?.SetValue(controller, settingManager);

            return controller;
        }

        private static ISettingManager CriarSettingManager()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns("false");
            return settingManager;
        }

        [Fact]
        public void Dado_UsuarioNaoEncontrado_Quando_GetAuthenticationProviders_Entao_DeveRetornarSystemProvider()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            // Quando
            var result = controller.GetAuthenticationProviders("unknown@user.com");

            // Então
            result.ShouldNotBeNull();
            result.AuthenticationSource.ShouldBe("System");
            result.UsernameOrEmailAddress.ShouldBe("unknown@user.com");
        }

        [Fact]
        public void Dado_LdapHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarLdap()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(LdapSettingNames.IsEnabled).Returns("true");
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_MicrosoftHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarMicrosoft()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(LdapSettingNames.IsEnabled).Returns("false");
            settingManager.GetSettingValueForApplication(AzureActiveDirectorySettingNames.IsEnabled).Returns("false");
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled).Returns("true");
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("Microsoft");
        }

        [Fact]
        public async Task Dado_SessaoNula_Quando_LogOut_Entao_DeveCompletarSemErro()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            controller.AbpSession = abpSession;

            // Quando & Então
            await Should.NotThrowAsync(controller.LogOut());
        }

        [Fact]
        public async Task Dado_ModeloInvalido_Quando_Authenticate_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.ModelState.AddModelError("Password", "Required");

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                controller.Authenticate(new AuthenticateModel { UserNameOrEmailAddress = "user", Password = null }));
            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ModeloInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.ModelState.AddModelError("ProviderKey", "Required");

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                controller.ExternalAuthenticate(new ExternalAuthenticateModel { AuthProvider = "test", ProviderKey = null, ProviderAccessCode = "code" }));
            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_CredenciaisValidas_Quando_Authenticate_Entao_DeveRetornarAccessToken()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = Substitute.For<LogInManager>(
                userManager,
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<IRepository<Tenant>>(),
                IdentityTestHelper.CreateUnitOfWorkManager(),
                CriarSettingManager(),
                Substitute.For<IRepository<UserLoginAttempt, long>>(),
                Substitute.For<IUserManagementConfig>(),
                Substitute.For<IIocResolver>(),
                roleManager,
                Substitute.For<IPasswordHasher<User>>(),
                IdentityTestHelper.CreateUserClaimsPrincipalFactory(userManager, roleManager));

            logInManager.LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(loginResult);

            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task Dado_UsuarioDeveAlterarSenha_Quando_Authenticate_Entao_DeveRetornarPasswordResetCode()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.ShouldChangePasswordOnNextLogin = true;
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            result.ShouldNotBeNull();
            result.ShouldResetPassword.ShouldBeTrue();
            result.PasswordResetCode.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task Dado_CredenciaisInvalidas_Quando_Authenticate_Entao_DeveLancarUserFriendlyException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(AbpLoginResultType.InvalidPassword, tenant, user);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var exception = await Should.ThrowAsync<Abp.UI.UserFriendlyException>(async () =>
                await controller.Authenticate(new AuthenticateModel
                {
                    UserNameOrEmailAddress = "admin",
                    Password = "wrongpassword"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ExternalLoginValido_Quando_ExternalAuthenticate_Entao_DeveRetornarAccessToken()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.Name = "Admin";
            user.Surname = "User";
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var externalAuthManager = Substitute.For<IExternalAuthManager>();
            externalAuthManager.GetUserInfo("Microsoft", "access-code").Returns(new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                Picture = string.Empty
            });

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.ExternalAuthenticate(new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ProviderKey = "provider-key",
                ProviderAccessCode = "access-code"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task Dado_ImpersonationTokenValido_Quando_ImpersonatedAuthenticate_Entao_DeveRetornarAccessToken()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });

            var impersonationManager = Substitute.For<IImpersonationManager>();
            impersonationManager.GetImpersonatedUserAndIdentity("token-123").Returns(new UserAndIdentity(user, identity));

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager, impersonationManager: impersonationManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.ImpersonatedAuthenticate("token-123");

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.ExpireInSeconds.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_SingleSignInHabilitado_Quando_Authenticate_Entao_DeveRetornarAccessTokenComReturnUrlModificado()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password",
                SingleSignIn = true,
                ReturnUrl = "https://example.com"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.ReturnUrl.ShouldContain("accessToken=");
            result.ReturnUrl.ShouldContain("userId=");
        }

        [Fact]
        public async Task Dado_LoginUnicoPorUsuario_Quando_Authenticate_Entao_DeveAtualizarSecurityStamp()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerComLoginUnico();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            await userManager.Received(1).UpdateSecurityStampAsync(user);
        }

        [Fact]
        public async Task Dado_UsuarioAutenticado_Quando_LogOut_Entao_DeveAtualizarSecurityStampELimparCache()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                        {
                            new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()!),
                            new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                            new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key")
                        }))
                }
            };

            var principalAccessor = (IPrincipalAccessor)typeof(TokenAuthController).GetField("_principalAccessor", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(controller)!;
            principalAccessor.Principal.Returns(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(MiddlewareCoreConsts.UserIdentifier, $"{user.Id}@{user.TenantId}"),
                    new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key")
                })));

            await Should.NotThrowAsync(() => controller.LogOut());

            await userManager.Received().UpdateSecurityStampAsync(user);
            await userManager.Received().RemoveTokenValidityKeyAsync(user, Arg.Any<string>(), Arg.Any<System.Threading.CancellationToken>());
        }

        [Fact]
        public async Task Dado_CacheItemExistente_Quando_SendTwoFactorAuthCode_Entao_DeveEnviarCodigoPorEmail()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);

            var cacheManager = new Abp.Runtime.Caching.Memory.AbpMemoryCacheManager(Substitute.For<Abp.Runtime.Caching.Configuration.ICachingConfiguration>());
            var cacheKey = new UserIdentifier(user.TenantId, user.Id).ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("old"));

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            await Should.NotThrowAsync(() => controller.SendTwoFactorAuthCode(new SendTwoFactorAuthCodeModel
            {
                UserId = user.Id,
                Provider = "Email"
            }));

            await userManager.Received().GenerateTwoFactorTokenAsync(user, "Email");
            var cacheItem = await cacheManager.GetTwoFactorCodeCache().GetOrDefaultAsync(cacheKey);
            cacheItem.ShouldNotBeNull();
            cacheItem.Code.ShouldBe("123456");
        }

        [Fact]
        public void Dado_UsuarioSemAuthenticationSource_Quando_GetAuthenticationProviders_Entao_DeveRetornarSystemProvider()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            user.AuthenticationSource = null;
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            // Quando
            var result = controller.GetAuthenticationProviders("admin");

            // Então
            result.ShouldNotBeNull();
            result.AuthenticationSource.ShouldBe("System");
        }

        [Fact]
        public void Dado_ProvedorGoogleHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarGoogle()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaProvider(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled));

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ProvedorAuthZeroHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarAuthZero()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaProvider(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled));

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("AuthZero");
        }

        [Fact]
        public void Dado_ProvedorOpenIdConnectHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarOpenIdConnect()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaProvider(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled));

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("OpenIdConnect");
        }

        [Fact]
        public void Dado_NenhumProvedorHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarSystem()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("System");
        }

        [Fact]
        public void Dado_ProvidersExternosComTenant_Quando_GetExternalAuthenticationProviders_Entao_DeveFiltrarPorConfiguracao()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaExternalProviders());
            controller.AbpSession = CriarAbpSession(user);

            var providerInfos = new List<ExternalLoginProviderInfo>
            {
                new ExternalLoginProviderInfo("OpenIdConnect", "client", "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>()),
                new ExternalLoginProviderInfo("Microsoft", "client", "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>()),
                new ExternalLoginProviderInfo("Google", "client", "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>()),
                new ExternalLoginProviderInfo("AuthZero", "client", "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>()),
                new ExternalLoginProviderInfo("Unknown", "client", "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>()),
                new ExternalLoginProviderInfo("EmptyClient", null, "secret", "1", typeof(object), new Dictionary<string, string>(), new List<JsonClaimMap>())
            };

            var infoProviders = providerInfos.Select(info =>
            {
                var provider = Substitute.For<IExternalLoginInfoProvider>();
                provider.GetExternalLoginInfo().Returns(info);
                return provider;
            }).ToList();

            var externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();
            externalAuthConfiguration.ExternalLoginInfoProviders.Returns(infoProviders);

            var field = typeof(TokenAuthController).GetField("_externalAuthConfiguration", BindingFlags.NonPublic | BindingFlags.Instance);
            field.ShouldNotBeNull();
            field.SetValue(controller, externalAuthConfiguration);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<ExternalLoginProviderInfoModel>>(Arg.Any<List<ExternalLoginProviderInfo>>())
                .Returns(callInfo =>
                {
                    var source = callInfo.Arg<List<ExternalLoginProviderInfo>>();
                    return source.Select(x => new ExternalLoginProviderInfoModel { Name = x.Name, ClientId = x.ClientId, TenantId = x.TenantId }).ToList();
                });
            controller.ObjectMapper = objectMapper;

            var result = controller.GetExternalAuthenticationProviders();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.Select(x => x.Name).ShouldContain("OpenIdConnect");
            result.Select(x => x.Name).ShouldContain("Unknown");
        }

        [Fact]
        public async Task Dado_CredenciaisValidasComSecurityStampVazio_Quando_Authenticate_Entao_DeveGerarSecurityStamp()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            user.SecurityStamp.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_TwoFactorHabilitado_Quando_AuthenticateSemCodigo_Entao_DeveRetornarRequerVerificacao()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerSubstituto(user);
            userManager.GetValidTwoFactorProvidersAsync(user).Returns(new List<string> { "Email" });

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerComTwoFactorHabilitado());
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            result.ShouldNotBeNull();
            result.RequiresTwoFactorVerification.ShouldBeTrue();
            result.TwoFactorAuthProviders.ShouldNotBeNull();
            result.TwoFactorAuthProviders.Count.ShouldBe(1);
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task Dado_ProviderKeyInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);

            var externalAuthManager = Substitute.For<IExternalAuthManager>();
            externalAuthManager.GetUserInfo("Microsoft", "access-code").Returns(new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = null,
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "admin@example.com",
                Picture = string.Empty
            });

            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                controller.ExternalAuthenticate(new ExternalAuthenticateModel
                {
                    AuthProvider = "Microsoft",
                    ProviderKey = "provider-key",
                    ProviderAccessCode = "access-code"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ExternalLoginInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Eaf.Middleware.MultiTenancy.Tenant("Default", "Default") { Id = 1, IsActive = true };
            var invalidLoginResult = new AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User>(AbpLoginResultType.InvalidPassword, tenant, user);

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, invalidLoginResult);

            var externalAuthManager = Substitute.For<IExternalAuthManager>();
            externalAuthManager.GetUserInfo("Microsoft", "access-code").Returns(new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "admin@example.com",
                Picture = string.Empty
            });

            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.SettingManager = CriarSettingManagerAsync();

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                controller.ExternalAuthenticate(new ExternalAuthenticateModel
                {
                    AuthProvider = "Microsoft",
                    ProviderKey = "provider-key",
                    ProviderAccessCode = "access-code"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_MicrosoftNaoHabilitado_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaTeams(false));
            controller.AbpSession = CriarAbpSession(user);

            var exception = await Should.ThrowAsync<AbpException>(() => controller.TeamsAuthenticate("token"));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Microsoft Provider is not enabled in HostSettings");
        }

        [Fact]
        public async Task Dado_MicrosoftNaoConfigurado_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager,
                settingManager: CriarSettingManagerParaTeams(true, ""));
            controller.AbpSession = CriarAbpSession(user);

            var exception = await Should.ThrowAsync<AbpException>(() => controller.TeamsAuthenticate("token"));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Microsoft Provider is not configured in HostSettings");
        }

        [Fact]
        public async Task Dado_ModeloInvalido_Quando_SendTwoFactorAuthCode_Entao_DeveLancarUserFriendlyException()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.ModelState.AddModelError("Provider", "Required");

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                controller.SendTwoFactorAuthCode(new SendTwoFactorAuthCodeModel { UserId = user.Id, Provider = "Email" }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ProviderNaoEmail_Quando_SendTwoFactorAuthCode_Entao_DeveGerarCodigoSemEnviarEmail()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);

            var cacheManager = new AbpMemoryCacheManager(Substitute.For<Abp.Runtime.Caching.Configuration.ICachingConfiguration>());
            var cacheKey = new UserIdentifier(user.TenantId, user.Id).ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("old"));

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.SettingManager = CriarSettingManagerAsync();

            await Should.NotThrowAsync(() => controller.SendTwoFactorAuthCode(new SendTwoFactorAuthCodeModel
            {
                UserId = user.Id,
                Provider = "Phone"
            }));

            var emailSender = (IEmailSender)typeof(TokenAuthController).GetField("_emailSender", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(controller)!;
            await emailSender.DidNotReceive().SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());

            var cacheItem = await cacheManager.GetTwoFactorCodeCache().GetOrDefaultAsync(cacheKey);
            cacheItem.ShouldNotBeNull();
            cacheItem.Code.ShouldBe("123456");
        }

        private static UserManager CriarUserManagerSubstituto(User user)
        {
            var userStore = Substitute.For<UserStore>(new object[10]);
            var userRepository = Substitute.For<IRepository<User, long>>();
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var unitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var userManager = Substitute.For<UserManager>(
                userStore,
                userRepository,
                Options.Create(new IdentityOptions()),
                Substitute.For<IPasswordHasher<User>>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<Microsoft.Extensions.Logging.ILogger<UserManager>>(),
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

            userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);
            userManager.AddTokenValidityKeyAsync(user, Arg.Any<string>(), Arg.Any<DateTime>()).Returns(Task.CompletedTask);
            userManager.InitializeOptionsAsync(Arg.Any<int?>()).Returns(Task.CompletedTask);
            userManager.UpdateSecurityStampAsync(user).Returns(IdentityResult.Success);
            userManager.RemoveTokenValidityKeyAsync(user, Arg.Any<string>()).Returns(Task.CompletedTask);
            userManager.GetSecurityStampAsync(user).Returns("new-stamp");
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(user);
            userManager.GenerateTwoFactorTokenAsync(user, Arg.Any<string>()).Returns("123456");
            userManager.GetEmailAsync(user).Returns("user@example.com");
            userManager.CreateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            userManager.FindByNameOrEmailAsync(Arg.Any<string>()).Returns((User?)null);

            return userManager;
        }

        private static LogInManager CriarLogInManagerSubstituto(UserManager userManager, RoleManager roleManager, AbpLoginResult<Eaf.Middleware.MultiTenancy.Tenant, User> result)
        {
            var logInManager = Substitute.For<LogInManager>(
                userManager,
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<IRepository<Eaf.Middleware.MultiTenancy.Tenant>>(),
                IdentityTestHelper.CreateUnitOfWorkManager(),
                CriarSettingManagerAsync(),
                Substitute.For<IRepository<UserLoginAttempt, long>>(),
                Substitute.For<IUserManagementConfig>(),
                Substitute.For<IIocResolver>(),
                roleManager,
                Substitute.For<IPasswordHasher<User>>(),
                IdentityTestHelper.CreateUserClaimsPrincipalFactory(userManager, roleManager));

            logInManager.LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(result);
            logInManager.LoginAsync(Arg.Any<UserLoginInfo>(), Arg.Any<string>()).Returns(result);
            return logInManager;
        }

        private static IAbpSession CriarAbpSession(User user)
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(user.Id);
            abpSession.TenantId.Returns(user.TenantId);
            return abpSession;
        }

        private static ISettingManager CriarSettingManagerAsync()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return "1";
                return "false";
            });
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return Task.FromResult("1");
                return Task.FromResult("false");
            });
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(Task.FromResult("false"));
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerComLoginUnico()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return "1";
                if (name == AppSettings.UserManagement.AllowOneConcurrentLoginPerUser)
                    return "true";
                return "false";
            });
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return Task.FromResult("1");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerParaProvider(string enabledSettingName)
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                return name == enabledSettingName ? "true" : "false";
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerComTwoFactorHabilitado()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns("false");
            settingManager.GetSettingValue(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return "1";
                return "false";
            });
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return Task.FromResult("1");
                if (name == AppSettings.UserManagement.TwoFactorLogin.IsEnabled)
                    return Task.FromResult("true");
                return Task.FromResult("false");
            });
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(Task.FromResult("false"));
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerParaTeams(bool enabled, string hostSettings = "")
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns("false");
            settingManager.GetSettingValue(Arg.Any<string>()).Returns("false");
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Host.Microsoft)
                    return Task.FromResult(hostSettings);
                if (name == AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled)
                    return Task.FromResult(enabled ? "true" : "false");
                return Task.FromResult("false");
            });
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return Task.FromResult("1");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerParaExternalProviders()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled)
                    return "true";
                return "false";
            });
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(Task.FromResult("false"));
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return Task.FromResult("1");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static IObjectMapper CriarObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<ExternalLoginProviderInfoModel>>(Arg.Any<List<ExternalLoginProviderInfo>>())
                .Returns(new List<ExternalLoginProviderInfoModel>
                {
                    new ExternalLoginProviderInfoModel { ClientId = "client-id", Name = "Test", TenantId = "1" }
                });
            return objectMapper;
        }
    }
}
