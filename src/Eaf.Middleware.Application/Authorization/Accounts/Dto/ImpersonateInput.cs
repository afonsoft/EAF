using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe ImpersonateInput.
    /// </summary>
    public class ImpersonateInput
    {
        public int? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}