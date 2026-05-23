using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserListRoleDtoTests
    {
        [Fact]
        public void Dado_UserListRoleDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserListRoleDto();
            dto.RoleId.ShouldBe(0);
            dto.RoleName.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserListRoleDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new UserListRoleDto { RoleId = 5, RoleName = "Admin" };
            dto.RoleId.ShouldBe(5);
            dto.RoleName.ShouldBe("Admin");
        }
    }
}
