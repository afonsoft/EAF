using Abp.Application.Services.Dto;
using Eaf.Middleware.Friendships;
using System;

namespace Eaf.Middleware.Chat.Dto
{
    /// <summary>
    /// Representa a classe ChatUserDto.
    /// </summary>
    public class ChatUserDto : EntityDto<long>
    {
        /// <summary>
        /// Obtém ou define IsOnline.
        /// </summary>
        public bool IsOnline { get; set; }
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define State.
        /// </summary>
        public FriendshipState State { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
        public int? TenantId { get; set; }
        /// <summary>
        /// Obtém ou define UnreadMessageCount.
        /// </summary>
        public int UnreadMessageCount { get; set; }
        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}