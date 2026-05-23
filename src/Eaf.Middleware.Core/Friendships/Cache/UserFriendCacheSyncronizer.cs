using Abp;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.ObjectMapping;
using Eaf.Middleware.Chat;

namespace Eaf.Middleware.Friendships.Cache
{
    /// <summary>
    /// Representa a classe UserFriendCacheSyncronizer.
    /// </summary>
    public class UserFriendCacheSyncronizer :
        IEventHandler<EntityCreatedEventData<Friendship>>,
        IEventHandler<EntityDeletedEventData<Friendship>>,
        IEventHandler<EntityUpdatedEventData<Friendship>>,
        IEventHandler<EntityCreatedEventData<ChatMessage>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        private readonly IUserFriendsCache _userFriendsCache;

        /// <summary>
        /// UserFriendCacheSyncronizer.
        /// </summary>
        /// <param name="userFriendsCache">Parâmetro userFriendsCache.</param>
        /// <param name="objectMapper">Parâmetro objectMapper.</param>
        /// <returns>Resultado da operação.</returns>
        public UserFriendCacheSyncronizer(
            IUserFriendsCache userFriendsCache,
            IObjectMapper objectMapper)
        {
            _userFriendsCache = userFriendsCache;
            _objectMapper = objectMapper;
        }

        /// <summary>
        /// HandleEvent.
        /// </summary>
        /// <param name="eventData">Parâmetro eventData.</param>
        public void HandleEvent(EntityCreatedEventData<Friendship> eventData)
        {
            _userFriendsCache.AddFriend(
                eventData.Entity.ToUserIdentifier(),
                _objectMapper.Map<FriendCacheItem>(eventData.Entity)
                );
        }

        /// <summary>
        /// HandleEvent.
        /// </summary>
        /// <param name="eventData">Parâmetro eventData.</param>
        public void HandleEvent(EntityDeletedEventData<Friendship> eventData)
        {
            _userFriendsCache.RemoveFriend(
                eventData.Entity.ToUserIdentifier(),
                _objectMapper.Map<FriendCacheItem>(eventData.Entity)
            );
        }

        /// <summary>
        /// HandleEvent.
        /// </summary>
        /// <param name="eventData">Parâmetro eventData.</param>
        public void HandleEvent(EntityUpdatedEventData<Friendship> eventData)
        {
            var friendCacheItem = _objectMapper.Map<FriendCacheItem>(eventData.Entity);
            _userFriendsCache.UpdateFriend(eventData.Entity.ToUserIdentifier(), friendCacheItem);
        }

        /// <summary>
        /// HandleEvent.
        /// </summary>
        /// <param name="eventData">Parâmetro eventData.</param>
        public void HandleEvent(EntityCreatedEventData<ChatMessage> eventData)
        {
            var message = eventData.Entity;
            if (message.ReadState == ChatMessageReadState.Unread)
            {
                _userFriendsCache.IncreaseUnreadMessageCount(
                    new UserIdentifier(message.TenantId, message.UserId),
                    new UserIdentifier(message.TargetTenantId, message.TargetUserId),
                    1
                );
            }
        }
    }
}