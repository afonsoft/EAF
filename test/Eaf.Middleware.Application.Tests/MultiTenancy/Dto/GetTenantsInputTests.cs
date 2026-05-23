using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class GetTenantsInputTests
    {
        [Fact]
        public void Dado_GetTenantsInput_Quando_Criado_Entao_FilterDeveSerStringVazia()
        {
            var input = new GetTenantsInput();
            input.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_GetTenantsInput_Quando_SortingNulo_Entao_NormalizeDeveDefinirComoTenancyName()
        {
            var input = new GetTenantsInput();
            input.Normalize();
            input.Sorting.ShouldBe("TenancyName");
        }

        [Fact]
        public void Dado_GetTenantsInput_Quando_SortingDefinido_Entao_NormalizeNaoDeveAlterar()
        {
            var input = new GetTenantsInput { Sorting = "Name DESC" };
            input.Normalize();
            input.Sorting.ShouldBe("Name DESC");
        }
    }
}
