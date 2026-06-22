using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    /// <summary>
    /// Testes BDD para AuthenticateModel seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AuthenticateModelBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirUserNameOrEmailAddress_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { UserNameOrEmailAddress = "admin@test.com" };
            model.UserNameOrEmailAddress.ShouldBe("admin@test.com");
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirPassword_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { Password = "P@ssw0rd!" };
            model.Password.ShouldBe("P@ssw0rd!");
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirRememberClient_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { RememberClient = true };
            model.RememberClient.ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirTwoFactorVerificationCode_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { TwoFactorVerificationCode = "123456" };
            model.TwoFactorVerificationCode.ShouldBe("123456");
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirReturnUrl_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { ReturnUrl = "/dashboard" };
            model.ReturnUrl.ShouldBe("/dashboard");
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirSingleSignIn_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { SingleSignIn = true };
            model.SingleSignIn.ShouldBe(true);
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_DefinirCaptchaResponse_Entao_DeveArmazenarCorretamente()
        {
            var model = new AuthenticateModel { CaptchaResponse = "captcha-token" };
            model.CaptchaResponse.ShouldBe("captcha-token");
        }

        #endregion

        #region Validacao

        [Fact]
        public void Dado_AuthenticateModel_Quando_UserNameVazio_Entao_DeveFalharValidacao()
        {
            var model = new AuthenticateModel { Password = "P@ssw0rd!" };
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeFalse();
            results.Any(r => r.MemberNames.Contains("UserNameOrEmailAddress")).ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_PasswordVazio_Entao_DeveFalharValidacao()
        {
            var model = new AuthenticateModel { UserNameOrEmailAddress = "admin" };
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeFalse();
            results.Any(r => r.MemberNames.Contains("Password")).ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthenticateModel_Quando_Preenchido_Entao_DevePassarValidacao()
        {
            var model = new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin@test.com",
                Password = "P@ssw0rd!"
            };
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeTrue();
        }

        #endregion
    }
}
