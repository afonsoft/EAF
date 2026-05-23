using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Profile.Dto
{
    public class ChangePasswordInputTests
    {
        [Fact]
        public void Dado_ChangePasswordInput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var input = new ChangePasswordInput();
            input.CurrentPassword.ShouldBeNull();
            input.NewPassword.ShouldBeNull();
        }

        [Fact]
        public void Dado_ChangePasswordInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var input = new ChangePasswordInput
            {
                CurrentPassword = "OldP@ss",
                NewPassword = "NewP@ss123"
            };

            input.CurrentPassword.ShouldBe("OldP@ss");
            input.NewPassword.ShouldBe("NewP@ss123");
        }

        [Theory]
        [InlineData(nameof(ChangePasswordInput.CurrentPassword))]
        [InlineData(nameof(ChangePasswordInput.NewPassword))]
        public void Dado_ChangePasswordInput_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(ChangePasswordInput).GetProperty(propertyName);
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
