using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Models.TokenAuth
{
    /// <summary>
    /// Testes BDD para modelos de TokenAuth seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TokenAuthModelBddTests
    {
        #region AuthenticateResultModel

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new AuthenticateResultModel
            {
                AccessToken = "jwt-token",
                EncryptedAccessToken = "encrypted-jwt",
                ExpireInSeconds = 3600,
                PasswordResetCode = "RST123",
                RequiresTwoFactorVerification = true,
                ReturnUrl = "/home",
                ShouldResetPassword = false,
                TwoFactorAuthProviders = new List<string> { "Email", "Phone" },
                TwoFactorRememberClientToken = "remember-token",
                UserId = 42
            };

            // Então
            model.AccessToken.ShouldBe("jwt-token");
            model.EncryptedAccessToken.ShouldBe("encrypted-jwt");
            model.ExpireInSeconds.ShouldBe(3600);
            model.PasswordResetCode.ShouldBe("RST123");
            model.RequiresTwoFactorVerification.ShouldBeTrue();
            model.ReturnUrl.ShouldBe("/home");
            model.ShouldResetPassword.ShouldBeFalse();
            model.TwoFactorAuthProviders.Count.ShouldBe(2);
            model.TwoFactorRememberClientToken.ShouldBe("remember-token");
            model.UserId.ShouldBe(42);
        }

        #endregion

        #region ExternalAuthenticateModel

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Google",
                ProviderAccessCode = "access-code",
                ProviderKey = "google-key-123",
                ReturnUrl = "/dashboard",
                SingleSignIn = true
            };

            // Então
            model.AuthProvider.ShouldBe("Google");
            model.ProviderAccessCode.ShouldBe("access-code");
            model.ProviderKey.ShouldBe("google-key-123");
            model.ReturnUrl.ShouldBe("/dashboard");
            model.SingleSignIn.ShouldBe(true);
        }

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_SingleSignInNull_Entao_DeveAceitar()
        {
            // Dado & Quando
            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Facebook",
                ProviderAccessCode = "code",
                ProviderKey = "key"
            };

            // Então
            model.SingleSignIn.ShouldBeNull();
        }

        #endregion

        #region ExternalAuthenticateResultModel

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ExternalAuthenticateResultModel
            {
                AccessToken = "ext-jwt",
                EncryptedAccessToken = "enc-ext-jwt",
                ExpireInSeconds = 7200,
                ReturnUrl = "/profile",
                WaitingForActivation = true,
                UserId = 99
            };

            // Então
            model.AccessToken.ShouldBe("ext-jwt");
            model.EncryptedAccessToken.ShouldBe("enc-ext-jwt");
            model.ExpireInSeconds.ShouldBe(7200);
            model.ReturnUrl.ShouldBe("/profile");
            model.WaitingForActivation.ShouldBeTrue();
            model.UserId.ShouldBe(99);
        }

        #endregion

        #region ExternalLoginProviderInfoModel

        [Fact]
        public void Dado_ExternalLoginProviderInfoModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ExternalLoginProviderInfoModel
            {
                Name = "Google",
                ClientId = "google-client-id",
                TenantId = "1",
                AdditionalParams = new Dictionary<string, string>
                {
                    { "scope", "openid profile" }
                }
            };

            // Então
            model.Name.ShouldBe("Google");
            model.ClientId.ShouldBe("google-client-id");
            model.TenantId.ShouldBe("1");
            model.AdditionalParams.ShouldContainKey("scope");
        }

        #endregion

        #region ImpersonateModel

        [Fact]
        public void Dado_ImpersonateModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ImpersonateModel
            {
                TenantId = 1,
                UserId = 100
            };

            // Então
            model.TenantId.ShouldBe(1);
            model.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_TenantIdNull_Entao_DeveAceitar()
        {
            // Dado & Quando
            var model = new ImpersonateModel { UserId = 50 };

            // Então
            model.TenantId.ShouldBeNull();
        }

        #endregion

        #region ImpersonateResultModel

        [Fact]
        public void Dado_ImpersonateResultModel_Quando_DefinirToken_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ImpersonateResultModel
            {
                ImpersonationToken = "imp-token-xyz"
            };

            // Então
            model.ImpersonationToken.ShouldBe("imp-token-xyz");
        }

        #endregion

        #region ImpersonatedAuthenticateResultModel

        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ImpersonatedAuthenticateResultModel
            {
                AccessToken = "imp-access",
                EncryptedAccessToken = "imp-enc-access",
                ExpireInSeconds = 1800
            };

            // Então
            model.AccessToken.ShouldBe("imp-access");
            model.EncryptedAccessToken.ShouldBe("imp-enc-access");
            model.ExpireInSeconds.ShouldBe(1800);
        }

        #endregion

        #region ProviderModel

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new ProviderModel
            {
                UsernameOrEmailAddress = "admin@acme.com",
                AuthenticationSource = "LDAP",
                Tenant = new TenantModal
                {
                    Id = 1,
                    Name = "Acme",
                    TenancyName = "acme"
                }
            };

            // Então
            model.UsernameOrEmailAddress.ShouldBe("admin@acme.com");
            model.AuthenticationSource.ShouldBe("LDAP");
            model.Tenant.ShouldNotBeNull();
            model.Tenant.Id.ShouldBe(1);
            model.Tenant.Name.ShouldBe("Acme");
            model.Tenant.TenancyName.ShouldBe("acme");
        }

        #endregion

        #region SendTwoFactorAuthCodeModel

        [Fact]
        public void Dado_SendTwoFactorAuthCodeModel_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new SendTwoFactorAuthCodeModel
            {
                UserId = 42,
                Provider = "Phone"
            };

            // Então
            model.UserId.ShouldBe(42);
            model.Provider.ShouldBe("Phone");
        }

        #endregion

        #region SwitchedAccountAuthenticateResultModel

        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var model = new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = "switch-jwt",
                EncryptedAccessToken = "switch-enc-jwt",
                ExpireInSeconds = 3600
            };

            // Então
            model.AccessToken.ShouldBe("switch-jwt");
            model.EncryptedAccessToken.ShouldBe("switch-enc-jwt");
            model.ExpireInSeconds.ShouldBe(3600);
        }

        #endregion
    }
}
