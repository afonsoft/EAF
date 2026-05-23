using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class SendEmailActivationLinkInputTests
    {
        [Fact]
        public void Dado_SendEmailActivationLinkInput_Quando_Criado_Entao_EmailDeveSerNulo()
        {
            var input = new SendEmailActivationLinkInput();
            input.EmailAddress.ShouldBeNull();
        }

        [Fact]
        public void Dado_SendEmailActivationLinkInput_Quando_AtribuirEmail_Entao_DeveRetornarValor()
        {
            var input = new SendEmailActivationLinkInput { EmailAddress = "test@example.com" };
            input.EmailAddress.ShouldBe("test@example.com");
        }

        [Fact]
        public void Dado_SendEmailActivationLinkInput_Quando_Verificado_Entao_EmailDeveConterRequiredAttribute()
        {
            var prop = typeof(SendEmailActivationLinkInput).GetProperty(nameof(SendEmailActivationLinkInput.EmailAddress));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}
