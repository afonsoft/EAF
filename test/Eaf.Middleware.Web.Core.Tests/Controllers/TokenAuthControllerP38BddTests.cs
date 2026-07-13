using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.ObjectMapping;
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
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public partial class TokenAuthControllerBddTests
    {
        #region Helpers

        private static void ConfigurarTokenAuthConfiguration(TokenAuthController controller)
        {
            var config = GetField<TokenAuthConfiguration>(controller, "_configuration");
            var keyBytes = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
            config.SecurityKey = new SymmetricSecurityKey(keyBytes);
            config.SigningCredentials = new SigningCredentials(config.SecurityKey, SecurityAlgorithms.HmacSha256);
            config.Issuer = "test";
            config.Audience = "test";
        }

        private static void ConfigurarJwtOptions(TokenAuthController controller)
        {
            var jwtOptions = new JwtBearerOptions();
            jwtOptions.SecurityTokenValidators.Add(new FakeSecurityTokenValidator());
            SetField(controller, "_jwtOptions", Options.Create(jwtOptions));
        }

        private static void ConfigurarTenantCache(TokenAuthController controller, string tenancyName)
        {
            var tenantCache = GetField<ITenantCache>(controller, "_tenantCache");
            tenantCache.GetOrNull(1).Returns(new TenantCacheItem { Id = 1, Name = "Default", TenancyName = tenancyName });
        }

        private static UserManager CriarUserManagerParaP38(User user)
        {
            var userManager = CriarUserManagerSubstituto(user);
            userManager.CreateAsync(Arg.Any<User>()).Returns(Task.FromResult(IdentityResult.Success));
            userManager.FindByNameOrEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<User>(null));
            userManager.GetValidTwoFactorProvidersAsync(Arg.Any<User>()).Returns(Task.FromResult<IList<string>>(new List<string> { "Email" }));
            return userManager;
        }

        private static IObjectMapper CriarObjectMapperMapeado()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<ExternalLoginProviderInfoModel>>(Arg.Any<List<ExternalLoginProviderInfo>>())
                .Returns(callInfo =>
                {
                    var input = callInfo.Arg<List<ExternalLoginProviderInfo>>();
                    if (input == null)
                        return new List<ExternalLoginProviderInfoModel>();

                    return input.Select(p => new ExternalLoginProviderInfoModel
                    {
                        Name = p.Name,
                        ClientId = p.ClientId,
                        TenantId = p.TenantId
                    }).ToList();
                });
            return objectMapper;
        }

        private static ISettingManager CriarSettingManagerTwoFactor()
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
                if (name == AppSettings.UserManagement.TwoFactorLogin.IsEnabled)
                    return Task.FromResult("true");
                if (name == AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled)
                    return Task.FromResult("true");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerCaptcha()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValue(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.UserManagement.TokenExpiration)
                    return "1";
                if (name == AppSettings.UserManagement.UseCaptchaOnLogin)
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

        private static ISettingManager CriarSettingManagerExternal()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled)
                    return "true";
                if (name == AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled)
                    return "true";
                if (name == AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled)
                    return "true";
                if (name == AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled)
                    return "true";
                return "false";
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerMicrosoftTeams()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled)
                    return Task.FromResult("true");
                if (name == AppSettings.ExternalLoginProvider.Host.Microsoft)
                    return Task.FromResult("{}");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerMicrosoftTeamsDesabilitado()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled)
                    return Task.FromResult("false");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static ISettingManager CriarSettingManagerMicrosoftTeamsVazio()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled)
                    return Task.FromResult("true");
                if (name == AppSettings.ExternalLoginProvider.Host.Microsoft)
                    return Task.FromResult("");
                return Task.FromResult("false");
            });
            return settingManager;
        }

        private static IExternalAuthConfiguration CriarExternalAuthConfiguration(params ExternalLoginProviderInfo[] providers)
        {
            var infoProviders = new List<IExternalLoginInfoProvider>();
            foreach (var provider in providers)
            {
                var infoProvider = Substitute.For<IExternalLoginInfoProvider>();
                infoProvider.GetExternalLoginInfo().Returns(provider);
                infoProviders.Add(infoProvider);
            }

            var externalAuthConfiguration = Substitute.For<IExternalAuthConfiguration>();
            externalAuthConfiguration.ExternalLoginInfoProviders.Returns(infoProviders);
            return externalAuthConfiguration;
        }

        private static IIocManager CriarIocManager(DefaultExternalLoginInfoManager externalLoginInfoManager)
        {
            var iocManager = Substitute.For<IIocManager>();
            iocManager.Resolve<DefaultExternalLoginInfoManager>().Returns(externalLoginInfoManager);
            iocManager.Resolve(typeof(DefaultExternalLoginInfoManager), Arg.Any<object>()).Returns(externalLoginInfoManager);
            return iocManager;
        }

        private static IBinaryObjectManager CriarBinaryObjectManager(BinaryObject existingBinary)
        {
            var binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            binaryObjectManager.GetOrNullAsync(Arg.Any<Guid>()).Returns(Task.FromResult(existingBinary));
            binaryObjectManager.SaveAsync(Arg.Any<BinaryObject>()).Returns(Task.CompletedTask);
            binaryObjectManager.DeleteAsync(Arg.Any<Guid>()).Returns(Task.CompletedTask);
            return binaryObjectManager;
        }

        private static DefaultExternalLoginInfoManager CriarExternalLoginInfoManager()
        {
            var manager = Substitute.For<DefaultExternalLoginInfoManager>();
            manager.GetUserNameFromExternalAuthUserInfo(Arg.Any<ExternalAuthUserInfo>()).Returns("testuser");
            return manager;
        }

        private static IExternalAuthManager CriarExternalAuthManager(ExternalAuthUserInfo userInfo)
        {
            var externalAuthManager = Substitute.For<IExternalAuthManager>();
            externalAuthManager.GetUserInfo(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(userInfo));
            return externalAuthManager;
        }

        private static T GetField<T>(object obj, string name)
        {
            var field = typeof(TokenAuthController).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            field.ShouldNotBeNull();
            return (T)field.GetValue(obj);
        }

        private static void SetField<T>(object obj, string name, T value)
        {
            var field = typeof(TokenAuthController).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            field.ShouldNotBeNull();
            field.SetValue(obj, value);
        }

        private class FakeSecurityTokenValidator : ISecurityTokenValidator
        {
            public int MaximumTokenSizeInBytes { get; set; } = 1024 * 1024;

            public bool CanValidateToken => true;

            public bool CanReadToken(string securityToken) => true;

            public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
            {
                validatedToken = null;
                return new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(EafClaimTypes.UserIdentifierClaimType, "1@1")
                }));
            }
        }

        #endregion

        #region Two Factor Authentication

        [Fact]
        public async Task Dado_UsuarioComTwoFactor_Quando_AuthenticateSemCodigo_Entao_DeveRetornarRequiresTwoFactor()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();
            ConfigurarTokenAuthConfiguration(controller);

            // Quando
            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password"
            });

            // Então
            result.ShouldNotBeNull();
            result.RequiresTwoFactorVerification.ShouldBeTrue();
            result.TwoFactorAuthProviders.ShouldContain("Email");
        }

        [Fact]
        public async Task Dado_UsuarioComTwoFactor_Quando_AuthenticateComCodigoValido_Entao_DeveRetornarAccessToken()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());
            var cacheKey = new UserIdentifier(user.TenantId, user.Id).ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("123456"));

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();
            ConfigurarTokenAuthConfiguration(controller);

            // Quando
            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password",
                TwoFactorVerificationCode = "123456",
                RememberClient = true
            });

            // Então
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.TwoFactorRememberClientToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        [Fact]
        public async Task Dado_UsuarioComTwoFactor_Quando_AuthenticateComCodigoInvalido_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());
            var cacheKey = new UserIdentifier(user.TenantId, user.Id).ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("123456"));

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();
            ConfigurarTokenAuthConfiguration(controller);

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.Authenticate(new AuthenticateModel
                {
                    UserNameOrEmailAddress = "admin",
                    Password = "password",
                    TwoFactorVerificationCode = "999999"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioComTwoFactorEClientRemembered_Quando_Authenticate_Entao_DeveRetornarAccessToken()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.IsTwoFactorEnabled = true;
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();
            ConfigurarTokenAuthConfiguration(controller);
            ConfigurarJwtOptions(controller);

            // Quando
            var result = await controller.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "password",
                TwoFactorRememberClientToken = "any-token"
            });

            // Então
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        #endregion

        #region Captcha

        [Fact]
        public async Task Dado_CaptchaHabilitado_Quando_Authenticate_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerCaptcha();
            ConfigurarTokenAuthConfiguration(controller);

            var recaptchaValidator = Substitute.For<IRecaptchaValidator>();
            recaptchaValidator.When(x => x.ValidateAsync(Arg.Any<string>()))
                .Do(x => throw new UserFriendlyException("InvalidCaptcha"));
            controller.RecaptchaValidator = recaptchaValidator;

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.Authenticate(new AuthenticateModel
                {
                    UserNameOrEmailAddress = "admin",
                    Password = "password",
                    CaptchaResponse = "invalid-captcha"
                }));

            exception.ShouldNotBeNull();
        }

        #endregion

        #region External Authenticate

        [Fact]
        public async Task Dado_LoginExternoUnknown_NovoUsuarioInativo_Quando_ExternalAuthenticate_Entao_DeveRetornarWaitingForActivation()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var unknownLoginResult = new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin, tenant, user);

            var userManager = CriarUserManagerParaP38(user);
            userManager.CreateAsync(Arg.Any<User>()).Returns(callInfo =>
            {
                callInfo.Arg<User>().IsActive = false;
                return Task.FromResult(IdentityResult.Success);
            });

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, unknownLoginResult);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "testuser@example.com",
                Picture = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 })
            };

            var externalAuthManager = CriarExternalAuthManager(externalUserInfo);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);

            var externalLoginInfoManager = CriarExternalLoginInfoManager();
            SetField(controller, "_iocManager", CriarIocManager(externalLoginInfoManager));
            SetField(controller, "_binaryObjectManager", CriarBinaryObjectManager(null));

            // Quando
            var result = await controller.ExternalAuthenticate(new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ProviderKey = "provider-key",
                ProviderAccessCode = "access-code"
            });

            // Então
            result.ShouldNotBeNull();
            result.WaitingForActivation.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_LoginExternoUnknown_NovoUsuarioAtivo_Quando_ExternalAuthenticate_Entao_DeveRetornarAccessTokenComReturnUrl()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var unknownLoginResult = new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin, tenant, user);
            var successLoginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, unknownLoginResult);
            logInManager.LoginAsync(Arg.Any<UserLoginInfo>(), Arg.Any<string>()).Returns(unknownLoginResult, successLoginResult);

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

            var externalLoginInfoManager = CriarExternalLoginInfoManager();
            SetField(controller, "_iocManager", CriarIocManager(externalLoginInfoManager));

            // Quando
            var result = await controller.ExternalAuthenticate(new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ProviderKey = "provider-key",
                ProviderAccessCode = "access-code",
                SingleSignIn = true,
                ReturnUrl = "https://example.com"
            });

            // Então
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.ReturnUrl.ShouldContain("accessToken=");
            result.ReturnUrl.ShouldContain("userId=");
            result.ReturnUrl.ShouldContain("tenantId=");
        }

        [Fact]
        public async Task Dado_LoginExternoComProviderKeyDiferente_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "different-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "testuser@example.com"
            };

            var externalAuthManager = CriarExternalAuthManager(externalUserInfo);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.ExternalAuthenticate(new ExternalAuthenticateModel
                {
                    AuthProvider = "Microsoft",
                    ProviderKey = "provider-key",
                    ProviderAccessCode = "access-code"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_LoginExternoSuccess_ComFotoDiferente_Quando_ExternalAuthenticate_Entao_DeveRetornarAccessToken()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.ProfilePictureId = Guid.NewGuid();
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });
            var loginResult = new AbpLoginResult<Tenant, User>(tenant, user, identity);

            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, loginResult);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Name = "Admin User",
                Surname = "User",
                EmailAddress = "testuser@example.com",
                Picture = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 })
            };

            var externalAuthManager = CriarExternalAuthManager(externalUserInfo);
            var controller = CriarController(userManager, roleManager, logInManager, externalAuthManager: externalAuthManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);

            var existingBinary = new BinaryObject(null, new byte[] { 5, 4, 3, 2, 1 }, ".bmp", "old.bmp");
            SetField(controller, "_binaryObjectManager", CriarBinaryObjectManager(existingBinary));

            // Quando
            var result = await controller.ExternalAuthenticate(new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ProviderKey = "provider-key",
                ProviderAccessCode = "access-code"
            });

            // Então
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.UserId.ShouldBe(user.Id);
        }

        #endregion

        #region Teams Authenticate

        [Fact]
        public async Task Dado_MicrosoftDesabilitado_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManagerMicrosoftTeamsDesabilitado();

            // Quando & Então
            var exception = await Should.ThrowAsync<AbpException>(async () =>
                await controller.TeamsAuthenticate("id-token"));

            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("Microsoft Provider is not enabled");
        }

        [Fact]
        public async Task Dado_MicrosoftVazio_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManagerMicrosoftTeamsVazio();

            // Quando & Então
            var exception = await Should.ThrowAsync<AbpException>(async () =>
                await controller.TeamsAuthenticate("id-token"));

            exception.ShouldNotBeNull();
            exception.Message.ShouldContain("Microsoft Provider is not configured");
        }

        [Fact]
        public async Task Dado_MicrosoftConfiguradoInvalido_Quando_TeamsAuthenticate_Entao_DeveLancarException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManagerMicrosoftTeams();

            // Quando & Então
            var exception = await Should.ThrowAsync<Exception>(async () =>
                await controller.TeamsAuthenticate("id-token"));

            exception.ShouldNotBeNull();
        }

        #endregion

        #region LogOut

        [Fact]
        public async Task Dado_ErroNosTresCaminhos_Quando_LogOut_Entao_DeveCompletarSemErro()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            userManager.FindByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                        {
                            new Claim(MiddlewareCoreConsts.UserIdentifier, user.ToUserIdentifier().ToUserIdentifierString()),
                            new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key")
                        }))
                }
            };

            var principalAccessor = Substitute.For<IPrincipalAccessor>();
            principalAccessor.Principal.Returns(x => throw new Exception("test"));
            SetField(controller, "_principalAccessor", principalAccessor);

            // Quando & Então
            await Should.NotThrowAsync(controller.LogOut());
        }

        #endregion

        #region Send Two Factor Auth Code

        [Fact]
        public async Task Dado_CacheItemNulo_Quando_SendTwoFactorAuthCode_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            // Quando & Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await controller.SendTwoFactorAuthCode(new SendTwoFactorAuthCodeModel
                {
                    UserId = user.Id,
                    Provider = "Email"
                }));

            exception.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ProviderPhone_Quando_SendTwoFactorAuthCode_Entao_DeveCompletarSemErro()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());
            var cacheKey = new UserIdentifier(user.TenantId, user.Id).ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("old"));

            var controller = CriarController(userManager, roleManager, logInManager, cacheManager: cacheManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();

            // Quando & Então
            await Should.NotThrowAsync(() => controller.SendTwoFactorAuthCode(new SendTwoFactorAuthCodeModel
            {
                UserId = user.Id,
                Provider = "Phone"
            }));
        }

        #endregion

        #region Authentication Providers

        [Fact]
        public void Dado_UsuarioComAuthenticationSourceNulo_Quando_GetAuthenticationProviders_Entao_DeveRetornarSystem()
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
        public void Dado_UsuarioNaoInformado_Quando_GetAuthenticationProviders_Entao_DeveRetornarSystemProvider()
        {
            // Dado
            var userManager = IdentityTestHelper.CreateUserManager();
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            // Quando
            var result = controller.GetAuthenticationProviders(null);

            // Então
            result.ShouldNotBeNull();
            result.AuthenticationSource.ShouldBe("System");
            result.UsernameOrEmailAddress.ShouldBeNull();
            result.Tenant.ShouldNotBeNull();
            result.Tenant.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_OpenIdConnectHabilitado_Quando_GetExternalAuthenticationProviders_Entao_DeveRetornarProvider()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.ObjectMapper = CriarObjectMapperMapeado();
            controller.SettingManager = CriarSettingManagerExternal();
            SetField(controller, "_settingManager", CriarSettingManagerExternal());

            var provider = new ExternalLoginProviderInfo(
                name: "OpenIdConnect",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            SetField(controller, "_externalAuthConfiguration", CriarExternalAuthConfiguration(provider));

            // Quando
            var result = controller.GetExternalAuthenticationProviders();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.First().Name.ShouldBe("OpenIdConnect");
        }

        [Fact]
        public void Dado_SessaoNulaEClientIdPreenchido_Quando_GetExternalAuthenticationProviders_Entao_DeveRetornarProvider()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.ObjectMapper = CriarObjectMapperMapeado();
            controller.SettingManager = CriarSettingManagerExternal();
            SetField(controller, "_settingManager", CriarSettingManagerExternal());

            var provider = new ExternalLoginProviderInfo(
                name: "Microsoft",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            SetField(controller, "_externalAuthConfiguration", CriarExternalAuthConfiguration(provider));

            // Quando
            var result = controller.GetExternalAuthenticationProviders();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.First().Name.ShouldBe("Microsoft");
        }

        [Fact]
        public void Dado_ClientIdVazio_Quando_GetExternalAuthenticationProviders_Entao_DeveRetornarListaVazia()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.ObjectMapper = CriarObjectMapperMapeado();
            controller.SettingManager = CriarSettingManagerExternal();

            var provider = new ExternalLoginProviderInfo(
                name: "Google",
                clientId: string.Empty,
                clientSecret: string.Empty,
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            SetField(controller, "_externalAuthConfiguration", CriarExternalAuthConfiguration(provider));

            // Quando
            var result = controller.GetExternalAuthenticationProviders();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        #endregion

        #region Reflection Helpers

        [Fact]
        public void Dado_GoogleHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarGoogle()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = CriarSettingManager();
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled).Returns("true");
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("Google");
        }

        [Fact]
        public void Dado_AuthZeroHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarAuthZero()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = CriarSettingManager();
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled).Returns("true");
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("AuthZero");
        }

        [Fact]
        public void Dado_OpenIdConnectHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarOpenIdConnect()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = CriarSettingManager();
            settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled).Returns("true");
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("OpenIdConnect");
        }

        [Fact]
        public void Dado_TenantDefinido_Quando_GetTenancyNameOrNull_Entao_DeveRetornarTenancyName()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            ConfigurarTenantCache(controller, "DefaultTenant");

            var method = typeof(TokenAuthController).GetMethod("GetTenancyNameOrNull", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBe("DefaultTenant");
        }

        [Fact]
        public void Dado_TenantNulo_Quando_GetTenancyNameOrNull_Entao_DeveRetornarNulo()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            controller.AbpSession = abpSession;

            var method = typeof(TokenAuthController).GetMethod("GetTenancyNameOrNull", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            // Quando
            var result = method.Invoke(controller, null);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_OpenIdConnectHabilitado_Quando_IsSchemeEnabled_Entao_DeveRetornarTrue()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.SettingManager = CriarSettingManagerExternal();
            SetField(controller, "_settingManager", CriarSettingManagerExternal());

            var method = typeof(TokenAuthController).GetMethod("IsSchemeEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var scheme = new ExternalLoginProviderInfo(
                name: "OpenIdConnect",
                clientId: "client-id",
                clientSecret: "client-secret",
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            // Quando
            var result = method.Invoke(controller, new object[] { scheme });

            // Então
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ClientIdVazio_Quando_IsSchemeEnabled_Entao_DeveRetornarFalse()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.SettingManager = CriarSettingManagerExternal();

            var method = typeof(TokenAuthController).GetMethod("IsSchemeEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var scheme = new ExternalLoginProviderInfo(
                name: "Google",
                clientId: string.Empty,
                clientSecret: string.Empty,
                tenantId: "1",
                providerApiType: typeof(object),
                additionalParams: new Dictionary<string, string>(),
                claimMappings: new List<JsonClaimMap>());

            // Quando
            var result = method.Invoke(controller, new object[] { scheme });

            // Então
            result.ShouldBe(false);
        }

        [Fact]
        public void Dado_ArraysIguais_Quando_ByteArrayCompare_Entao_DeveRetornarTrue()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);

            var method = typeof(TokenAuthController).GetMethod("ByteArrayCompare", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var a1 = new byte[] { 1, 2, 3 };
            var a2 = new byte[] { 1, 2, 3 };

            // Quando
            var result = method.Invoke(null, new object[] { a1, a2 });

            // Então
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ArraysDiferentes_Quando_ByteArrayCompare_Entao_DeveRetornarFalse()
        {
            // Dado
            var user = IdentityTestHelper.CreateUser();
            var userManager = CriarUserManagerParaP38(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null!);
            var controller = CriarController(userManager, roleManager, logInManager);

            var method = typeof(TokenAuthController).GetMethod("ByteArrayCompare", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var a1 = new byte[] { 1, 2, 3 };
            var a2 = new byte[] { 1, 2, 4 };

            // Quando
            var result = method.Invoke(null, new object[] { a1, a2 });

            // Então
            result.ShouldBe(false);
        }

        [Fact]
        public void Dado_ProviderKeysIguais_Quando_ProviderKeysAreEqual_Entao_DeveRetornarTrue()
        {
            // Dado
            var method = typeof(TokenAuthController).GetMethod("ProviderKeysAreEqual", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var model = new ExternalAuthenticateModel { ProviderKey = "provider-key" };
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "provider-key" };

            // Quando
            var result = method.Invoke(null, new object[] { model, userInfo });

            // Então
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ProviderKeysDiferentes_Quando_ProviderKeysAreEqual_Entao_DeveRetornarFalse()
        {
            // Dado
            var method = typeof(TokenAuthController).GetMethod("ProviderKeysAreEqual", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var model = new ExternalAuthenticateModel { ProviderKey = "provider-key" };
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "different-key" };

            // Quando
            var result = method.Invoke(null, new object[] { model, userInfo });

            // Então
            result.ShouldBe(false);
        }

        [Fact]
        public void Dado_ReturnUrlSemQuery_Quando_AddSingleSignInParametersToReturnUrl_Entao_DeveAdicionarQueryString()
        {
            // Dado
            var method = typeof(TokenAuthController).GetMethod("AddSingleSignInParametersToReturnUrl", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            // Quando
            var result = (string)method.Invoke(null, new object[] { "https://example.com", "signin-token", 1, 1 })!;

            // Então
            result.ShouldContain("?");
            result.ShouldContain("signin-token");
            result.ShouldContain("tenantId=");
        }

        [Fact]
        public void Dado_ReturnUrlComQuery_Quando_AddSingleSignInParametersToReturnUrl_Entao_DeveAdicionarAmpersand()
        {
            // Dado
            var method = typeof(TokenAuthController).GetMethod("AddSingleSignInParametersToReturnUrl", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            // Quando
            var result = (string)method.Invoke(null, new object[] { "https://example.com?existing=1", "signin-token", 1, 1 })!;

            // Então
            result.IndexOf("?").ShouldBeLessThan(result.IndexOf("&"));
            result.ShouldContain("signin-token");
            result.ShouldContain("tenantId=");
        }

        #endregion
    }
}
