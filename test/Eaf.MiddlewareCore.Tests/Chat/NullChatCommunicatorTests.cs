using Abp;
using Abp.RealTime;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Chat
{
    public class NullChatCommunicatorTests
    {
        private readonly NullChatCommunicator _communicator;

        public NullChatCommunicatorTests()
        {
            _communicator = new NullChatCommunicator();
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToAll_Entao_NaoDeveLancarExcecao()
        {
            await Should.NotThrowAsync(async () => await _communicator.SendMessageToAll("test message"));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToClient_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(2, 200);
            var message = new ChatMessage(user, target, ChatSide.Sender, "teste",
                ChatMessageReadState.Unread, System.Guid.NewGuid(), ChatMessageReadState.Unread);

            await Should.NotThrowAsync(async () => await _communicator.SendMessageToClient(clients, message));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendAllUnreadMessagesOfUserReadToClients_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);

            await Should.NotThrowAsync(async () => await _communicator.SendAllUnreadMessagesOfUserReadToClients(clients, user));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendFriendshipRequestToClient_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            var friendship = new Friendship(user, friend, "tenant", "user", null, FriendshipState.Accepted);

            await Should.NotThrowAsync(async () => await _communicator.SendFriendshipRequestToClient(clients, friendship, true, false));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendReadStateChangeToClients_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);

            await Should.NotThrowAsync(async () => await _communicator.SendReadStateChangeToClients(clients, user));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserConnectionChangeToClients_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);

            await Should.NotThrowAsync(async () => await _communicator.SendUserConnectionChangeToClients(clients, user, true));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserStateChangeToClients_Entao_NaoDeveLancarExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);

            await Should.NotThrowAsync(async () => await _communicator.SendUserStateChangeToClients(clients, user, FriendshipState.Blocked));
        }
    }
}
