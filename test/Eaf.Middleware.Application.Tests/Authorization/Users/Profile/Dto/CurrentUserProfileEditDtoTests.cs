using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Profile.Dto
{
    public class CurrentUserProfileEditDtoTests
    {
        [Fact]
        public void Dado_CurrentUserProfileEditDto_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var dto = new CurrentUserProfileEditDto();

            dto.EmailAddress.ShouldBeNull();
            dto.Name.ShouldBeNull();
            dto.Surname.ShouldBeNull();
            dto.Timezone.ShouldBeNull();
            dto.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_CurrentUserProfileEditDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new CurrentUserProfileEditDto
            {
                EmailAddress = "user@test.com",
                Name = "Admin",
                Surname = "User",
                Timezone = "America/Sao_Paulo",
                UserName = "admin"
            };

            dto.EmailAddress.ShouldBe("user@test.com");
            dto.Name.ShouldBe("Admin");
            dto.Surname.ShouldBe("User");
            dto.Timezone.ShouldBe("America/Sao_Paulo");
            dto.UserName.ShouldBe("admin");
        }

        [Theory]
        [InlineData(nameof(CurrentUserProfileEditDto.EmailAddress))]
        [InlineData(nameof(CurrentUserProfileEditDto.Name))]
        [InlineData(nameof(CurrentUserProfileEditDto.Surname))]
        [InlineData(nameof(CurrentUserProfileEditDto.UserName))]
        public void Dado_CurrentUserProfileEditDto_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(CurrentUserProfileEditDto).GetProperty(propertyName);
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
