using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Dto
{
    /// <summary>
    /// Representa a classe PagedInputDto.
    /// </summary>
    public class PagedInputDto : IPagedResultRequest
    {
        /// <summary>
        /// PagedInputDto.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public PagedInputDto()
        {
            MaxResultCount = MiddlewareAppConsts.DefaultPageSize;
        }

        [Range(1, MiddlewareAppConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; } = 0;
    }
}