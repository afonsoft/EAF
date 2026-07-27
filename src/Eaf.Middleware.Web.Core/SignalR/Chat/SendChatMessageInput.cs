using System;

namespace Eaf.AspNetCore.SignalR.Chat
{
    /// <summary>
    /// Representa a classe SendChatMessageInput.
    /// </summary>
    public class SendChatMessageInput
    {
        /// <summary>
        /// Obtém ou define Message.
        /// </summary>
        public string Message { get; set; }
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
        public int? TenantId { get; set; }
        public long? UserId { get; set; }
        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
        public long? GroupId { get; set; }

        /// <summary>
        /// Client-generated idempotency key to avoid duplicate messages on retries.
        /// </summary>
        public string ClientMessageId { get; set; }

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
        /// Context type describing where the message was produced.
        /// </summary>
        public string ContextType { get; set; }
    }
}