using System;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Input for marking chat messages as read in a contextual conversation.
    /// </summary>
    public class MarkChatReadInput
    {
        /// <summary>
        /// Conversation identifier for contextual chat grouping.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Optional game identifier when the message is related to a game.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// Optional match identifier when the message is related to a multiplayer match.
        /// </summary>
        public Guid? MatchId { get; set; }

        /// <summary>
        /// Target user identifier for direct messages.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Target tenant identifier for direct messages.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Target group identifier for group messages.
        /// </summary>
        public long? GroupId { get; set; }
    }
}
