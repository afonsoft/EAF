using Abp.Runtime.Security;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.Web;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class ResetPasswordInputBddTests
    {
        [Fact]
        public void Dado_ResetPasswordInput_ComCEncriptado_Quando_Normalize_Entao_DeveExtrairUserId()
        {
            var queryString = "userId=42&resetCode=ABC123";
            var encrypted = SimpleStringCipher.Instance.Encrypt(queryString);

            var input = new ResetPasswordInput { c = encrypted };

            input.Normalize();

            input.UserId.ShouldBe(42);
            input.ResetCode.ShouldBe("ABC123");
        }

        [Fact]
        public void Dado_ResetPasswordInput_ComAuthSource_Quando_Normalize_Entao_DeveExtrairAuthenticationSource()
        {
            var queryString = "userId=1&resetCode=XYZ&authenticationSource=LDAP";
            var encrypted = SimpleStringCipher.Instance.Encrypt(queryString);

            var input = new ResetPasswordInput { c = encrypted };

            input.Normalize();

            input.UserId.ShouldBe(1);
            input.ResetCode.ShouldBe("XYZ");
            input.AuthenticationSource.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_ResetPasswordInput_SemC_Quando_Normalize_Entao_NaoDeveMudarPropriedades()
        {
            var input = new ResetPasswordInput
            {
                c = null,
                UserId = 0,
                ResetCode = null
            };

            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ResetCode.ShouldBeNull();
        }

        [Fact]
        public void Dado_ResetPasswordInput_ComCVazio_Quando_Normalize_Entao_NaoDeveMudarPropriedades()
        {
            var input = new ResetPasswordInput { c = "" };

            input.Normalize();

            input.UserId.ShouldBe(0);
        }

        [Fact]
        public void Dado_ResetPasswordInput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var input = new ResetPasswordInput
            {
                Password = "StrongP@ss123",
                ReturnUrl = "https://app.example.com",
                SingleSignIn = "true"
            };

            input.Password.ShouldBe("StrongP@ss123");
            input.ReturnUrl.ShouldBe("https://app.example.com");
            input.SingleSignIn.ShouldBe("true");
        }
    }

    public class ActivateEmailInputBddTests
    {
        [Fact]
        public void Dado_ActivateEmailInput_ComCEncriptado_Quando_Normalize_Entao_DeveExtrairUserIdEConfirmationCode()
        {
            var queryString = "userId=10&confirmationCode=CONF-CODE";
            var encrypted = SimpleStringCipher.Instance.Encrypt(queryString);

            var input = new ActivateEmailInput { c = encrypted };

            input.Normalize();

            input.UserId.ShouldBe(10);
            input.ConfirmationCode.ShouldBe("CONF-CODE");
        }

        [Fact]
        public void Dado_ActivateEmailInput_SemC_Quando_Normalize_Entao_NaoDeveMudarPropriedades()
        {
            var input = new ActivateEmailInput { c = null };

            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ConfirmationCode.ShouldBeNull();
        }

        [Fact]
        public void Dado_ActivateEmailInput_ComCVazio_Quando_Normalize_Entao_NaoDeveMudarPropriedades()
        {
            var input = new ActivateEmailInput { c = "" };

            input.Normalize();

            input.UserId.ShouldBe(0);
            input.ConfirmationCode.ShouldBeNull();
        }
    }

    public class ResolveTenantIdInputBddTests
    {
        [Fact]
        public void Dado_ResolveTenantIdInput_Quando_DefinirC_Entao_DevePersistir()
        {
            var input = new ResolveTenantIdInput { c = "encrypted-value" };

            input.c.ShouldBe("encrypted-value");
        }

        [Fact]
        public void Dado_ResolveTenantIdInput_Quando_CriarNovo_Entao_CDeveSerNull()
        {
            var input = new ResolveTenantIdInput();

            input.c.ShouldBeNull();
        }
    }
}
