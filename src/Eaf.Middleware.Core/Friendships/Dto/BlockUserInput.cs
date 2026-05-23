using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Friendships.Dto
{
    /// <summary>
    /// Representa a classe BlockUserInput.
    /// </summary>
    public class BlockUserInput
    {
        public int? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}