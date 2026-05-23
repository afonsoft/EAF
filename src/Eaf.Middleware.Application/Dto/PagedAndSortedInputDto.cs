using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Dto
{
    /// <summary>
    /// Representa a classe PagedAndSortedInputDto.
    /// </summary>
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        /// <summary>
        /// PagedAndSortedInputDto.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public PagedAndSortedInputDto()
        {
            MaxResultCount = MiddlewareAppConsts.DefaultPageSize;
        }

        /// <summary>
        /// Obtém ou define Sorting.
        /// </summary>
        public string Sorting { get; set; } = "";
    }
}