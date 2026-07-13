using Abp;
using Abp.RealTime;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Chat
{
    /// <summary>
    /// Testes BDD para NullChatCommunicator seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class NullChatCommunicatorBddTests
    {
        private readonly NullChatCommunicator _communicator = new NullChatCommunicator();
        private readonly IReadOnlyList<IOnlineClient> _emptyClients = new List<IOnlineClient>();

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToClient_Entao_NaoDeveLancarExcecao()
        {
            var message = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(1, 2),
                ChatSide.Sender,
                "Olá",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            await Should.NotThrowAsync(async () => await _communicator.SendMessageToClient(_emptyClients, message));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToAll_Entao_NaoDeveLancarExcecao()
        {
            await Should.NotThrowAsync(async () => await _communicator.SendMessageToAll("Broadcast message"));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendFriendshipRequest_Entao_NaoDeveLancarExcecao()
        {
            var friend = new Friendship(new UserIdentifier(1, 100), new UserIdentifier(2, 200), "acme", "test-friend", null, FriendshipState.Accepted);
            await Should.NotThrowAsync(async () => await _communicator.SendFriendshipRequestToClient(_emptyClients, friend, true, false));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserStateChange_Entao_NaoDeveLancarExcecao()
        {
            var user = new UserIdentifier(1, 42);
            await Should.NotThrowAsync(async () => await _communicator.SendUserStateChangeToClients(_emptyClients, user, FriendshipState.Blocked));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserConnectionChange_Entao_NaoDeveLancarExcecao()
        {
            var user = new UserIdentifier(1, 42);
            await Should.NotThrowAsync(async () => await _communicator.SendUserConnectionChangeToClients(_emptyClients, user, true));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendReadStateChange_Entao_NaoDeveLancarExcecao()
        {
            var user = new UserIdentifier(1, 42);
            await Should.NotThrowAsync(async () => await _communicator.SendReadStateChangeToClients(_emptyClients, user));
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendAllUnreadMessages_Entao_NaoDeveLancarExcecao()
        {
            var user = new UserIdentifier(1, 42);
            await Should.NotThrowAsync(async () => await _communicator.SendAllUnreadMessagesOfUserReadToClients(_emptyClients, user));
        }
    }
}
