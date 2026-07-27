using System;

namespace Eaf.Middleware.Contracts
{
    /// <summary>
    /// Contract for contextual chat messages shared across services.
    /// </summary>
    public class ContextualChatMessageContract
    {
        /// <summary>
        /// Unique message identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Identifier of the sender user.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Conversation identifier for contextual chat grouping.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Optional game identifier.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// Optional match identifier.
        /// </summary>
        public Guid? MatchId { get; set; }

        /// <summary>
        /// Context type (e.g., lobby, match, team).
        /// </summary>
        public string ContextType { get; set; }

        /// <summary>
        /// Client-generated idempotency key.
        /// </summary>
        public string ClientMessageId { get; set; }

        /// <summary>
        /// Message creation timestamp.
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
