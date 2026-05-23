using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class AccountDtoTests
    {
        [Fact]
        public void Dado_TenantAvailabilityState_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)TenantAvailabilityState.Available).ShouldBe(1);
            ((int)TenantAvailabilityState.InActive).ShouldBe(2);
            ((int)TenantAvailabilityState.NotFound).ShouldBe(3);
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarComEstado_Entao_DeveDefinirPropriedades()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.Available, 1);

            output.State.ShouldBe(TenantAvailabilityState.Available);
            output.TenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarComServerRoot_Entao_DeveDefinirPropriedades()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.Available, 1, "https://api.example.com");

            output.State.ShouldBe(TenantAvailabilityState.Available);
            output.TenantId.ShouldBe(1);
            output.ServerRootAddress.ShouldBe("https://api.example.com");
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarSemTenantId_Entao_DeveSerNull()
        {
            var output = new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            output.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_IsTenantAvailableOutput_Quando_CriarConstrutorPadrao_Entao_DeveFuncionar()
        {
            var output = new IsTenantAvailableOutput();
            output.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_RegisterOutput_Quando_CanLoginTrue_Entao_DeveRetornarTrue()
        {
            var output = new RegisterOutput { CanLogin = true };
            output.CanLogin.ShouldBeTrue();
        }

        [Fact]
        public void Dado_RegisterOutput_Quando_CanLoginFalse_Entao_DeveRetornarFalse()
        {
            var output = new RegisterOutput { CanLogin = false };
            output.CanLogin.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ImpersonateInput_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var input = new ImpersonateInput
            {
                TenantId = 5,
                UserId = 42
            };

            input.TenantId.ShouldBe(5);
            input.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ImpersonateOutput_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var output = new ImpersonateOutput
            {
                ImpersonationToken = "token-123",
                TenancyName = "TenantA"
            };

            output.ImpersonationToken.ShouldBe("token-123");
            output.TenancyName.ShouldBe("TenantA");
        }

        [Fact]
        public void Dado_IsTenantAvailableInput_Quando_DefinirTenancyName_Entao_DeveRetornarCorreto()
        {
            var input = new IsTenantAvailableInput
            {
                TenancyName = "my-tenant"
            };

            input.TenancyName.ShouldBe("my-tenant");
        }

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_DefinirEmail_Entao_DeveRetornarCorreto()
        {
            var input = new SendPasswordResetCodeInput
            {
                EmailAddress = "user@test.com"
            };

            input.EmailAddress.ShouldBe("user@test.com");
        }

        [Fact]
        public void Dado_ResetPasswordInput_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var input = new ResetPasswordInput
            {
                Password = "newpass123",
                ResetCode = "abc-reset",
                UserId = 10,
                ReturnUrl = "/login",
                SingleSignIn = "true",
                AuthenticationSource = "Local"
            };

            input.Password.ShouldBe("newpass123");
            input.ResetCode.ShouldBe("abc-reset");
            input.UserId.ShouldBe(10);
            input.ReturnUrl.ShouldBe("/login");
            input.SingleSignIn.ShouldBe("true");
            input.AuthenticationSource.ShouldBe("Local");
        }

        [Fact]
        public void Dado_ResetPasswordInput_Quando_cVazio_Entao_NormalizeNaoDeveAlterarPropriedades()
        {
            var input = new ResetPasswordInput
            {
                UserId = 5,
                ResetCode = "code",
                c = ""
            };

            input.Normalize();
            input.UserId.ShouldBe(5);
            input.ResetCode.ShouldBe("code");
        }

        [Fact]
        public void Dado_ResetPasswordInput_Quando_cNull_Entao_NormalizeNaoDeveAlterarPropriedades()
        {
            var input = new ResetPasswordInput
            {
                UserId = 5,
                ResetCode = "code"
            };

            input.Normalize();
            input.UserId.ShouldBe(5);
            input.ResetCode.ShouldBe("code");
        }
    }
}
