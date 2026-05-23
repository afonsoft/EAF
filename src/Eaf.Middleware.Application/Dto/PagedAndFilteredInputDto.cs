using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Dto
{
    /// <summary>
    /// Representa a classe PagedAndFilteredInputDto.
    /// </summary>
    public class PagedAndFilteredInputDto : IPagedResultRequest
    {
        /// <summary>
        /// PagedAndFilteredInputDto.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public PagedAndFilteredInputDto()
        {
            MaxResultCount = MiddlewareAppConsts.DefaultPageSize;
        }

        /// <summary>
        /// Obtém ou define Filter.
        /// </summary>
        public string Filter { get; set; } = "";

        [Range(1, MiddlewareAppConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }
    }
}