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
    }
}