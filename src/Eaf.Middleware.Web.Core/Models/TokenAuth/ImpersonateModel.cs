using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe ImpersonateModel.
    /// </summary>
    public class ImpersonateModel
    {
        public int? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}