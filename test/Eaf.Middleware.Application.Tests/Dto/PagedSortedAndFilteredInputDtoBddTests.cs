using Eaf.Middleware.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class PagedSortedAndFilteredInputDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new PagedSortedAndFilteredInputDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new PagedSortedAndFilteredInputDto();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }
    }
}
