using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Friendships.Dto
{
    /// <summary>
    /// Representa a classe CreateFriendshipRequestInput.
    /// </summary>
    public class CreateFriendshipRequestInput
    {
        public int? TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}