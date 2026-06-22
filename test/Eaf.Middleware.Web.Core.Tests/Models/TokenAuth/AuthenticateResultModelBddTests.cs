using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    /// <summary>
    /// Testes BDD para AuthenticateResultModel seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AuthenticateResultModelBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirAccessToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { AccessToken = "jwt-token-123" };
            model.AccessToken.ShouldBe("jwt-token-123");
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirEncryptedAccessToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { EncryptedAccessToken = "encrypted-token" };
            model.EncryptedAccessToken.ShouldBe("encrypted-token");
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirExpireInSeconds_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { ExpireInSeconds = 3600 };
            model.ExpireInSeconds.ShouldBe(3600);
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirRequiresTwoFactorVerification_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { RequiresTwoFactorVerification = true };
            model.RequiresTwoFactorVerification.ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirTwoFactorAuthProviders_Entao_DeveArmazenarCorretamente()
        {
            var providers = new List<string> { "Email", "Authenticator" };
            var model = new AuthenticateResultModel { TwoFactorAuthProviders = providers };
            model.TwoFactorAuthProviders.ShouldBe(providers);
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirShouldResetPassword_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { ShouldResetPassword = true };
            model.ShouldResetPassword.ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirPasswordResetCode_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { PasswordResetCode = "reset-code-abc" };
            model.PasswordResetCode.ShouldBe("reset-code-abc");
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_DefinirReturnUrl_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateResultModel { ReturnUrl = "/dashboard" };
            model.ReturnUrl.ShouldBe("/dashboard");
        }

        [Fact]
        public void Dado_AuthenticateResultModel_Quando_CriarInstancia_Entao_ValoresPadraoCorretos()
        {
            var model = new AuthenticateResultModel();
            model.AccessToken.ShouldBeNull();
            model.ExpireInSeconds.ShouldBe(0);
            model.RequiresTwoFactorVerification.ShouldBeFalse();
            model.ShouldResetPassword.ShouldBeFalse();
        }

        #endregion
    }
}
