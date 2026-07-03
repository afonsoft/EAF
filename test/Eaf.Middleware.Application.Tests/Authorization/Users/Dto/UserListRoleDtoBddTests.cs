using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserListRoleDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserListRoleDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirRoleName_Entao_DeveArmazenar()
        {
            var sut = new UserListRoleDto();
            sut.RoleName = "test_value";
            sut.RoleName.ShouldBe("test_value");
        }
    }
}
