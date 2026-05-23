using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class UserLoginInfoDtoTests
    {
        [Fact]
        public void Dado_UserLoginInfoDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserLoginInfoDto();

            dto.AuthenticationSource.ShouldBeNull();
            dto.EmailAddress.ShouldBeNull();
            dto.Name.ShouldBeNull();
            dto.ProfilePictureId.ShouldBeNull();
            dto.Surname.ShouldBeNull();
            dto.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserLoginInfoDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new UserLoginInfoDto
            {
                AuthenticationSource = "AzureAD",
                EmailAddress = "user@test.com",
                Name = "Admin",
                ProfilePictureId = "pic-123",
                Surname = "User",
                UserName = "admin"
            };

            dto.AuthenticationSource.ShouldBe("AzureAD");
            dto.EmailAddress.ShouldBe("user@test.com");
            dto.Name.ShouldBe("Admin");
            dto.ProfilePictureId.ShouldBe("pic-123");
            dto.Surname.ShouldBe("User");
            dto.UserName.ShouldBe("admin");
        }
    }
}
