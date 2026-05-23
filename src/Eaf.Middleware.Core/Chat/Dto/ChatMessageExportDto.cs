using System;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe ChatMessageExportDto.
    /// </summary>
    public class ChatMessageExportDto
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
        /// Obtém ou define Side.
        /// </summary>
        public ChatSide Side { get; set; }
        public int? TargetTenantId { get; set; }
        /// <summary>
        /// Obtém ou define TargetTenantName.
        /// </summary>
        public string TargetTenantName { get; set; }
        /// <summary>
        /// Obtém ou define TargetUserId.
        /// </summary>
        public long TargetUserId { get; set; }

        /// <summary>
        /// Obtém ou define TargetUserName.
        /// </summary>
        public string TargetUserName { get; set; }
    }
}