using Eaf.Middleware.Dto;

namespace Eaf.Middleware.Common.Dto
{
    /// <summary>
    /// Representa a classe FindUsersInput.
    /// </summary>
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}