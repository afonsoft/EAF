using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Accounts.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Conta seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AccountDtoTests
    {
        #region IsTenantAvailableInput

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new IsTenantAvailableInput { TenancyName = "acme" };

            // Então
            input.TenancyName.ShouldBe("acme");
        }

        #endregion

        #region TenantAvailabilityState

        [Fact]
        public void Dado_TenantAvailabilityState_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            TenantAvailabilityState.Available.ShouldBe((TenantAvailabilityState)1);
            TenantAvailabilityState.InActive.ShouldBe((TenantAvailabilityState)2);
            TenantAvailabilityState.NotFound.ShouldBe((TenantAvailabilityState)3);
        }

        #endregion

        #region ResetPasswordInput

        [Fact]
        public void Dado_ResetPasswordInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new ResetPasswordInput
            {
                AuthenticationSource = "LDAP",
                Password = "NewP@ss123",
                ResetCode = "ABC123",
                ReturnUrl = "/dashboard",
                SingleSignIn = "true",
                UserId = 42
            };

            // Então
            input.AuthenticationSource.ShouldBe("LDAP");
            input.Password.ShouldBe("NewP@ss123");
            input.ResetCode.ShouldBe("ABC123");
            input.ReturnUrl.ShouldBe("/dashboard");
            input.SingleSignIn.ShouldBe("true");
            input.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ResetPasswordInput_Quando_NormalizeSemParametroC_Entao_NaoDeveAlterar()
        {
            // Dado
            var input = new ResetPasswordInput
            {
                UserId = 10,
                ResetCode = "XYZ"
            };

            // Quando
            input.Normalize();

            // Então
            input.UserId.ShouldBe(10);
            input.ResetCode.ShouldBe("XYZ");
        }

        #endregion

        #region ResetPasswordOutput

        [Fact]
        public void Dado_ResetPasswordOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var output = new ResetPasswordOutput
            {
                CanLogin = true,
                UserName = "admin"
            };

            // Então
            output.CanLogin.ShouldBeTrue();
            output.UserName.ShouldBe("admin");
        }

        #endregion

        #region SendPasswordResetCodeInput

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_DefinirEmail_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new SendPasswordResetCodeInput { EmailAddress = "user@acme.com" };

            // Então
            input.EmailAddress.ShouldBe("user@acme.com");
        }

        #endregion

        #region ActivateEmailInput

        [Fact]
        public void Dado_ActivateEmailInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new ActivateEmailInput
            {
                UserId = 5,
                ConfirmationCode = "confirm-abc"
            };

            // Então
            input.UserId.ShouldBe(5);
            input.ConfirmationCode.ShouldBe("confirm-abc");
        }

        #endregion
    }
}
