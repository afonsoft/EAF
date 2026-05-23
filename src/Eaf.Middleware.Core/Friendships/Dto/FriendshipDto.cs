using Abp.AutoMapper;
using System;

namespace Eaf.Middleware.Friendships.Dto
{
    [AutoMap(typeof(Friendship))]
    public class FriendshipDto
    {
        public Guid? FriendProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define FriendTenancyName.
        /// </summary>
        public string FriendTenancyName { get; set; }
        public int? FriendTenantId { get; set; }
        /// <summary>
        /// Obtém ou define FriendUserId.
        /// </summary>
        public long FriendUserId { get; set; }
        /// <summary>
        /// Obtém ou define FriendUserName.
        /// </summary>
        public string FriendUserName { get; set; }
        /// <summary>
        /// Obtém ou define IsOnline.
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// Obtém ou define State.
        /// </summary>
        public FriendshipState State { get; set; }
        /// <summary>
        /// Obtém ou define UnreadMessageCount.
        /// </summary>
        public int UnreadMessageCount { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define Surname.
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Obtém ou define Email.
        /// </summary>
        public string Email { get; set; }
        public long? GroupId { get; set; }
    }
}