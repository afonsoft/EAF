using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class CreateOrUpdateUserInputTests
    {
        [Fact]
        public void Dado_CreateOrUpdateUserInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new CreateOrUpdateUserInput();

            input.AssignedRoleNames.ShouldBeNull();
            input.SendActivationEmail.ShouldBeFalse();
            input.SetRandomPassword.ShouldBeFalse();
            input.User.ShouldBeNull();
        }

        [Fact]
        public void Dado_CreateOrUpdateUserInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var user = new UserEditDto { UserName = "admin" };
            var input = new CreateOrUpdateUserInput
            {
                AssignedRoleNames = new[] { "Admin", "User" },
                SendActivationEmail = true,
                SetRandomPassword = true,
                User = user
            };

            input.AssignedRoleNames.Length.ShouldBe(2);
            input.SendActivationEmail.ShouldBeTrue();
            input.SetRandomPassword.ShouldBeTrue();
            input.User.UserName.ShouldBe("admin");
        }

        [Theory]
        [InlineData(nameof(CreateOrUpdateUserInput.AssignedRoleNames))]
        [InlineData(nameof(CreateOrUpdateUserInput.User))]
        public void Dado_CreateOrUpdateUserInput_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(CreateOrUpdateUserInput).GetProperty(propertyName);
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
