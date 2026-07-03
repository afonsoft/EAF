using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Roles
{
    public class RoleListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new RoleListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new RoleListDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsDefault_Entao_DeveArmazenar()
        {
            var sut = new RoleListDto();
            sut.IsDefault = true;
            sut.IsDefault.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsStatic_Entao_DeveArmazenar()
        {
            var sut = new RoleListDto();
            sut.IsStatic = true;
            sut.IsStatic.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new RoleListDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }
    }
}
