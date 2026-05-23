using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserRoleDtoTests
    {
        [Fact]
        public void Dado_UserRoleDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserRoleDto();

            dto.IsAssigned.ShouldBeFalse();
            dto.RoleDisplayName.ShouldBeNull();
            dto.RoleId.ShouldBe(0);
            dto.RoleName.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserRoleDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new UserRoleDto
            {
                IsAssigned = true,
                RoleDisplayName = "Administrator",
                RoleId = 1,
                RoleName = "Admin"
            };

            dto.IsAssigned.ShouldBeTrue();
            dto.RoleDisplayName.ShouldBe("Administrator");
            dto.RoleId.ShouldBe(1);
            dto.RoleName.ShouldBe("Admin");
        }
    }
}
