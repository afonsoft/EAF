using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.MultiTenancy;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Castle.Core.Logging;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Configuration;
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
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public class TokenAuthControllerBddTests
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

        private static TokenAuthController CriarController(UserManager userManager, RoleManager roleManager, LogInManager logInManager)
        {
            var settingManager = CriarSettingManager();
            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { Id = 1, Name = "Default", TenancyName = "Default" });

            var controller = new TokenAuthController(
                logInManager,
                new AbpLoginResultTypeHelper(),
                new TokenAuthConfiguration(),
                userManager,
                roleManager,
                tenantCache,
                Substitute.For<ICacheManager>(),
                Substitute.For<IImpersonationManager>(),
                Options.Create(new IdentityOptions()),
                Substitute.For<ILogger>(),
                settingManager,
                Substitute.For<IExternalAuthManager>(),
                Substitute.For<IExternalAuthConfiguration>(),
                Substitute.For<IIocManager>(),
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
