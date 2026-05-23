namespace Eaf.Middleware.Dto
{
    /// <summary>
    /// Representa a classe PagedSortedAndFilteredInputDto.
    /// </summary>
    public class PagedSortedAndFilteredInputDto : PagedAndSortedInputDto
    {
        /// <summary>
        /// Obtém ou define Filter.
        /// </summary>
        public string Filter { get; set; } = "";
    }
}