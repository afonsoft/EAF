using Abp.Dependency;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using Abp.RealTime;
using Abp.Threading;
using System.Linq;
using Abp;
using System.Threading.Tasks;

namespace Eaf.Middleware.Friendships
{
    /// <summary>
    /// Representa a classe ChatUserStateWatcher.
    /// </summary>
    public class ChatUserStateWatcher : ISingletonDependency
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IUserFriendsCache _userFriendsCache;

        /// <summary>
        /// ChatUserStateWatcher.
        /// </summary>
        /// <param name="chatCommunicator">Parâmetro chatCommunicator.</param>
        /// <param name="userFriendsCache">Parâmetro userFriendsCache.</param>
        /// <param name="onlineClientManager">Parâmetro onlineClientManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatUserStateWatcher(
            IChatCommunicator chatCommunicator,
            IUserFriendsCache userFriendsCache,
            IOnlineClientManager<ChatChannel> onlineClientManager)
        {
            _chatCommunicator = chatCommunicator;
            _userFriendsCache = userFriendsCache;
            _onlineClientManager = onlineClientManager;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _onlineClientManager.UserConnected += OnlineClientManager_UserConnected;
            _onlineClientManager.UserDisconnected += OnlineClientManager_UserDisconnected;
        }

        private async Task NotifyUserConnectionStateChange(UserIdentifier user, bool isConnected)
        {
            var cacheItem = _userFriendsCache.GetCacheItem(user);

            foreach (var friend in cacheItem.Friends)
            {
                var friendUserClients = await _onlineClientManager.GetAllByUserIdAsync(new UserIdentifier(friend.FriendTenantId, friend.FriendUserId));
                if (!friendUserClients.Any())
                {
                    continue;
                }

                AsyncHelper.RunSync(() => _chatCommunicator.SendUserConnectionChangeToClients(friendUserClients, user, isConnected));
            }
        }

        private void OnlineClientManager_UserConnected(object sender, OnlineUserEventArgs e)
        {
            NotifyUserConnectionStateChange(e.User, true).GetAwaiter();
        }

        private void OnlineClientManager_UserDisconnected(object sender, OnlineUserEventArgs e)
        {
            NotifyUserConnectionStateChange(e.User, false).GetAwaiter();
        }
    }
}