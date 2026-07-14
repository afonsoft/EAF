using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Timing;
using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Host;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Security;
using Eaf.Middleware.Timing;
using Castle.Core.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Abp.Zero.Configuration;

namespace Eaf.Middleware.Application.Tests.Configuration.Host
{
    public class HostSettingsAppServiceBddTests
    {
        private static HostSettingsAppService CreateSut(
            out ISettingManager settingManager,
            out ISettingDefinitionManager settingDefinitionManager,
            out ITimeZoneService timeZoneService)
        {
            return CreateSut(out settingManager, out settingDefinitionManager, out timeZoneService, out _, out _, out _);
        }

        private static HostSettingsAppService CreateSut(
            out ISettingManager settingManager,
            out ISettingDefinitionManager settingDefinitionManager,
            out ITimeZoneService timeZoneService,
            out IEmailSender emailSender,
            out IEafMiddlewareAzureActiveDirectoryModuleConfig azureConfig,
            out IEafMiddlewareLdapModuleConfig ldapConfig)
        {
            settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>(), Arg.Any<bool>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));

            settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            settingDefinitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci => new SettingDefinition(ci.Arg<string>(), GetSettingValue(ci.Arg<string>())));

            timeZoneService = Substitute.For<ITimeZoneService>();
            timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>()).Returns("UTC");

            azureConfig = Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
            ldapConfig = Substitute.For<IEafMiddlewareLdapModuleConfig>();

            emailSender = Substitute.For<IEmailSender>();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            var sut = new HostSettingsAppService(
                emailSender,
                timeZoneService,
                settingDefinitionManager,
                azureConfig,
                ldapConfig
            )
            {
                AbpSession = abpSession,
                SettingManager = settingManager,
                Logger = Substitute.For<ILogger>()
            };

            return sut;
        }

        private static string GetSettingValue(string name)
        {
            if (name.Contains("ExternalLoginProvider.Host"))
            {
                return "{}";
            }

            if (name.Contains("MappedClaims"))
            {
                return "";
            }

            if (name.Contains("Port") ||
                name.Contains("Length") ||
                name.Contains("Seconds") ||
                name.Contains("Quantity") ||
                name.Contains("Days") ||
                name.Contains("Attempts") ||
                name.Contains("Expiration"))
            {
                return "0";
            }

            if (name.Contains("Is") ||
                name.Contains("Enabled") ||
                name.Contains("Enable") ||
                name.Contains("Require") ||
                name.Contains("Allow") ||
                name.Contains("UseCaptcha") ||
                name.Contains("UseDefault") ||
                name.Contains("Token"))
            {
                return "false";
            }

            return "false";
        }

        [Fact]
        public async Task Dado_ConfiguracoesPadrao_Quando_GetAllSettingsAnonymous_Entao_DeveRetornarConfiguracoesDoHost()
        {
            var sut = CreateSut(out _, out _, out _);

            var result = await sut.GetAllSettingsAnonymous();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioAdmin_Quando_GetAllSettings_Entao_DeveRetornarConfiguracoesCompletas()
        {
            var sut = CreateSut(out _, out _, out _);

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.Security.ShouldNotBeNull();
            result.UserManagement.ShouldNotBeNull();
            result.Email.ShouldNotBeNull();
            result.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.AzureActiveDirectory.ShouldNotBeNull();
            result.Ldap.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ConfiguracoesValidas_Quando_UpdateAllSettings_Entao_DeveAtualizarSemErros()
        {
            var sut = CreateSut(out _, out _, out _);

            var input = CreateValidInput();

            await sut.UpdateAllSettings(input);

            true.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_TimezoneIgualAoPadrao_Quando_GetAllSettings_Entao_TimezoneDeveRetornarVazio()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone).Returns("UTC");

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.General.ShouldNotBeNull();
            result.General.Timezone.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_ErroNoLogDeleter_Quando_GetAllSettings_Entao_DeveRetornarValoresPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.LogDeleter.ShouldNotBeNull();
            result.LogDeleter.DeletedQuantity.ShouldBe(30000);
            result.LogDeleter.Enabled.ShouldBe(true);
            result.LogDeleter.ExpiredDays.ShouldBe(3);
        }

        [Fact]
        public async Task Dado_ErroNoExternalLoginProvider_Quando_GetAllSettingsAnonymous_Entao_DeveRetornarConfiguracaoPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Google)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettingsAnonymous();

            result.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google.ShouldNotBeNull();
            result.ExternalLoginProviderSettings.Google_IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_InputComTimezoneVazioEUseDefaultPasswordComplexity_Quando_UpdateAllSettings_Entao_DeveAtualizarConfiguracoes()
        {
            var sut = CreateSut(out var settingManager, out _, out var timeZoneService);
            var input = CreateValidInput();
            input.General.Timezone = string.Empty;
            input.Security.UseDefaultPasswordComplexitySettings = true;
            input.Security.DefaultPasswordComplexity = new PasswordComplexitySetting
            {
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = false,
                RequiredLength = 0
            };

            var originalProvider = Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Clock.Provider = clockProvider;

            try
            {
                await sut.UpdateAllSettings(input);
            }
            finally
            {
                Clock.Provider = originalProvider;
            }

            await settingManager.Received(1).ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, "UTC");
            await timeZoneService.Received(1).GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>());
            await settingManager.Received(1).ChangeSettingForApplicationAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_ModulosAdLdapHabilitados_Quando_UpdateAllSettings_Entao_DeveRemoverUsuariosDoAuthSource()
        {
            var sut = CreateSut(out _, out _, out _, out _, out var azureConfig, out var ldapConfig);
            azureConfig.IsEnabled.Returns(true);
            ldapConfig.IsEnabled.Returns(true);

            var input = CreateValidInput();
            input.AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto { IsEnabled = false };
            input.Ldap = new LdapSettingsEditDto { IsEnabled = false };

            var userManager = ManagerTestHelper.CreateUserManager();
            var adUser = new User { Id = 1, UserName = "aduser", AuthenticationSource = "ActiveDirectory" };
            var ldapUser = new User { Id = 2, UserName = "ldapuser", AuthenticationSource = "LDAP" };
            userManager.Users.Returns(new List<User> { adUser, ldapUser }.AsQueryable());
            sut.UserManager = userManager;

            await sut.UpdateAllSettings(input);

            await userManager.Received(1).DeleteAsync(Arg.Is<User>(u => u.AuthenticationSource == "ActiveDirectory"));
            await userManager.Received(1).DeleteAsync(Arg.Is<User>(u => u.AuthenticationSource == "LDAP"));
        }

        [Fact]
        public async Task Dado_EmailValido_Quando_SendTestEmail_Entao_DeveEnviarEmail()
        {
            var sut = CreateSut(out _, out _, out _, out var emailSender, out _, out _);

            await sut.SendTestEmail(new SendTestEmailInput { EmailAddress = "test@example.com" });

            await emailSender.Received(1).SendAsync(
                "test@example.com",
                "TestEmail_Subject",
                "TestEmail_Body"
            );
        }

        [Fact]
        public async Task Dado_ErroNoLoginImpersonator_Quando_GetAllSettings_Entao_DeveRetornarValorPadrao()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            settingManager.GetSettingValueForApplicationAsync(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled)
                .ThrowsAsync(new Exception("fail"));

            var result = await sut.GetAllSettings();

            result.ShouldNotBeNull();
            result.LoginImpersonator.ShouldNotBeNull();
            result.LoginImpersonator.Enabled.ShouldBe(true);
        }

        [Fact]
        public async Task Dado_TimezonePreenchido_Quando_UpdateAllSettings_Entao_DeveUsarTimezoneFornecido()
        {
            var sut = CreateSut(out var settingManager, out _, out _);
            var input = CreateValidInput();
            input.General.Timezone = "America/Sao_Paulo";

            var originalProvider = Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Clock.Provider = clockProvider;

            try
            {
                await sut.UpdateAllSettings(input);
            }
            finally
            {
                Clock.Provider = originalProvider;
            }

            await settingManager.Received(1).ChangeSettingForApplicationAsync(TimingSettingNames.TimeZone, "America/Sao_Paulo");
        }

        private static HostSettingsEditDto CreateValidInput()
        {
            return new HostSettingsEditDto
            {
                General = new GeneralSettingsEditDto { Timezone = "UTC" },
                UserManagement = new HostUserManagementSettingsEditDto
                {
                    AllowOneConcurrentLoginPerUser = false,
                    IsCookieConsentEnabled = false,
                    IsEmailConfirmationRequiredForLogin = false,
                    IsRegisterRequiredForLogin = false,
                    StoreExternalTokenInformation = false,
                    TokenExpiration = 0,
                    UseCaptchaOnLogin = false
                },
                Security = new SecuritySettingsEditDto
                {
                    AllowOneConcurrentLoginPerUser = false,
                    UseDefaultPasswordComplexitySettings = false,
                    PasswordComplexity = new PasswordComplexitySetting
                    {
                        RequireDigit = false,
                        RequireLowercase = false,
                        RequireNonAlphanumeric = false,
                        RequireUppercase = false,
                        RequiredLength = 0
                    },
                    UserLockOut = new UserLockOutSettingsEditDto
                    {
                        IsEnabled = false,
                        DefaultAccountLockoutSeconds = 0,
                        MaxFailedAccessAttemptsBeforeLockout = 0
                    },
                    TwoFactorLogin = new TwoFactorLoginSettingsEditDto
                    {
                        IsEnabled = false,
                        IsEmailProviderEnabled = false,
                        IsSmsProviderEnabled = false,
                        IsRememberBrowserEnabled = false,
                        IsEnabledForApplication = false,
                        IsGoogleAuthenticatorEnabled = false
                    }
                },
                Email = new EmailSettingsEditDto
                {
                    DefaultFromAddress = "test@eaf.com",
                    DefaultFromDisplayName = "EAF",
                    SmtpDomain = "",
                    SmtpEnableSsl = false,
                    SmtpHost = "smtp.eaf.com",
                    SmtpPassword = "password",
                    SmtpPort = 587,
                    SmtpUseDefaultCredentials = false,
                    SmtpUserName = "user"
                },
                Google = new GoogleSettingsEditDto
                {
                    Analytics = "",
                    RecaptchaSiteKey = "",
                    Tag = ""
                },
                ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto
                {
                    Google = new GoogleExternalLoginProviderSettings(),
                    Google_IsEnabled = false,
                    Microsoft = new MicrosoftExternalLoginProviderSettings(),
                    Microsoft_IsEnabled = false,
                    OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                    OpenIdConnect_IsEnabled = false,
                    OpenIdConnectClaimsMapping = new List<JsonClaimMapDto>(),
                    AuthZero = new AuthZeroExternalLoginProviderSettings(),
                    AuthZero_IsEnabled = false
                },
                LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto
                {
                    DeletedQuantity = 30000,
                    Enabled = true,
                    ExpiredDays = 3
                },
                LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto
                {
                    Enabled = true
                }
            };
        }
    }
}
