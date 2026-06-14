using Eaf.Middleware.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Dto
{
    public class PagedInputDtoBddTests
    {
        [Fact]
        public void Dado_PagedInputDto_Quando_CriarNovo_Entao_MaxResultCountDeveSerDefaultPageSize()
        {
            var dto = new PagedInputDto();

            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.SkipCount.ShouldBe(0);
        }

        [Fact]
        public void Dado_PagedInputDto_Quando_DefinirMaxResultCount_Entao_DevePersistir()
        {
            var dto = new PagedInputDto { MaxResultCount = 50 };

            dto.MaxResultCount.ShouldBe(50);
        }

        [Fact]
        public void Dado_PagedAndFilteredInputDto_Quando_CriarNovo_Entao_DeveInicializarComDefaults()
        {
            var dto = new PagedAndFilteredInputDto();

            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.SkipCount.ShouldBe(0);
            dto.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_PagedAndFilteredInputDto_Quando_DefinirFilter_Entao_DevePersistir()
        {
            var dto = new PagedAndFilteredInputDto { Filter = "busca" };

            dto.Filter.ShouldBe("busca");
        }

        [Fact]
        public void Dado_PagedAndSortedInputDto_Quando_CriarNovo_Entao_DeveInicializarComDefaults()
        {
            var dto = new PagedAndSortedInputDto();

            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.Sorting.ShouldBe("");
        }

        [Fact]
        public void Dado_PagedAndSortedInputDto_Quando_DefinirSorting_Entao_DevePersistir()
        {
            var dto = new PagedAndSortedInputDto { Sorting = "Name ASC" };

            dto.Sorting.ShouldBe("Name ASC");
        }
    }
}
