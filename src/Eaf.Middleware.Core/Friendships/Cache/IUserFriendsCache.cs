using Abp;

namespace Eaf.Middleware.Friendships.Cache
{
    /// <summary>
    /// Representa a interface IUserFriendsCache.
    /// </summary>
    public interface IUserFriendsCache
    {
        void AddFriend(UserIdentifier userIdentifier, FriendCacheItem friend);

        UserWithFriendsCacheItem GetCacheItem(UserIdentifier userIdentifier);

        UserWithFriendsCacheItem GetCacheItemOrNull(UserIdentifier userIdentifier);

        void IncreaseUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier, int change);

        void RemoveFriend(UserIdentifier userIdentifier, FriendCacheItem friend);

        void ResetUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier);

        void UpdateFriend(UserIdentifier userIdentifier, FriendCacheItem friend);
    }
}