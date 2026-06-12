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
        private readonly NullChatCommunicator _communicator = new();

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToAll_Entao_DeveCompletarSemExcecao()
        {
            await _communicator.SendMessageToAll("test message");
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendMessageToClient_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var message = new ChatMessage(user, target, ChatSide.Sender, "Olá", ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);
            await _communicator.SendMessageToClient(clients, message);
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendFriendshipRequestToClient_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            var probableFriend = new UserIdentifier(2, 200);
            var friend = new Friendship(user, probableFriend, "tenant2", "user2", null, FriendshipState.Accepted);
            await _communicator.SendFriendshipRequestToClient(clients, friend, true, false);
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserConnectionChangeToClients_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            await _communicator.SendUserConnectionChangeToClients(clients, user, true);
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendUserStateChangeToClients_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            await _communicator.SendUserStateChangeToClients(clients, user, FriendshipState.Accepted);
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendReadStateChangeToClients_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            await _communicator.SendReadStateChangeToClients(clients, user);
        }

        [Fact]
        public async Task Dado_NullChatCommunicator_Quando_SendAllUnreadMessagesOfUserReadToClients_Entao_DeveCompletarSemExcecao()
        {
            var clients = new List<IOnlineClient>();
            var user = new UserIdentifier(1, 100);
            await _communicator.SendAllUnreadMessagesOfUserReadToClients(clients, user);
        }
    }
}
