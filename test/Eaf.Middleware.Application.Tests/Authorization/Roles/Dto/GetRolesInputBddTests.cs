using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Roles
{
    public class GetRolesInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetRolesInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new GetRolesInput();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSorting_Entao_DeveArmazenar()
        {
            var sut = new GetRolesInput();
            sut.Sorting = "test_value";
            sut.Sorting.ShouldBe("test_value");
        }
    }
}
