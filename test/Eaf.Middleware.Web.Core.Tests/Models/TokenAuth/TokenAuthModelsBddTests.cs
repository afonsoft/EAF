using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    /// <summary>
    /// Testes BDD para modelos de TokenAuth seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TokenAuthModelsBddTests
    {
        #region AuthenticateModel

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin@acme.com",
                Password = "Senha@123",
                RememberClient = true,
                ReturnUrl = "/dashboard",
                SingleSignIn = true,
                TwoFactorVerificationCode = "123456",
                TwoFactorRememberClientToken = "token-abc",
                CaptchaResponse = "captcha-xyz"
            };

            model.UserNameOrEmailAddress.ShouldBe("admin@acme.com");
            model.Password.ShouldBe("Senha@123");
            model.RememberClient.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/dashboard");
            model.SingleSignIn.ShouldBe(true);
            model.TwoFactorVerificationCode.ShouldBe("123456");
            model.TwoFactorRememberClientToken.ShouldBe("token-abc");
            model.CaptchaResponse.ShouldBe("captcha-xyz");
        }

        #endregion

        #region AuthenticateResultModel

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new AuthenticateResultModel
            {
                AccessToken = "jwt-token",
                EncryptedAccessToken = "encrypted",
                ExpireInSeconds = 3600,
                UserId = 42,
                ShouldResetPassword = false,
                RequiresTwoFactorVerification = true,
                TwoFactorAuthProviders = new List<string> { "Email", "Google" },
                TwoFactorRememberClientToken = "remember-token",
                PasswordResetCode = "reset-code",
                ReturnUrl = "/home"
            };

            model.AccessToken.ShouldBe("jwt-token");
            model.EncryptedAccessToken.ShouldBe("encrypted");
            model.ExpireInSeconds.ShouldBe(3600);
            model.UserId.ShouldBe(42);
            model.ShouldResetPassword.ShouldBeFalse();
            model.RequiresTwoFactorVerification.ShouldBeTrue();
            model.TwoFactorAuthProviders.Count.ShouldBe(2);
            model.PasswordResetCode.ShouldBe("reset-code");
            model.ReturnUrl.ShouldBe("/home");
        }

        #endregion

        #region ExternalAuthenticateModel

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Google",
                ProviderKey = "google-key",
                ProviderAccessCode = "access-code",
                ReturnUrl = "/callback",
                SingleSignIn = false
            };

            model.AuthProvider.ShouldBe("Google");
            model.ProviderKey.ShouldBe("google-key");
            model.ProviderAccessCode.ShouldBe("access-code");
            model.ReturnUrl.ShouldBe("/callback");
            model.SingleSignIn.ShouldBe(false);
        }

        #endregion

        #region ExternalAuthenticateResultModel

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ExternalAuthenticateResultModel
            {
                AccessToken = "ext-token",
                EncryptedAccessToken = "enc-ext",
                ExpireInSeconds = 7200,
                ReturnUrl = "/ext-return",
                WaitingForActivation = true,
                UserId = 99
            };

            model.AccessToken.ShouldBe("ext-token");
            model.EncryptedAccessToken.ShouldBe("enc-ext");
            model.ExpireInSeconds.ShouldBe(7200);
            model.WaitingForActivation.ShouldBeTrue();
            model.UserId.ShouldBe(99);
        }

        #endregion

        #region ImpersonateModel

        [Fact]
        public void Dado_ImpersonateModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ImpersonateModel
            {
                TenantId = 5,
                UserId = 100
            };

            model.TenantId.ShouldBe(5);
            model.UserId.ShouldBe(100);
        }

        #endregion

        #region ImpersonateResultModel

        [Fact]
        public void Dado_ImpersonateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ImpersonateResultModel
            {
                ImpersonationToken = "imp-token-abc"
            };

            model.ImpersonationToken.ShouldBe("imp-token-abc");
        }

        #endregion

        #region ImpersonatedAuthenticateResultModel

        [Fact]
        public void Dado_ImpersonatedAuthResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ImpersonatedAuthenticateResultModel
            {
                AccessToken = "imp-access",
                EncryptedAccessToken = "imp-encrypted",
                ExpireInSeconds = 1800
            };

            model.AccessToken.ShouldBe("imp-access");
            model.EncryptedAccessToken.ShouldBe("imp-encrypted");
            model.ExpireInSeconds.ShouldBe(1800);
        }

        #endregion

        #region SwitchedAccountAuthenticateResultModel

        [Fact]
        public void Dado_SwitchedAccountAuthResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = "switch-token",
                EncryptedAccessToken = "switch-encrypted",
                ExpireInSeconds = 900
            };

            model.AccessToken.ShouldBe("switch-token");
            model.ExpireInSeconds.ShouldBe(900);
        }

        #endregion

        #region ProviderModel

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new ProviderModel
            {
                UsernameOrEmailAddress = "user@test.com",
                AuthenticationSource = "LDAP",
                Tenant = new TenantModal
                {
                    Id = 1,
                    Name = "Acme Corp",
                    TenancyName = "acme"
                }
            };

            model.UsernameOrEmailAddress.ShouldBe("user@test.com");
            model.AuthenticationSource.ShouldBe("LDAP");
            model.Tenant.ShouldNotBeNull();
            model.Tenant.Id.ShouldBe(1);
            model.Tenant.Name.ShouldBe("Acme Corp");
            model.Tenant.TenancyName.ShouldBe("acme");
        }

        #endregion

        #region SendTwoFactorAuthCodeModel

        [Fact]
        public void Dado_SendTwoFactorAuthCodeModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new SendTwoFactorAuthCodeModel
            {
                Provider = "Email",
                UserId = 55
            };

            model.Provider.ShouldBe("Email");
            model.UserId.ShouldBe(55);
        }

        #endregion
    }
}
