using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    public class TokenAuthModelTests
    {
        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin@test.com",
                Password = "123456",
                RememberClient = true,
                ReturnUrl = "/dashboard",
                SingleSignIn = true,
                CaptchaResponse = "captcha-token",
                TwoFactorRememberClientToken = "2fa-token",
                TwoFactorVerificationCode = "123456"
            };

            model.UserNameOrEmailAddress.ShouldBe("admin@test.com");
            model.Password.ShouldBe("123456");
            model.RememberClient.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/dashboard");
            model.SingleSignIn.ShouldBe(true);
            model.CaptchaResponse.ShouldBe("captcha-token");
            model.TwoFactorRememberClientToken.ShouldBe("2fa-token");
            model.TwoFactorVerificationCode.ShouldBe("123456");
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var providers = new List<string> { "Email", "SMS" };
            var model = new AuthenticateResultModel
            {
                AccessToken = "token-abc",
                EncryptedAccessToken = "encrypted-abc",
                ExpireInSeconds = 3600,
                PasswordResetCode = "reset-code",
                RequiresTwoFactorVerification = true,
                ReturnUrl = "/home",
                ShouldResetPassword = false,
                TwoFactorAuthProviders = providers,
                TwoFactorRememberClientToken = "remember-token",
                UserId = 42
            };

            model.AccessToken.ShouldBe("token-abc");
            model.EncryptedAccessToken.ShouldBe("encrypted-abc");
            model.ExpireInSeconds.ShouldBe(3600);
            model.PasswordResetCode.ShouldBe("reset-code");
            model.RequiresTwoFactorVerification.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/home");
            model.ShouldResetPassword.ShouldBeFalse();
            model.TwoFactorAuthProviders.Count.ShouldBe(2);
            model.TwoFactorRememberClientToken.ShouldBe("remember-token");
            model.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Google",
                ProviderAccessCode = "access-code-123",
                ProviderKey = "provider-key-abc",
                ReturnUrl = "/callback",
                SingleSignIn = false
            };

            model.AuthProvider.ShouldBe("Google");
            model.ProviderAccessCode.ShouldBe("access-code-123");
            model.ProviderKey.ShouldBe("provider-key-abc");
            model.ReturnUrl.ShouldBe("/callback");
            model.SingleSignIn.ShouldBe(false);
        }

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new ExternalAuthenticateResultModel
            {
                AccessToken = "ext-token",
                EncryptedAccessToken = "ext-encrypted",
                ExpireInSeconds = 7200,
                ReturnUrl = "/ext-callback",
                WaitingForActivation = true,
                UserId = 99
            };

            model.AccessToken.ShouldBe("ext-token");
            model.EncryptedAccessToken.ShouldBe("ext-encrypted");
            model.ExpireInSeconds.ShouldBe(7200);
            model.ReturnUrl.ShouldBe("/ext-callback");
            model.WaitingForActivation.ShouldBeTrue();
            model.UserId.ShouldBe(99);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new ImpersonateModel
            {
                TenantId = 5,
                UserId = 100
            };

            model.TenantId.ShouldBe(5);
            model.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_TenantIdNull_Entao_DevePermitir()
        {
            var model = new ImpersonateModel { UserId = 1 };
            model.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_ImpersonateResultModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new ImpersonateResultModel
            {
                ImpersonationToken = "imp-token-xyz"
            };

            model.ImpersonationToken.ShouldBe("imp-token-xyz");
        }

        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new ImpersonatedAuthenticateResultModel
            {
                AccessToken = "imp-access-token",
                EncryptedAccessToken = "imp-encrypted-token",
                ExpireInSeconds = 1800
            };

            model.AccessToken.ShouldBe("imp-access-token");
            model.EncryptedAccessToken.ShouldBe("imp-encrypted-token");
            model.ExpireInSeconds.ShouldBe(1800);
        }

        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = "switch-token",
                EncryptedAccessToken = "switch-encrypted",
                ExpireInSeconds = 900
            };

            model.AccessToken.ShouldBe("switch-token");
            model.EncryptedAccessToken.ShouldBe("switch-encrypted");
            model.ExpireInSeconds.ShouldBe(900);
        }

        [Fact]
        public void Dado_SendTwoFactorAuthCodeModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var model = new SendTwoFactorAuthCodeModel
            {
                Provider = "Email",
                UserId = 42
            };

            model.Provider.ShouldBe("Email");
            model.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ExternalLoginProviderInfoModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var additionalParams = new Dictionary<string, string>
            {
                { "scope", "openid profile" },
                { "response_type", "code" }
            };

            var model = new ExternalLoginProviderInfoModel
            {
                Name = "Google",
                ClientId = "client-id-123",
                TenantId = "tenant-1",
                AdditionalParams = additionalParams
            };

            model.Name.ShouldBe("Google");
            model.ClientId.ShouldBe("client-id-123");
            model.TenantId.ShouldBe("tenant-1");
            model.AdditionalParams.Count.ShouldBe(2);
            model.AdditionalParams["scope"].ShouldBe("openid profile");
        }

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var tenant = new TenantModal
            {
                Id = 1,
                Name = "TenantA",
                TenancyName = "tenantA"
            };

            var model = new ProviderModel
            {
                UsernameOrEmailAddress = "admin@test.com",
                AuthenticationSource = "LDAP",
                Tenant = tenant
            };

            model.UsernameOrEmailAddress.ShouldBe("admin@test.com");
            model.AuthenticationSource.ShouldBe("LDAP");
            model.Tenant.ShouldNotBeNull();
            model.Tenant.Id.ShouldBe(1);
            model.Tenant.Name.ShouldBe("TenantA");
            model.Tenant.TenancyName.ShouldBe("tenantA");
        }
    }
}
