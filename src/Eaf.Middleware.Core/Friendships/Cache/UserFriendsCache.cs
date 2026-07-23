using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Friendships.Cache
{
    /// <summary>
    /// Representa a classe UserFriendsCache.
    /// </summary>
    public class UserFriendsCache : IUserFriendsCache, ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<Friendship, long> _friendshipRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly object _syncObj = new object();
        private readonly ITenantCache _tenantCache;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserStore _userStore;

        /// <summary>
        /// UserFriendsCache.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <param name="friendshipRepository">Parâmetro friendshipRepository.</param>
        /// <param name="chatMessageRepository">Parâmetro chatMessageRepository.</param>
        /// <param name="userRepository">Parâmetro userRepository.</param>
        /// <param name="tenantCache">Parâmetro tenantCache.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <param name="userStore">Parâmetro userStore.</param>
        /// <returns>Resultado da operação.</returns>
        public UserFriendsCache(
            ICacheManager cacheManager,
            IRepository<Friendship, long> friendshipRepository,
            IRepository<ChatMessage, long> chatMessageRepository,
            IRepository<User, long> userRepository,
            ITenantCache tenantCache,
            IUnitOfWorkManager unitOfWorkManager,
            UserStore userStore)
        {
            _cacheManager = cacheManager;
            _friendshipRepository = friendshipRepository;
            _chatMessageRepository = chatMessageRepository;
            _userRepository = userRepository;
            _tenantCache = tenantCache;
            _unitOfWorkManager = unitOfWorkManager;
            _userStore = userStore;
        }

        [UnitOfWork]
        public void AddFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                if (!user.Friends.ContainsFriend(friend))
                {
                    user.Friends.Add(friend);
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        [UnitOfWork]
        public virtual UserWithFriendsCacheItem GetCacheItem(UserIdentifier userIdentifier)
        {
            return _cacheManager
                .GetCache(FriendCacheItem.CacheName)
                .AsTyped<string, UserWithFriendsCacheItem>()
                .Get(userIdentifier.ToUserIdentifierString(), f => GetUserFriendsCacheItemInternal(userIdentifier));
        }

        /// <summary>
        /// GetCacheItemOrNull.
        /// </summary>
        /// <param name="userIdentifier">Parâmetro userIdentifier.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual UserWithFriendsCacheItem GetCacheItemOrNull(UserIdentifier userIdentifier)
        {
            return _cacheManager
                .GetCache(FriendCacheItem.CacheName)
                .AsTyped<string, UserWithFriendsCacheItem>()
                .GetOrDefault(userIdentifier.ToUserIdentifierString());
        }

        [UnitOfWork]
        public virtual void IncreaseUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier, int change)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                var friend = user.Friends.FirstOrDefault(
                     f => f.FriendUserId == friendIdentifier.UserId &&
                     f.FriendTenantId == friendIdentifier.TenantId
                );

                if (friend == null)
                {
                    return;
                }

                friend.UnreadMessageCount += change;
                UpdateUserOnCache(userIdentifier, user);
            }
        }

        [UnitOfWork]
        public void RemoveFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                if (user.Friends.ContainsFriend(friend))
                {
                    user.Friends.RemoveAll(f => f.FriendTenantId == friend.FriendTenantId && f.FriendUserId == friend.FriendUserId);
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        [UnitOfWork]
        public virtual void ResetUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                var friend = user.Friends.FirstOrDefault(
                     f => f.FriendUserId == friendIdentifier.UserId &&
                     f.FriendTenantId == friendIdentifier.TenantId
                 );

                if (friend == null)
                {
                    return;
                }

                friend.UnreadMessageCount = 0;
                UpdateUserOnCache(userIdentifier, user);
            }
        }

        [UnitOfWork]
        public void UpdateFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                var existingFriendIndex = user.Friends.FindIndex(
                    f => f.FriendUserId == friend.FriendUserId &&
                    f.FriendTenantId == friend.FriendTenantId
                );

                if (existingFriendIndex >= 0)
                {
                    user.Friends[existingFriendIndex] = friend;
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        [UnitOfWork]
        protected virtual UserWithFriendsCacheItem GetUserFriendsCacheItemInternal(UserIdentifier userIdentifier)
        {
            var tenancyName = userIdentifier.TenantId.HasValue
                ? _tenantCache.GetOrNull(userIdentifier.TenantId.Value)?.TenancyName
                : null;

            using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
            {
                var friendships = _friendshipRepository.GetAll()
                    .Where(friendship => friendship.UserId == userIdentifier.UserId)
                    .ToList();

                var friendUserIds = friendships.Select(f => f.FriendUserId).Distinct().ToList();
                var friendUsers = _userRepository.GetAll()
                    .Where(u => friendUserIds.Contains(u.Id))
                    .ToDictionary(u => u.Id, u => new { u.Name, u.Surname, u.EmailAddress });

                var unreadCounts = _chatMessageRepository.GetAll()
                    .Where(cm => cm.ReadState == ChatMessageReadState.Unread &&
                                 cm.UserId == userIdentifier.UserId &&
                                 cm.TenantId == userIdentifier.TenantId &&
                                 cm.Side == ChatSide.Receiver &&
                                 friendUserIds.Contains(cm.TargetUserId))
                    .ToList()
                    .GroupBy(cm => new { cm.TargetUserId, cm.TargetTenantId })
                    .ToDictionary(g => (g.Key.TargetUserId, g.Key.TargetTenantId), g => g.Count());

                var friendCacheItems = friendships.Select(friendship =>
                {
                    var friendCacheItem = new FriendCacheItem
                    {
                        FriendUserId = friendship.FriendUserId,
                        FriendTenantId = friendship.FriendTenantId,
                        State = friendship.State,
                        FriendUserName = friendship.FriendUserName,
                        FriendTenancyName = friendship.FriendTenancyName,
                        FriendProfilePictureId = friendship.FriendProfilePictureId,
                        UnreadMessageCount = unreadCounts.TryGetValue((friendship.FriendUserId, friendship.FriendTenantId), out var count) ? count : 0
                    };

                    if (friendUsers.TryGetValue(friendship.FriendUserId, out var friendUser))
                    {
                        friendCacheItem.Name = friendUser.Name;
                        friendCacheItem.Surname = friendUser.Surname;
                        friendCacheItem.Email = friendUser.EmailAddress;
                    }

                    return friendCacheItem;
                }).ToList();

                var user = _userStore.FindById(userIdentifier.UserId.ToString(), default);

                return new UserWithFriendsCacheItem
                {
                    TenantId = userIdentifier.TenantId,
                    UserId = userIdentifier.UserId,
                    TenancyName = tenancyName,
                    UserName = user.UserName,
                    ProfilePictureId = user.ProfilePictureId,
                    Friends = friendCacheItems,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.EmailAddress
                };
            }
        }

        private void UpdateUserOnCache(UserIdentifier userIdentifier, UserWithFriendsCacheItem user)
        {
            _cacheManager.GetCache(FriendCacheItem.CacheName).Set(userIdentifier.ToUserIdentifierString(), user);
        }
    }
}