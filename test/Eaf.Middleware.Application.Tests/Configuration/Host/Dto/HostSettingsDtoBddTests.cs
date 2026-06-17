using Eaf.Configuration.Host.Dto;
using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration.Host.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de configuração do Host seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class HostSettingsDtoBddTests
    {
        [Fact]
        public void Dado_AzureActiveDirectorySettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new AzureActiveDirectorySettingsEditDto
            {
                ClientId = "client-123",
                ClientSecret = "secret-456",
                IsEnabled = true,
                IsModuleEnabled = true,
                Tenant = "contoso.onmicrosoft.com"
            };

            dto.ClientId.ShouldBe("client-123");
            dto.ClientSecret.ShouldBe("secret-456");
            dto.IsEnabled.ShouldBeTrue();
            dto.IsModuleEnabled.ShouldBeTrue();
            dto.Tenant.ShouldBe("contoso.onmicrosoft.com");
        }

        [Fact]
        public void Dado_EmailSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new EmailSettingsEditDto
            {
                DefaultFromAddress = "noreply@acme.com",
                DefaultFromDisplayName = "Acme Corp",
                SmtpHost = "smtp.acme.com",
                SmtpPort = 587,
                SmtpUserName = "smtp-user",
                SmtpPassword = "smtp-pass",
                SmtpEnableSsl = true,
                SmtpUseDefaultCredentials = false
            };

            dto.DefaultFromAddress.ShouldBe("noreply@acme.com");
            dto.DefaultFromDisplayName.ShouldBe("Acme Corp");
            dto.SmtpHost.ShouldBe("smtp.acme.com");
            dto.SmtpPort.ShouldBe(587);
            dto.SmtpUserName.ShouldBe("smtp-user");
            dto.SmtpPassword.ShouldBe("smtp-pass");
            dto.SmtpEnableSsl.ShouldBeTrue();
            dto.SmtpUseDefaultCredentials.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ExpiredEntityLogDeleterSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ExpiredEntityLogDeleterSettingsEditDto
            {
                ExpiredDays = 90,
                Enabled = true,
                DeletedQuantity = 1000
            };

            dto.ExpiredDays.ShouldBe(90);
            dto.Enabled.ShouldBe(true);
            dto.DeletedQuantity.ShouldBe(1000);
        }

        [Fact]
        public void Dado_ExpiredEntityLoginImpersonatorSettingsEditDto_Quando_DefinirEnabled_Entao_DeveArmazenar()
        {
            var dto = new ExpiredEntityLoginImpersonatorSettingsEditDto { Enabled = true };
            dto.Enabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_GeneralSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GeneralSettingsEditDto { Timezone = "America/Sao_Paulo" };
            dto.Timezone.ShouldBe("America/Sao_Paulo");
        }

        [Fact]
        public void Dado_LdapSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new LdapSettingsEditDto
            {
                Domain = "ad.acme.com",
                IsModuleEnabled = true,
                UserName = "svc_ldap",
                Password = "ldap-pass"
            };

            dto.Domain.ShouldBe("ad.acme.com");
            dto.IsModuleEnabled.ShouldBeTrue();
            dto.UserName.ShouldBe("svc_ldap");
            dto.Password.ShouldBe("ldap-pass");
        }

        [Fact]
        public void Dado_SecuritySettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new SecuritySettingsEditDto
            {
                AllowOneConcurrentLoginPerUser = true,
                UseDefaultPasswordComplexitySettings = false
            };

            dto.AllowOneConcurrentLoginPerUser.ShouldBeTrue();
            dto.UseDefaultPasswordComplexitySettings.ShouldBeFalse();
        }

        [Fact]
        public void Dado_SendTestEmailInput_Quando_DefinirEmailAddress_Entao_DeveArmazenar()
        {
            var dto = new SendTestEmailInput { EmailAddress = "test@acme.com" };
            dto.EmailAddress.ShouldBe("test@acme.com");
        }

        [Fact]
        public void Dado_TwoFactorLoginSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TwoFactorLoginSettingsEditDto
            {
                IsEnabled = true,
                IsEmailProviderEnabled = true,
                IsSmsProviderEnabled = false,
                IsRememberBrowserEnabled = true
            };

            dto.IsEnabled.ShouldBeTrue();
            dto.IsEmailProviderEnabled.ShouldBeTrue();
            dto.IsSmsProviderEnabled.ShouldBeFalse();
            dto.IsRememberBrowserEnabled.ShouldBeTrue();
        }

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

        [Fact]
        public void Dado_GoogleSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new GoogleSettingsEditDto
            {
                Analytics = "UA-123456",
                RecaptchaSiteKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI",
                Tag = "GTM-ABC123"
            };

            dto.Analytics.ShouldBe("UA-123456");
            dto.RecaptchaSiteKey.ShouldBe("6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI");
            dto.Tag.ShouldBe("GTM-ABC123");
        }

        [Fact]
        public void Dado_HostSettingsEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new HostSettingsEditDto
            {
                General = new GeneralSettingsEditDto { Timezone = "UTC" },
                Email = new EmailSettingsEditDto { DefaultFromAddress = "noreply@acme.com" },
                Security = new SecuritySettingsEditDto { AllowOneConcurrentLoginPerUser = true },
                Google = new GoogleSettingsEditDto { Analytics = "UA-123" },
                Ldap = new LdapSettingsEditDto { Domain = "ad.acme.com" },
                AzureActiveDirectory = new AzureActiveDirectorySettingsEditDto { ClientId = "aad-123" },
                LogDeleter = new ExpiredEntityLogDeleterSettingsEditDto { Enabled = true },
                LoginImpersonator = new ExpiredEntityLoginImpersonatorSettingsEditDto { Enabled = true }
            };

            dto.General.Timezone.ShouldBe("UTC");
            dto.Email.DefaultFromAddress.ShouldBe("noreply@acme.com");
            dto.Security.AllowOneConcurrentLoginPerUser.ShouldBeTrue();
            dto.Google.Analytics.ShouldBe("UA-123");
            dto.Ldap.Domain.ShouldBe("ad.acme.com");
            dto.AzureActiveDirectory.ClientId.ShouldBe("aad-123");
            dto.LogDeleter.Enabled.ShouldBe(true);
            dto.LoginImpersonator.Enabled.ShouldBe(true);
        }
    }
}
