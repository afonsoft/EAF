using Abp.Runtime.Security;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.Web;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Accounts.Dto
{
    /// <summary>
    /// Testes BDD para ActivateEmailInput e ResetPasswordInput seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ActivateEmailAndResetPasswordBddTests
    {
        #region ActivateEmailInput

        [Fact]
        public void Dado_ActivateEmailInput_SemParametroC_Quando_Normalize_Entao_NaoDeveAlterarPropriedades()
        {
            var input = new ActivateEmailInput();
            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ConfirmationCode.ShouldBeNull();
        }

        [Fact]
        public void Dado_ActivateEmailInput_ComParametroC_Quando_Normalize_Entao_DeveResolverParametros()
        {
            // Dado
            var encrypted = SimpleStringCipher.Instance.Encrypt("userId=42&confirmationCode=ABC123");
            var input = new ActivateEmailInput { c = encrypted };

            // Quando
            input.Normalize();

            // Então
            input.UserId.ShouldBe(42);
            input.ConfirmationCode.ShouldBe("ABC123");
        }

        [Fact]
        public void Dado_ActivateEmailInput_ComParametroCSemUserId_Quando_Normalize_Entao_UserIdDeveSerZero()
        {
            var encrypted = SimpleStringCipher.Instance.Encrypt("confirmationCode=XYZ");
            var input = new ActivateEmailInput { c = encrypted };
            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ConfirmationCode.ShouldBe("XYZ");
        }

        [Fact]
        public void Dado_ActivateEmailInput_ComCVazio_Quando_Normalize_Entao_NaoDeveAlterarPropriedades()
        {
            var input = new ActivateEmailInput { c = "" };
            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ConfirmationCode.ShouldBeNull();
        }

        #endregion

        #region ResetPasswordInput

        [Fact]
        public void Dado_ResetPasswordInput_SemParametroC_Quando_Normalize_Entao_NaoDeveAlterarPropriedades()
        {
            var input = new ResetPasswordInput();
            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ResetCode.ShouldBeNull();
            input.AuthenticationSource.ShouldBeNull();
        }

        [Fact]
        public void Dado_ResetPasswordInput_ComParametroC_Quando_Normalize_Entao_DeveResolverParametros()
        {
            var encrypted = SimpleStringCipher.Instance.Encrypt("userId=99&resetCode=RST456&authenticationSource=LDAP");
            var input = new ResetPasswordInput { c = encrypted };

            input.Normalize();

            input.UserId.ShouldBe(99);
            input.ResetCode.ShouldBe("RST456");
            input.AuthenticationSource.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_ResetPasswordInput_ComPropriedadesAdicionais_Quando_Definir_Entao_DeveArmazenar()
        {
            var input = new ResetPasswordInput
            {
                Password = "NewP@ss123",
                ReturnUrl = "https://app.acme.com",
                SingleSignIn = "true"
            };

            input.Password.ShouldBe("NewP@ss123");
            input.ReturnUrl.ShouldBe("https://app.acme.com");
            input.SingleSignIn.ShouldBe("true");
        }

        [Fact]
        public void Dado_ResetPasswordInput_ComCSemResetCode_Quando_Normalize_Entao_ResetCodeDeveSerNull()
        {
            var encrypted = SimpleStringCipher.Instance.Encrypt("userId=10");
            var input = new ResetPasswordInput { c = encrypted };
            input.Normalize();

            input.UserId.ShouldBe(10);
            input.ResetCode.ShouldBeNull();
        }

        #endregion
    }
}
