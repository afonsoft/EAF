using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Chat

{
    /// <summary>
    /// Representa a classe ChatMessage.
    /// </summary>
    [Table("EafChatMessages")]
    public class ChatMessage : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        /// <summary>
        /// ChatMessage.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="targetUser">Parâmetro targetUser.</param>
        /// <param name="side">Parâmetro side.</param>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="readState">Parâmetro readState.</param>
        /// <param name="sharedMessageId">Parâmetro sharedMessageId.</param>
        /// <param name="receiverReadState">Parâmetro receiverReadState.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatMessage(
            UserIdentifier user,
            UserIdentifier targetUser,
            ChatSide side,
            string message,
            ChatMessageReadState readState,
            Guid sharedMessageId,
            ChatMessageReadState receiverReadState)
        {
            UserId = user.UserId;
            TenantId = user.TenantId;
            TargetUserId = targetUser.UserId;
            TargetTenantId = targetUser.TenantId;
            Message = message;
            Side = side;
            ReadState = readState;
            SharedMessageId = sharedMessageId;
            ReceiverReadState = receiverReadState;

            CreationTime = Clock.Now;
        }

        protected ChatMessage()
        {
        }

        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }

        [Required]
        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        /// <summary>
        /// Obtém ou define ReadState.
        /// </summary>
        public ChatMessageReadState ReadState { get; private set; }
        /// <summary>
        /// Obtém ou define ReceiverReadState.
        /// </summary>
        public ChatMessageReadState ReceiverReadState { get; private set; }
        public Guid? SharedMessageId { get; set; }
        /// <summary>
        /// Obtém ou define Side.
        /// </summary>
        public ChatSide Side { get; set; }
        public int? TargetTenantId { get; set; }
        /// <summary>
        /// Obtém ou define TargetUserId.
        /// </summary>
        public long TargetUserId { get; set; }
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
        /// Context type describing where the message was produced (e.g., lobby, match, team).
        /// </summary>
        [StringLength(64)]
        public string ContextType { get; set; }

        /// <summary>
        /// Client-generated idempotency key to avoid duplicate messages on retries.
        /// </summary>
        [StringLength(64)]
        public string ClientMessageId { get; set; }

        /// <summary>
        /// ChangeReadState.
        /// </summary>
        /// <param name="newState">Parâmetro newState.</param>
        public void ChangeReadState(ChatMessageReadState newState)
        {
            ReadState = newState;
        }

        /// <summary>
        /// ChangeReceiverReadState.
        /// </summary>
        /// <param name="newState">Parâmetro newState.</param>
        public void ChangeReceiverReadState(ChatMessageReadState newState)
        {
            ReceiverReadState = newState;
        }
    }
}