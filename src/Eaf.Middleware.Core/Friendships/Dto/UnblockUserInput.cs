using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Friendships.Dto
{
    /// <summary>
    /// Representa a classe UnblockUserInput.
    /// </summary>
    public class UnblockUserInput
    {
        public int? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}