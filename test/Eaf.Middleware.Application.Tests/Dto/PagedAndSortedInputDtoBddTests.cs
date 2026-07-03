using Eaf.Middleware.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class PagedAndSortedInputDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new PagedAndSortedInputDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSorting_Entao_DeveArmazenar()
        {
            var sut = new PagedAndSortedInputDto();
            sut.Sorting = "test_value";
            sut.Sorting.ShouldBe("test_value");
        }
    }
}
