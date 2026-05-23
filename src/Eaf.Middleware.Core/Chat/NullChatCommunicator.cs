using Abp;
using Abp.RealTime;
using Eaf.Middleware.Friendships;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a classe NullChatCommunicator.
    /// </summary>
    public class NullChatCommunicator : IChatCommunicator
    {
        /// <summary>
        /// SendAllUnreadMessagesOfUserReadToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        public async Task SendAllUnreadMessagesOfUserReadToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
        {
            await Task.CompletedTask;
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
            await Task.CompletedTask;
        }

        /// <summary>
        /// SendMessageToAll.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public async Task SendMessageToAll(string message)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SendMessageToClient.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="message">Parâmetro message.</param>
        public async Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SendReadStateChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        public async Task SendReadStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SendUserConnectionChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="isConnected">Parâmetro isConnected.</param>
        public async Task SendUserConnectionChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, bool isConnected)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// SendUserStateChangeToClients.
        /// </summary>
        /// <param name="clients">Parâmetro clients.</param>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="newState">Parâmetro newState.</param>
        public async Task SendUserStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, FriendshipState newState)
        {
            await Task.CompletedTask;
        }
    }
}