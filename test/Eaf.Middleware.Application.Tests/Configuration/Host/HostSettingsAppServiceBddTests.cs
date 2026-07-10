using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Host;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Security;
using Eaf.Middleware.Timing;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration.Host
{
    public class HostSettingsAppServiceBddTests
    {
        private static HostSettingsAppService CreateSut(
            out ISettingManager settingManager,
            out ISettingDefinitionManager settingDefinitionManager,
            out ITimeZoneService timeZoneService)
        {
            settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>(), Arg.Any<bool>()).Returns(ci => GetSettingValue(ci.Arg<string>()));
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(ci => GetSettingValue(ci.Arg<string>()));

            settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            settingDefinitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(ci => new SettingDefinition(ci.Arg<string>(), GetSettingValue(ci.Arg<string>())));

            timeZoneService = Substitute.For<ITimeZoneService>();
            timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, Arg.Any<int?>()).Returns("UTC");

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            var sut = new HostSettingsAppService(
                Substitute.For<IEmailSender>(),
                timeZoneService,
                settingDefinitionManager,
                Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>(),
                Substitute.For<IEafMiddlewareLdapModuleConfig>()
            )
            {
                AbpSession = abpSession,
                SettingManager = settingManager
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

            var input = new HostSettingsEditDto
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

            await sut.UpdateAllSettings(input);

            true.ShouldBeTrue();
        }
    }
}
