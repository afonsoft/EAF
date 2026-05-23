using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Configuration.Host.Dto;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Security;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration.Host.Dto
{
    public class HostConfigDtoCoverageTests
    {
        [Fact]
        public void AzureActiveDirectorySettingsEditDto_ShouldSet()
        {
            var dto = new AzureActiveDirectorySettingsEditDto
            {
                ClientId = "c",
                ClientSecret = "s",
                IsEnabled = true,
                IsModuleEnabled = true,
                Tenant = "t"
            };
            dto.ClientId.ShouldBe("c");
            dto.ClientSecret.ShouldBe("s");
            dto.IsEnabled.ShouldBeTrue();
            dto.IsModuleEnabled.ShouldBeTrue();
            dto.Tenant.ShouldBe("t");
        }

        [Fact]
        public void EmailSettingsEditDto_ShouldSetAll()
        {
            var dto = new EmailSettingsEditDto
            {
                DefaultFromAddress = "a@b.com",
                DefaultFromDisplayName = "dn",
                SmtpDomain = "sd",
                SmtpEnableSsl = true,
                SmtpHost = "h",
                SmtpPassword = "p",
                SmtpPort = 25,
                SmtpUseDefaultCredentials = true,
                SmtpUserName = "u"
            };
            dto.DefaultFromAddress.ShouldBe("a@b.com");
            dto.DefaultFromDisplayName.ShouldBe("dn");
            dto.SmtpDomain.ShouldBe("sd");
            dto.SmtpEnableSsl.ShouldBeTrue();
            dto.SmtpHost.ShouldBe("h");
            dto.SmtpPassword.ShouldBe("p");
            dto.SmtpPort.ShouldBe(25);
            dto.SmtpUseDefaultCredentials.ShouldBeTrue();
            dto.SmtpUserName.ShouldBe("u");
        }

        [Fact]
        public void ExpiredEntityLogDeleterSettingsEditDto_ShouldSet()
        {
            var dto = new ExpiredEntityLogDeleterSettingsEditDto
            {
                ExpiredDays = 30,
                Enabled = true,
                DeletedQuantity = 100
            };
            dto.ExpiredDays.ShouldBe(30);
            dto.Enabled.ShouldBe(true);
            dto.DeletedQuantity.ShouldBe(100);
        }

        [Fact]
        public void ExpiredEntityLoginImpersonatorSettingsEditDto_ShouldSet()
        {
            var dto = new ExpiredEntityLoginImpersonatorSettingsEditDto { Enabled = true };
            dto.Enabled.ShouldBe(true);
        }

        [Fact]
        public void ExternalLoginProviderSettingsEditDto_ShouldSet()
        {
            var dto = new ExternalLoginProviderSettingsEditDto
            {
                Google = new GoogleExternalLoginProviderSettings(),
                Google_IsEnabled = true,
                Microsoft = new MicrosoftExternalLoginProviderSettings(),
                Microsoft_IsEnabled = true,
                OpenIdConnect = new OpenIdConnectExternalLoginProviderSettings(),
                OpenIdConnect_IsEnabled = false,
                OpenIdConnectClaimsMapping = new List<JsonClaimMapDto>(),
                AuthZero = new AuthZeroExternalLoginProviderSettings(),
                AuthZero_IsEnabled = true
            };
            dto.Google.ShouldNotBeNull();
            dto.Google_IsEnabled.ShouldBeTrue();
            dto.Microsoft.ShouldNotBeNull();
            dto.Microsoft_IsEnabled.ShouldBeTrue();
            dto.OpenIdConnect.ShouldNotBeNull();
            dto.OpenIdConnect_IsEnabled.ShouldBeFalse();
            dto.OpenIdConnectClaimsMapping.ShouldNotBeNull();
            dto.AuthZero.ShouldNotBeNull();
            dto.AuthZero_IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void GeneralSettingsEditDto_ShouldSet()
        {
            var dto = new GeneralSettingsEditDto { Timezone = "UTC", TimezoneForComparison = "UTC" };
            dto.Timezone.ShouldBe("UTC");
            dto.TimezoneForComparison.ShouldBe("UTC");
        }

        [Fact]
        public void GoogleSettingsEditDto_ShouldSet()
        {
            var dto = new GoogleSettingsEditDto { Analytics = "a", RecaptchaSiteKey = "r", Tag = "t" };
            dto.Analytics.ShouldBe("a");
            dto.RecaptchaSiteKey.ShouldBe("r");
            dto.Tag.ShouldBe("t");
        }

        [Fact]
        public void HostSettingsEditDto_ShouldSetAll()
        {
            var dto = new HostSettingsEditDto
            {
                AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto(),
                Email = new EmailSettingsEditDto(),
                ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto(),
                General = new GeneralSettingsEditDto(),
                Google = new GoogleSettingsEditDto(),
                Ldap = new LdapSettingsEditDto(),
                Security = new SecuritySettingsEditDto(),
                UserManagement = new HostUserManagementSettingsEditDto(),
                LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto(),
                LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto()
            };
            dto.AzureActiveDirectory.ShouldNotBeNull();
            dto.Email.ShouldNotBeNull();
            dto.ExternalLoginProviderSettings.ShouldNotBeNull();
            dto.General.ShouldNotBeNull();
            dto.Google.ShouldNotBeNull();
            dto.Ldap.ShouldNotBeNull();
            dto.Security.ShouldNotBeNull();
            dto.UserManagement.ShouldNotBeNull();
            dto.LogDeleter.ShouldNotBeNull();
            dto.LoginImpersonator.ShouldNotBeNull();
        }

        [Fact]
        public void HostUserManagementSettingsEditDto_ShouldSetAll()
        {
            var dto = new HostUserManagementSettingsEditDto
            {
                AllowOneConcurrentLoginPerUser = true,
                IsCookieConsentEnabled = true,
                IsEmailConfirmationRequiredForLogin = true,
                IsRegisterRequiredForLogin = false,
                StoreExternalTokenInformation = true,
                TokenExpiration = 30,
                UseCaptchaOnLogin = true
            };
            dto.AllowOneConcurrentLoginPerUser.ShouldBeTrue();
            dto.IsCookieConsentEnabled.ShouldBeTrue();
            dto.IsEmailConfirmationRequiredForLogin.ShouldBeTrue();
            dto.IsRegisterRequiredForLogin.ShouldBeFalse();
            dto.StoreExternalTokenInformation.ShouldBeTrue();
            dto.TokenExpiration.ShouldBe(30);
            dto.UseCaptchaOnLogin.ShouldBeTrue();
        }

        [Fact]
        public void LdapSettingsEditDto_ShouldSet()
        {
            var dto = new LdapSettingsEditDto
            {
                Domain = "d",
                IsEnabled = true,
                IsModuleEnabled = false,
                Password = "p",
                UserName = "u"
            };
            dto.Domain.ShouldBe("d");
            dto.IsEnabled.ShouldBeTrue();
            dto.IsModuleEnabled.ShouldBeFalse();
            dto.Password.ShouldBe("p");
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void SecuritySettingsEditDto_ShouldSet()
        {
            var dto = new SecuritySettingsEditDto
            {
                AllowOneConcurrentLoginPerUser = true,
                DefaultPasswordComplexity = new PasswordComplexitySetting(),
                PasswordComplexity = new PasswordComplexitySetting(),
                TwoFactorLogin = new TwoFactorLoginSettingsEditDto(),
                UseDefaultPasswordComplexitySettings = true,
                UserLockOut = new UserLockOutSettingsEditDto()
            };
            dto.AllowOneConcurrentLoginPerUser.ShouldBeTrue();
            dto.DefaultPasswordComplexity.ShouldNotBeNull();
            dto.PasswordComplexity.ShouldNotBeNull();
            dto.TwoFactorLogin.ShouldNotBeNull();
            dto.UseDefaultPasswordComplexitySettings.ShouldBeTrue();
            dto.UserLockOut.ShouldNotBeNull();
        }

        [Fact]
        public void SendTestEmailInput_ShouldSet()
        {
            var dto = new SendTestEmailInput { EmailAddress = "a@b.com" };
            dto.EmailAddress.ShouldBe("a@b.com");
        }

        [Fact]
        public void TwoFactorLoginSettingsEditDto_ShouldSetAll()
        {
            var dto = new TwoFactorLoginSettingsEditDto
            {
                IsEmailProviderEnabled = true,
                IsEnabled = true,
                IsEnabledForApplication = true,
                IsGoogleAuthenticatorEnabled = true,
                IsRememberBrowserEnabled = true,
                IsSmsProviderEnabled = true
            };
            dto.IsEmailProviderEnabled.ShouldBeTrue();
            dto.IsEnabled.ShouldBeTrue();
            dto.IsEnabledForApplication.ShouldBeTrue();
            dto.IsGoogleAuthenticatorEnabled.ShouldBeTrue();
            dto.IsRememberBrowserEnabled.ShouldBeTrue();
            dto.IsSmsProviderEnabled.ShouldBeTrue();
        }

        [Fact]
        public void UserLockOutSettingsEditDto_ShouldSetAll()
        {
            var dto = new UserLockOutSettingsEditDto
            {
                DefaultAccountLockoutSeconds = 300,
                IsEnabled = true,
                MaxFailedAccessAttemptsBeforeLockout = 5
            };
            dto.DefaultAccountLockoutSeconds.ShouldBe(300);
            dto.IsEnabled.ShouldBeTrue();
            dto.MaxFailedAccessAttemptsBeforeLockout.ShouldBe(5);
        }
    }
}
