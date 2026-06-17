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
    }
}