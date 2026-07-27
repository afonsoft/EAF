using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// DTO (Data Transfer Object) para ChatMessage.
    /// </summary>
    [AutoMap(typeof(ChatMessage))]
    public class ChatMessageDto : EntityDto
    {
        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// Obtém ou define Message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Obtém ou define ReadState.
        /// </summary>
        public ChatMessageReadState ReadState { get; set; }
        /// <summary>
        /// Obtém ou define ReceiverReadState.
        /// </summary>
        public ChatMessageReadState ReceiverReadState { get; set; }
        /// <summary>
        /// Obtém ou define SharedMessageId.
        /// </summary>
        public string SharedMessageId { get; set; }
        /// <summary>
        /// Obtém ou define Side.
        /// </summary>
        public ChatSide Side { get; set; }
        public int? TargetTenantId { get; set; }
        /// <summary>
        /// Obtém ou define TargetUserId.
        /// </summary>
        public long TargetUserId { get; set; }
        /// <summary>
        /// Obtém ou define TargetUserName.
        /// </summary>
        public string TargetUserName { get; set; }
        public int? TenantId { get; set; }
        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }

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

        /// <summary>
        /// Client-generated idempotency key to avoid duplicate messages on retries.
        /// </summary>
        public string ClientMessageId { get;  set; }
    }
}