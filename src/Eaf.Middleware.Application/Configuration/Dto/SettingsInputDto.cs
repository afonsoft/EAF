using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Configuration.Dto
{
    /// <summary>
    /// Representa a classe SettingsInputDto.
    /// </summary>
    public class SettingsInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Obtém ou define Filter.
        /// </summary>
        public string Filter { get; set; } = null;
    }
}