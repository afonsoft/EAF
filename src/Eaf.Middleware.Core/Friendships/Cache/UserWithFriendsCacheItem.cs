using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Friendships.Cache
{
    /// <summary>
    /// Representa a classe UserWithFriendsCacheItem.
    /// </summary>
    public class UserWithFriendsCacheItem
    {
        public List<FriendCacheItem> Friends { get; set; }
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
        public int? TenantId { get; set; }

        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
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
    }
}