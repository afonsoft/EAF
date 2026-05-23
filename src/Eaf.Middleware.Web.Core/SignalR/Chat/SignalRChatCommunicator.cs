using Abp.Dependency;
using Abp.RealTime;
using Abp;
using Castle.Core.Logging;


using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Abp.ObjectMapping;


using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.AspNetCore.SignalR.Chat
{
    /// <summary>
    /// Representa a classe SignalRChatCommunicator.
    /// </summary>
    public class SignalRChatCommunicator : IChatCommunicator, ITransientDependency
    {
        private readonly IHubContext<ChatHub> _chatHub;

        private readonly IObjectMapper _objectMapper;

        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;

        /// <summary>
        /// SignalRChatCommunicator.
        /// </summary>
        /// <param name="objectMapper">Parâmetro objectMapper.</param>
        /// <param name="onlineClientManager">Parâmetro onlineClientManager.</param>
        /// <param name="chatHub">Parâmetro chatHub.</param>
        /// <returns>Resultado da operação.</returns>
        public SignalRChatCommunicator(
                    IObjectMapper objectMapper,
                    IOnlineClientManager<ChatChannel> onlineClientManager,
                    IHubContext<ChatHub> chatHub)
        {
            _objectMapper = objectMapper;
            _chatHub = chatHub;
            _onlineClientManager = onlineClientManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// SendAllUnreadMessagesOfUserReadToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        public async Task SendAllUnreadMessagesOfUserReadToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                await signalRClient.SendAsync("getallUnreadMessagesOfUserRead", user);
            }
        }

        /// <summary>
        /// SendFriendshipRequestToClient.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="friend">Parâmetro friend.</param>
        /// <param name="isOwnRequest">Parâmetro isOwnRequest.</param>
        /// <param name="isFriendOnline">Parâmetro isFriendOnline.</param>
        public async Task SendFriendshipRequestToClient(IReadOnlyList<IOnlineClient> clients, Friendship friend, bool isOwnRequest, bool isFriendOnline)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    return;
                }

                var friendshipRequest = _objectMapper.Map<FriendshipDto>(friend);
                friendshipRequest.IsOnline = isFriendOnline;

                await signalRClient.SendAsync("getFriendshipRequest", friendshipRequest, isOwnRequest);
            }
        }

        /// <summary>
        /// SendMessageToAll.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public async Task SendMessageToAll(string message)
        {
            var clients = await _onlineClientManager.GetAllClientsAsync();
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                await signalRClient.SendAsync("getMessage", message);
            }
        }

        /// <summary>
        /// SendMessageToClient.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="message">Parâmetro message.</param>
        public async Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    return;
                }

                await signalRClient.SendAsync("getChatMessage", _objectMapper.Map<ChatMessageDto>(message));
            }
        }

        /// <summary>
        /// SendReadStateChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        public async Task SendReadStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                await signalRClient.SendAsync("getReadStateChange", user);
            }
        }

        /// <summary>
        /// SendUserConnectionChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="isConnected">Parâmetro isConnected.</param>
        public async Task SendUserConnectionChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, bool isConnected)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                await signalRClient.SendAsync("getUserConnectNotification", user, isConnected);
            }
        }

        /// <summary>
        /// SendUserStateChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="newState">Parâmetro newState.</param>
        public async Task SendUserStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, FriendshipState newState)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                await signalRClient.SendAsync("getUserStateChange", user, newState);
            }
        }

        private IClientProxy GetSignalRClientOrNull(IOnlineClient client)
        {
            var signalRClient = _chatHub.Clients.Client(client.ConnectionId);
            if (signalRClient == null)
            {
                Logger.DebugFormat("Can not get chat user {0} from SignalR hub!", client.UserId);
                return null;
            }

            return signalRClient;
        }
    }
}