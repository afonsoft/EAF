using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Memory;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Castle.MicroKernel.Registration;
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
        #region LogOut

        [Fact]
        public async Task Dado_PrincipalAccessorComClaimsValidas_Quando_LogOut_Entao_DeveRemoverTokenValidityKeyEAtualizarSecurityStamp()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null);
            var controller = CriarController(userManager, roleManager, logInManager);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            controller.AbpSession = abpSession;
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var principalAccessor = GetField<IPrincipalAccessor>(controller, "_principalAccessor");
            principalAccessor.Principal.Returns(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(MiddlewareCoreConsts.TokenValidityKey, "token-key"),
                    new Claim(MiddlewareCoreConsts.UserIdentifier, $"{user.Id}@{user.TenantId}")
                })));

            await Should.NotThrowAsync(controller.LogOut());

            await userManager.Received().RemoveTokenValidityKeyAsync(user, "token-key");
            await userManager.Received().UpdateSecurityStampAsync(user);
        }

        #endregion

        #region GetDefaultEnabledProvider

        [Fact]
        public void Dado_NenhumProviderHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarSystem()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManager();

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("System");
        }

        [Fact]
        public void Dado_ActiveDirectoryHabilitado_Quando_GetDefaultEnabledProvider_Entao_DeveRetornarActiveDirectory()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplication(Arg.Any<string>()).Returns(callInfo =>
            {
                var name = callInfo.Arg<string>();
                if (name == AzureActiveDirectorySettingNames.IsEnabled)
                    return "true";
                return "false";
            });
            controller.SettingManager = settingManager;

            var method = typeof(TokenAuthController).GetMethod("GetDefaultEnabledProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var result = method.Invoke(controller, null);
            result.ShouldBe("ActiveDirectory");
        }

        #endregion

        #region Helpers Privados

        [Fact]
        public void Dado_ChaveProviderIgual_Quando_ProviderKeysAreEqual_Entao_DeveRetornarVerdadeiro()
        {
            var method = typeof(TokenAuthController).GetMethod("ProviderKeysAreEqual", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var model = new ExternalAuthenticateModel { ProviderKey = "000-123" };
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "000-123" };

            var result = method.Invoke(null, new object[] { model, userInfo });
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ChaveProviderComFormatacaoDiferente_Quando_ProviderKeysAreEqual_Entao_DeveRetornarVerdadeiro()
        {
            var method = typeof(TokenAuthController).GetMethod("ProviderKeysAreEqual", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var model = new ExternalAuthenticateModel { ProviderKey = "001-23" };
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "000123" };

            var result = method.Invoke(null, new object[] { model, userInfo });
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ChaveProviderNula_Quando_ProviderKeysAreEqual_Entao_DeveRetornarFalso()
        {
            var method = typeof(TokenAuthController).GetMethod("ProviderKeysAreEqual", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var result1 = method.Invoke(null, new object[] { new ExternalAuthenticateModel { ProviderKey = "key" }, new ExternalAuthUserInfo { ProviderKey = null } });
            var result2 = method.Invoke(null, new object[] { new ExternalAuthenticateModel { ProviderKey = null }, new ExternalAuthUserInfo { ProviderKey = "key" } });

            result1.ShouldBe(false);
            result2.ShouldBe(false);
        }

        [Fact]
        public void Dado_ReturnUrlVazio_Quando_AddSingleSignInParametersToReturnUrl_Entao_DeveGerarUrlComParametros()
        {
            var method = typeof(TokenAuthController).GetMethod("AddSingleSignInParametersToReturnUrl", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var result = method.Invoke(null, new object[] { null, "sign-in-token", 1, (int?)null });

            result.ShouldBeOfType<string>();
            result.ToString().ShouldContain("accessToken=sign-in-token");
            result.ToString().ShouldContain("userId=");
            result.ToString().ShouldNotContain("tenantId=");
        }

        [Fact]
        public void Dado_ReturnUrlComTenantId_Quando_AddSingleSignInParametersToReturnUrl_Entao_DeveGerarUrlComTenantId()
        {
            var method = typeof(TokenAuthController).GetMethod("AddSingleSignInParametersToReturnUrl", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var result = method.Invoke(null, new object[] { "https://example.com?x=1", "sign-in-token", 1, 2 });

            result.ToString().ShouldContain("accessToken=sign-in-token");
            result.ToString().ShouldContain("tenantId=");
        }

        [Fact]
        public void Dado_ByteArraysIguais_Quando_ByteArrayCompare_Entao_DeveRetornarVerdadeiro()
        {
            var userManager = IdentityTestHelper.CreateUserManager();
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var controller = CriarController(userManager, roleManager, CriarLogInManagerSubstituto(userManager, roleManager, null));
            var method = typeof(TokenAuthController).GetMethod("ByteArrayCompare", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var a1 = new byte[] { 1, 2, 3 };
            var a2 = new byte[] { 1, 2, 3 };

            var result = method.Invoke(null, new object[] { a1, a2 });
            result.ShouldBe(true);
        }

        [Fact]
        public void Dado_ByteArraysDiferentes_Quando_ByteArrayCompare_Entao_DeveRetornarFalso()
        {
            var userManager = IdentityTestHelper.CreateUserManager();
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var controller = CriarController(userManager, roleManager, CriarLogInManagerSubstituto(userManager, roleManager, null));
            var method = typeof(TokenAuthController).GetMethod("ByteArrayCompare", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            var result1 = method.Invoke(null, new object[] { new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 4 } });
            var result2 = method.Invoke(null, new object[] { new byte[] { 1, 2 }, new byte[] { 1, 2, 3 } });
            var result3 = method.Invoke(controller, new object[] { null, new byte[] { 1, 2, 3 } });

            result1.ShouldBe(false);
            result2.ShouldBe(false);
            result3.ShouldBe(false);
        }

        #endregion

        #region CreateJwtClaims

        [Fact]
        public async Task Dado_IdentidadeValida_Quando_CreateJwtClaims_Entao_DeveRetornarClaimsComTokenValidity()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var tenant = new Tenant("Default", "Default") { Id = 1, IsActive = true };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) });

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerAsync();
            ConfigurarTokenAuthConfiguration(controller);

            var method = typeof(TokenAuthController).GetMethod("CreateJwtClaims", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<IEnumerable<Claim>>)method.Invoke(controller, new object[] { identity, user, "" });
            var claims = await task;

            claims.ShouldNotBeNull();
            claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Sub);
            claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Jti);
            claims.ShouldContain(c => c.Type == JwtRegisteredClaimNames.Iat);
            claims.ShouldContain(c => c.Type == MiddlewareCoreConsts.TokenValidityKey);
            claims.ShouldContain(c => c.Type == MiddlewareCoreConsts.UserIdentifier);
            claims.ShouldContain(c => c.Type == MiddlewareCoreConsts.TokenValidityValue);
        }

        #endregion

        #region Two Factor

        [Fact]
        public async Task Dado_CodigoTwoFactorValidoComRememberClient_Quando_TwoFactorAuthenticate_Entao_DeveRetornarToken()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();
            controller.SettingManager = CriarSettingManagerTwoFactor();
            ConfigurarTokenAuthConfiguration(controller);

            var cacheManager = new AbpMemoryCacheManager(Substitute.For<ICachingConfiguration>());
            var cacheKey = user.ToUserIdentifier().ToString();
            await cacheManager.GetTwoFactorCodeCache().SetAsync(cacheKey, new TwoFactorCodeCacheItem("123456"));

            SetField(controller, "_cacheManager", cacheManager);

            var method = typeof(TokenAuthController).GetMethod("TwoFactorAuthenticateAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<string>)method.Invoke(controller, new object[]
            {
                user,
                new AuthenticateModel { TwoFactorVerificationCode = "123456", RememberClient = true }
            });

            var result = await task;
            result.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Dado_UsuarioComTwoFactorSemRememberClient_Quando_IsTwoFactorAuthRequired_Entao_DeveRetornarVerdadeiro()
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
            controller.SettingManager = CriarSettingManagerTwoFactor();

            var method = typeof(TokenAuthController).GetMethod("IsTwoFactorAuthRequiredAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<bool>)method.Invoke(controller, new object[]
            {
                loginResult,
                new AuthenticateModel()
            });

            var result = await task;
            result.ShouldBe(true);
        }

        #endregion

        #region External Users

        [Fact]
        public async Task Dado_ExternalUserInfo_Quando_UpdateExternalUserAsync_Entao_DeveAtualizarNomeSobrenomeEFoto()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            user.Name = "Old";
            user.Surname = "Old";

            var userManager = CriarUserManagerSubstituto(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var externalUserInfo = new ExternalAuthUserInfo
            {
                Name = "Updated Name",
                Surname = "Surname",
                Provider = "Microsoft",
                Picture = "ZmFrZQ=="
            };

            var method = typeof(TokenAuthController).GetMethod("UpdateExternalUserAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task)method.Invoke(controller, new object[] { user, externalUserInfo });
            await task;

            user.Name.ShouldBe("Updated");
            user.Surname.ShouldBe("Surname");
            user.ExternalAuthProviderformation.ShouldBe("Microsoft");
            user.ProfilePictureId.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ExternalUserInfoNovoUsuario_Quando_RegisterExternalUserAsync_Entao_DeveCriarUsuario()
        {
            var user = IdentityTestHelper.CreateUser(securityStamp: "stamp-123");
            var userManager = CriarUserManagerSubstituto(user);
            userManager.CreateAsync(Arg.Any<User>()).Returns(Task.FromResult(IdentityResult.Success));
            userManager.FindByNameOrEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<User>(null));
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = CriarLogInManagerSubstituto(userManager, roleManager, null);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.AbpSession = CriarAbpSession(user);
            controller.UnitOfWorkManager = IdentityTestHelper.CreateUnitOfWorkManager();

            var iocManager = new IocManager();
            var externalLoginInfoManager = Substitute.For<DefaultExternalLoginInfoManager>();
            externalLoginInfoManager.GetUserNameFromExternalAuthUserInfo(Arg.Any<ExternalAuthUserInfo>()).Returns("newuser");
            iocManager.IocContainer.Register(Component.For<DefaultExternalLoginInfoManager>().Instance(externalLoginInfoManager));
            SetField(controller, "_iocManager", iocManager);

            var externalUserInfo = new ExternalAuthUserInfo
            {
                EmailAddress = "newuser@example.com",
                Name = "New User",
                Surname = "User",
                Provider = "Microsoft",
                ProviderKey = "provider-key",
                Picture = "ZmFrZQ=="
            };

            var method = typeof(TokenAuthController).GetMethod("RegisterExternalUserAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            method.ShouldNotBeNull();

            var task = (Task<User>)method.Invoke(controller, new object[] { externalUserInfo });
            var result = await task;

            result.ShouldNotBeNull();
            result.UserName.ShouldBe("newuser");
            result.EmailAddress.ShouldBe("newuser@example.com");
        }

        #endregion

        #region Teams

        [Fact]
        public async Task Dado_MicrosoftTeamsDesabilitado_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManagerMicrosoftTeamsDesabilitado();

            var exception = await Should.ThrowAsync<AbpException>(async () =>
                await controller.TeamsAuthenticate("id-token"));

            exception.Message.ShouldContain("Microsoft Provider is not enabled");
        }

        [Fact]
        public async Task Dado_MicrosoftTeamsNaoConfigurado_Quando_TeamsAuthenticate_Entao_DeveLancarAbpException()
        {
            var user = IdentityTestHelper.CreateUser();
            var userManager = IdentityTestHelper.CreateUserManager(user);
            var roleManager = IdentityTestHelper.CreateRoleManager();
            var logInManager = IdentityTestHelper.CreateApplicationLogInManager(userManager, roleManager);
            var controller = CriarController(userManager, roleManager, logInManager);
            controller.SettingManager = CriarSettingManagerMicrosoftTeamsVazio();

            var exception = await Should.ThrowAsync<AbpException>(async () =>
                await controller.TeamsAuthenticate("id-token"));

            exception.Message.ShouldContain("Microsoft Provider is not configured");
        }

        #endregion
    }
}
