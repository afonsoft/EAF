using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class GetRolesInputTests
    {
        [Fact]
        public void Dado_GetRolesInput_Quando_Criado_Entao_FilterDeveSerStringVazia()
        {
            var input = new GetRolesInput();
            input.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_SortingNulo_Entao_NormalizeDeveDefinirComoName()
        {
            var input = new GetRolesInput();
            input.Normalize();
            input.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_SortingDefinido_Entao_NormalizeNaoDeveAlterar()
        {
            var input = new GetRolesInput { Sorting = "DisplayName DESC" };
            input.Normalize();
            input.Sorting.ShouldBe("DisplayName DESC");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_SortingVazio_Entao_NormalizeDeveDefinirComoName()
        {
            var input = new GetRolesInput { Sorting = "" };
            input.Normalize();
            input.Sorting.ShouldBe("Name");
        }
    }
}
