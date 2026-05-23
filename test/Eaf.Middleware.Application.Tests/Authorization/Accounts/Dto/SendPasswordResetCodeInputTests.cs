using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class SendPasswordResetCodeInputTests
    {
        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_Criado_Entao_EmailDeveSerNulo()
        {
            var input = new SendPasswordResetCodeInput();
            input.EmailAddress.ShouldBeNull();
        }

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_AtribuirEmail_Entao_DeveRetornarValor()
        {
            var input = new SendPasswordResetCodeInput { EmailAddress = "admin@test.com" };
            input.EmailAddress.ShouldBe("admin@test.com");
        }

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_Verificado_Entao_EmailDeveConterRequiredAttribute()
        {
            var prop = typeof(SendPasswordResetCodeInput).GetProperty(nameof(SendPasswordResetCodeInput.EmailAddress));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_SendPasswordResetCodeInput_Quando_Verificado_Entao_EmailDeveConterMaxLengthAttribute()
        {
            var prop = typeof(SendPasswordResetCodeInput).GetProperty(nameof(SendPasswordResetCodeInput.EmailAddress));
            var attr = prop!.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;
            attr.ShouldNotBeNull();
            attr!.Length.ShouldBeGreaterThan(0);
        }
    }
}
