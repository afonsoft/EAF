using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration.Host.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Host Configuration seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class HostConfigurationDtoBddTests
    {
        #region HostSettingsEditDto

        [Fact]
        public void Dado_HostSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new HostSettingsEditDto
            {
                General = new GeneralSettingsEditDto(),
                Email = new EmailSettingsEditDto(),
                Security = new SecuritySettingsEditDto(),
                UserManagement = new HostUserManagementSettingsEditDto(),
                AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto(),
                Google = new GoogleSettingsEditDto(),
                Ldap = new LdapSettingsEditDto(),
                ExternalLoginProviderSettings = new ExternalLoginProviderSettingsEditDto(),
                LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto(),
                LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto()
            };

            dto.General.ShouldNotBeNull();
            dto.Email.ShouldNotBeNull();
            dto.Security.ShouldNotBeNull();
            dto.UserManagement.ShouldNotBeNull();
            dto.AzureActiveDirectory.ShouldNotBeNull();
            dto.Google.ShouldNotBeNull();
            dto.Ldap.ShouldNotBeNull();
            dto.ExternalLoginProviderSettings.ShouldNotBeNull();
            dto.LogDeleter.ShouldNotBeNull();
            dto.LoginImpersonator.ShouldNotBeNull();
        }

        #endregion

        #region EmailSettingsEditDto

        [Fact]
        public void Dado_EmailSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new EmailSettingsEditDto
            {
                DefaultFromAddress = "noreply@acme.com",
                DefaultFromDisplayName = "EAF System",
                SmtpHost = "smtp.acme.com",
                SmtpPort = 587,
                SmtpUserName = "smtp-user",
                SmtpPassword = "smtp-pass",
                SmtpDomain = "acme.com",
                SmtpEnableSsl = true,
                SmtpUseDefaultCredentials = false
            };

            dto.DefaultFromAddress.ShouldBe("noreply@acme.com");
            dto.DefaultFromDisplayName.ShouldBe("EAF System");
            dto.SmtpHost.ShouldBe("smtp.acme.com");
            dto.SmtpPort.ShouldBe(587);
            dto.SmtpUserName.ShouldBe("smtp-user");
            dto.SmtpPassword.ShouldBe("smtp-pass");
            dto.SmtpEnableSsl.ShouldBeTrue();
            dto.SmtpUseDefaultCredentials.ShouldBeFalse();
        }

        #endregion

        #region GeneralSettingsEditDto

        [Fact]
        public void Dado_GeneralSettingsEditDto_Quando_DefinirTimezone_Entao_DeveArmazenar()
        {
            var dto = new GeneralSettingsEditDto
            {
                Timezone = "E. South America Standard Time",
                TimezoneForComparison = "UTC"
            };

            dto.Timezone.ShouldBe("E. South America Standard Time");
            dto.TimezoneForComparison.ShouldBe("UTC");
        }

        #endregion

        #region SecuritySettingsEditDto

        [Fact]
        public void Dado_SecuritySettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings = true,
                PasswordComplexity = new Eaf.Middleware.Security.PasswordComplexitySetting
                {
                    RequireDigit = true,
                    RequireUppercase = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequiredLength = 8
                },
                DefaultPasswordComplexity = new Eaf.Middleware.Security.PasswordComplexitySetting(),
                UserLockOut = new UserLockOutSettingsEditDto(),
                TwoFactorLogin = new TwoFactorLoginSettingsEditDto()
            };

            dto.UseDefaultPasswordComplexitySettings.ShouldBeTrue();
            dto.PasswordComplexity.RequireDigit.ShouldBeTrue();
            dto.PasswordComplexity.RequiredLength.ShouldBe(8);
        }

        #endregion

        #region UserLockOutSettingsEditDto

        [Fact]
        public void Dado_UserLockOutSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UserLockOutSettingsEditDto
            {
                IsEnabled = true,
                MaxFailedAccessAttemptsBeforeLockout = 5,
                DefaultAccountLockoutSeconds = 300
            };

            dto.IsEnabled.ShouldBeTrue();
            dto.MaxFailedAccessAttemptsBeforeLockout.ShouldBe(5);
            dto.DefaultAccountLockoutSeconds.ShouldBe(300);
        }

        #endregion

        #region TwoFactorLoginSettingsEditDto

        [Fact]
        public void Dado_TwoFactorLoginSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TwoFactorLoginSettingsEditDto
            {
                IsEnabled = true,
                IsEmailProviderEnabled = true,
                IsSmsProviderEnabled = false,
                IsRememberBrowserEnabled = true,
                IsGoogleAuthenticatorEnabled = false
            };

            dto.IsEnabled.ShouldBeTrue();
            dto.IsEmailProviderEnabled.ShouldBeTrue();
            dto.IsSmsProviderEnabled.ShouldBeFalse();
            dto.IsRememberBrowserEnabled.ShouldBeTrue();
            dto.IsGoogleAuthenticatorEnabled.ShouldBeFalse();
        }

        #endregion

        #region HostUserManagementSettingsEditDto

        [Fact]
        public void Dado_HostUserManagementSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new HostUserManagementSettingsEditDto
            {
                IsEmailConfirmationRequiredForLogin = true,
                AllowOneConcurrentLoginPerUser = false,
                IsCookieConsentEnabled = true,
                IsRegisterRequiredForLogin = false,
                StoreExternalTokenInformation = true,
                TokenExpiration = 3600,
                UseCaptchaOnLogin = false
            };

            dto.IsEmailConfirmationRequiredForLogin.ShouldBeTrue();
            dto.AllowOneConcurrentLoginPerUser.ShouldBeFalse();
            dto.IsCookieConsentEnabled.ShouldBeTrue();
            dto.TokenExpiration.ShouldBe(3600);
        }

        #endregion

        #region AzureActiveDirectorySettingsEditDto

        [Fact]
        public void Dado_AzureActiveDirectorySettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new AzureActiveDirectorySettingsEditDto
            {
                IsEnabled = true,
                ClientId = "aad-client-id",
                Tenant = "acme.onmicrosoft.com"
            };

            dto.IsEnabled.ShouldBeTrue();
            dto.ClientId.ShouldBe("aad-client-id");
            dto.Tenant.ShouldBe("acme.onmicrosoft.com");
        }

        #endregion

        #region GoogleSettingsEditDto

        [Fact]
        public void Dado_GoogleSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GoogleSettingsEditDto
            {
                Analytics = "UA-12345",
                RecaptchaSiteKey = "recaptcha-key-123",
                Tag = "GTM-XYZ"
            };

            dto.Analytics.ShouldBe("UA-12345");
            dto.RecaptchaSiteKey.ShouldBe("recaptcha-key-123");
            dto.Tag.ShouldBe("GTM-XYZ");
        }

        #endregion

        #region LdapSettingsEditDto

        [Fact]
        public void Dado_LdapSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new LdapSettingsEditDto
            {
                IsEnabled = true,
                Domain = "acme.local",
                UserName = "ldap-user",
                Password = "ldap-pass"
            };

            dto.IsEnabled.ShouldBeTrue();
            dto.Domain.ShouldBe("acme.local");
            dto.UserName.ShouldBe("ldap-user");
            dto.Password.ShouldBe("ldap-pass");
        }

        #endregion

        #region ExternalLoginProviderSettingsEditDto

        [Fact]
        public void Dado_ExternalLoginProviderSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ExternalLoginProviderSettingsEditDto
            {
                Google_IsEnabled = true,
                Google = new Eaf.Middleware.Core.Authentication.GoogleExternalLoginProviderSettings
                {
                    ClientId = "g-id",
                    ClientSecret = "g-secret"
                },
                Microsoft_IsEnabled = false,
                Microsoft = new Eaf.Middleware.Core.Authentication.MicrosoftExternalLoginProviderSettings(),
                OpenIdConnect_IsEnabled = true,
                OpenIdConnect = new Eaf.Middleware.Core.Authentication.OpenIdConnectExternalLoginProviderSettings(),
                AuthZero_IsEnabled = false,
                AuthZero = new Eaf.Middleware.Core.Authentication.AuthZeroExternalLoginProviderSettings()
            };

            dto.Google_IsEnabled.ShouldBeTrue();
            dto.Google.ShouldNotBeNull();
            dto.Microsoft_IsEnabled.ShouldBeFalse();
            dto.Microsoft.ShouldNotBeNull();
            dto.OpenIdConnect_IsEnabled.ShouldBeTrue();
            dto.AuthZero_IsEnabled.ShouldBeFalse();
        }

        #endregion

        #region SendTestEmailInput

        [Fact]
        public void Dado_SendTestEmailInput_Quando_DefinirEmail_Entao_DeveArmazenar()
        {
            var input = new SendTestEmailInput
            {
                EmailAddress = "test@acme.com"
            };

            input.EmailAddress.ShouldBe("test@acme.com");
        }

        #endregion

        #region ExpiredEntityLogDeleterSettingsEditDto

        [Fact]
        public void Dado_ExpiredEntityLogDeleterSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ExpiredEntityLogDeleterSettingsEditDto
            {
                Enabled = true,
                ExpiredDays = 90,
                DeletedQuantity = 1000
            };

            dto.Enabled.ShouldBe(true);
            dto.ExpiredDays.ShouldBe(90);
            dto.DeletedQuantity.ShouldBe(1000);
        }

        #endregion
    }
}
