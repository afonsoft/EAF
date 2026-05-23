using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Friendships.Cache
{
    /// <summary>
    /// Representa a classe FriendCacheItemExtensions.
    /// </summary>
    public static class FriendCacheItemExtensions
    {
        /// <summary>
        /// ContainsFriend.
        /// </summary>
        /// <param name="items">Parâmetro items.</param>
        /// <param name="item">Parâmetro item.</param>
        /// <returns>Resultado da operação.</returns>
        public static bool ContainsFriend(this List<FriendCacheItem> items, FriendCacheItem item)
        {
            return items.Any(f => f.FriendTenantId == item.FriendTenantId && f.FriendUserId == item.FriendUserId);
        }
    }
}