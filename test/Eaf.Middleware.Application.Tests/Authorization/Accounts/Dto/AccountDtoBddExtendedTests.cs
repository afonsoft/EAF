using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    /// <summary>
    /// Testes BDD estendidos para DTOs de Accounts seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AccountDtoBddExtendedTests
    {
        #region IsTenantAvailableOutput

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarComConstrutorPadrao_Entao_DeveInicializar()
        {
            var output = new IsTenantAvailableOutput();
            output.TenantId.ShouldBeNull();
            output.ServerRootAddress.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarComStateETenantId_Entao_DeveDefinir()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.Available, 42);
            output.State.ShouldBe(TenantAvailabilityState.Available);
            output.TenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarComServerRootAddress_Entao_DeveDefinir()
        {
            var output = new IsTenantAvailableOutput(
                TenantAvailabilityState.Available, 1, "https://api.acme.com");
            output.State.ShouldBe(TenantAvailabilityState.Available);
            output.TenantId.ShouldBe(1);
            output.ServerRootAddress.ShouldBe("https://api.acme.com");
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_StateInActive_Entao_DeveSerCorreto()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            output.State.ShouldBe(TenantAvailabilityState.InActive);
            output.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_StateNotFound_Entao_DeveSerCorreto()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            output.State.ShouldBe(TenantAvailabilityState.NotFound);
        }

        #endregion

        #region TenantAvailabilityState

        [Fact]
        public void Dado_TenantAvailabilityState_Quando_VerificarValores_Entao_DevemEstarCorretos()
        {
            ((int)TenantAvailabilityState.Available).ShouldBe(1);
            ((int)TenantAvailabilityState.InActive).ShouldBe(2);
            ((int)TenantAvailabilityState.NotFound).ShouldBe(3);
        }

        #endregion

        #region ImpersonateInput

        [Fact]
        public void Dado_ImpersonateInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new ImpersonateInput
            {
                TenantId = 5,
                UserId = 100
            };

            input.TenantId.ShouldBe(5);
            input.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_ImpersonateInput_SemTenantId_Quando_Verificar_Entao_DeveSerNull()
        {
            var input = new ImpersonateInput { UserId = 1 };
            input.TenantId.ShouldBeNull();
        }

        #endregion

        #region ImpersonateOutput

        [Fact]
        public void Dado_ImpersonateOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new ImpersonateOutput
            {
                ImpersonationToken = "token-xyz",
                TenancyName = "acme"
            };

            output.ImpersonationToken.ShouldBe("token-xyz");
            output.TenancyName.ShouldBe("acme");
        }

        #endregion

        #region IsTenantAvailableInput

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var input = new IsTenantAvailableInput
            {
                TenancyName = "acme-corp"
            };

            input.TenancyName.ShouldBe("acme-corp");
        }

        #endregion

        #region RegisterOutput

        [Fact]
        public void Dado_RegisterOutput_Quando_DefinirCanLogin_Entao_DeveArmazenar()
        {
            var output = new RegisterOutput { CanLogin = true };
            output.CanLogin.ShouldBeTrue();
        }

        [Fact]
        public void Dado_RegisterOutput_Quando_Padrao_Entao_CanLoginDeveFalse()
        {
            var output = new RegisterOutput();
            output.CanLogin.ShouldBeFalse();
        }

        #endregion

        #region ResetPasswordOutput

        [Fact]
        public void Dado_ResetPasswordOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new ResetPasswordOutput
            {
                CanLogin = true,
                UserName = "admin"
            };

            output.CanLogin.ShouldBeTrue();
            output.UserName.ShouldBe("admin");
        }

        #endregion

        #region CurrentTenantInfoDto

        [Fact]
        public void Dado_CurrentTenantInfoDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new CurrentTenantInfoDto
            {
                Id = 10,
                Name = "Acme Corporation",
                TenancyName = "acme"
            };

            dto.Id.ShouldBe(10);
            dto.Name.ShouldBe("Acme Corporation");
            dto.TenancyName.ShouldBe("acme");
        }

        #endregion

        #region SendPasswordResetCodeInput

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_DefinirEmail_Entao_DeveArmazenar()
        {
            var input = new SendPasswordResetCodeInput
            {
                EmailAddress = "user@acme.com"
            };

            input.EmailAddress.ShouldBe("user@acme.com");
        }

        #endregion

        #region SendEmailActivationLinkInput

        [Fact]
        public void Dado_SendEmailActivationLinkInput_Quando_DefinirEmail_Entao_DeveArmazenar()
        {
            var input = new SendEmailActivationLinkInput
            {
                EmailAddress = "newuser@acme.com"
            };

            input.EmailAddress.ShouldBe("newuser@acme.com");
        }

        #endregion

        #region ResetPasswordInput

        [Fact]
        public void Dado_ResetPasswordInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new ResetPasswordInput
            {
                UserId = 42,
                ResetCode = "reset-code-123",
                Password = "NewPass@123",
                ReturnUrl = "/login",
                SingleSignIn = "true",
                AuthenticationSource = "LDAP"
            };

            input.UserId.ShouldBe(42);
            input.ResetCode.ShouldBe("reset-code-123");
            input.Password.ShouldBe("NewPass@123");
            input.ReturnUrl.ShouldBe("/login");
            input.SingleSignIn.ShouldBe("true");
            input.AuthenticationSource.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_ResetPasswordInput_SemC_Quando_Normalize_Entao_NaoDeveFalhar()
        {
            var input = new ResetPasswordInput
            {
                UserId = 1,
                ResetCode = "abc"
            };

            input.Normalize();
            input.UserId.ShouldBe(1);
            input.ResetCode.ShouldBe("abc");
        }

        #endregion
    }
}
