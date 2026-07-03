using Eaf.Middleware.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class PagedAndFilteredInputDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new PagedAndFilteredInputDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new PagedAndFilteredInputDto();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMaxResultCount_Entao_DeveArmazenar()
        {
            var sut = new PagedAndFilteredInputDto();
            sut.MaxResultCount = 42;
            sut.MaxResultCount.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSkipCount_Entao_DeveArmazenar()
        {
            var sut = new PagedAndFilteredInputDto();
            sut.SkipCount = 42;
            sut.SkipCount.ShouldBe(42);
        }
    }
}
