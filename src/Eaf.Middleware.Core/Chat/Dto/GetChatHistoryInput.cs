using System;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Input for retrieving contextual chat history.
    /// </summary>
    public class GetChatHistoryInput
    {
        /// <summary>
        /// Conversation identifier for contextual chat grouping.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Optional game identifier to filter messages related to a game.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// Optional match identifier to filter messages related to a multiplayer match.
        /// </summary>
        public Guid? MatchId { get; set; }

        /// <summary>
        /// Context type describing where the message was produced.
        /// </summary>
        public string ContextType { get; set; }

        /// <summary>
        /// Minimum message id for pagination.
        /// </summary>
        public long? MinMessageId { get; set; }

        /// <summary>
        /// Maximum number of messages to return.
        /// </summary>
        public int? MaxResultCount { get; set; }
    }
}
