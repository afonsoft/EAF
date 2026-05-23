using Abp;
using Abp.RealTime;
using Eaf.Middleware.Friendships;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a interface IChatCommunicator.
    /// </summary>
    public interface IChatCommunicator
    {
        Task SendAllUnreadMessagesOfUserReadToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user);

        Task SendFriendshipRequestToClient(IReadOnlyList<IOnlineClient> clients, Friendship friend, bool isOwnRequest, bool isFriendOnline);

        Task SendMessageToAll(string message);

        Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message);

        Task SendReadStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user);

        Task SendUserConnectionChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, bool isConnected);

        Task SendUserStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, FriendshipState newState);
    }
}