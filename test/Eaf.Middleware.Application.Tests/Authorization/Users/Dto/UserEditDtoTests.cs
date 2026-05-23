using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserEditDtoTests
    {
        [Fact]
        public void Dado_UserEditDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserEditDto();

            dto.EmailAddress.ShouldBeNull();
            dto.Id.ShouldBeNull();
            dto.IsActive.ShouldBeFalse();
            dto.IsLockoutEnabled.ShouldBeFalse();
            dto.Name.ShouldBeNull();
            dto.Password.ShouldBeNull();
            dto.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
            dto.Surname.ShouldBeNull();
            dto.UserName.ShouldBeNull();
            dto.PhoneNumber.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserEditDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new UserEditDto
            {
                EmailAddress = "admin@test.com",
                Id = 42L,
                IsActive = true,
                IsLockoutEnabled = true,
                Name = "Admin",
                Password = "P@ssw0rd",
                ShouldChangePasswordOnNextLogin = true,
                Surname = "User",
                UserName = "admin",
                PhoneNumber = "+5511999999999"
            };

            dto.EmailAddress.ShouldBe("admin@test.com");
            dto.Id.ShouldBe(42L);
            dto.IsActive.ShouldBeTrue();
            dto.IsLockoutEnabled.ShouldBeTrue();
            dto.Name.ShouldBe("Admin");
            dto.Password.ShouldBe("P@ssw0rd");
            dto.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
            dto.Surname.ShouldBe("User");
            dto.UserName.ShouldBe("admin");
            dto.PhoneNumber.ShouldBe("+5511999999999");
        }

        [Theory]
        [InlineData(nameof(UserEditDto.EmailAddress))]
        [InlineData(nameof(UserEditDto.Name))]
        [InlineData(nameof(UserEditDto.Surname))]
        [InlineData(nameof(UserEditDto.UserName))]
        public void Dado_UserEditDto_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(UserEditDto).GetProperty(propertyName);
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_UserEditDto_Quando_Verificado_Entao_EmailDeveConterEmailAddressAttribute()
        {
            var prop = typeof(UserEditDto).GetProperty(nameof(UserEditDto.EmailAddress));
            prop!.GetCustomAttributes(typeof(EmailAddressAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
