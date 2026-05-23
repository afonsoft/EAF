using Abp;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe MarkAllUnreadMessagesOfUserAsReadInput.
    /// </summary>
    public class MarkAllUnreadMessagesOfUserAsReadInput
    {
        public int? TenantId { get; set; }

        public long? UserId { get; set; }

        public long? GroupId { get; set; }

        /// <summary>
        /// ToUserIdentifier.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, (UserId ?? GroupId).Value);
        }
    }
}