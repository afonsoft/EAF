using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserRoleDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserRoleDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirRoleDisplayName_Entao_DeveArmazenar()
        {
            var sut = new UserRoleDto();
            sut.RoleDisplayName = "test_value";
            sut.RoleDisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirRoleId_Entao_DeveArmazenar()
        {
            var sut = new UserRoleDto();
            sut.RoleId = 42;
            sut.RoleId.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirRoleName_Entao_DeveArmazenar()
        {
            var sut = new UserRoleDto();
            sut.RoleName = "test_value";
            sut.RoleName.ShouldBe("test_value");
        }
    }
}
