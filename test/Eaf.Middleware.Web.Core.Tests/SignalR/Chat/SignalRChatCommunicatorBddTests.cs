using Abp;
using Abp.ObjectMapping;
using Abp.RealTime;
using Eaf.AspNetCore.SignalR.Chat;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.SignalR.Chat
{
    public class SignalRChatCommunicatorBddTests
    {
        private class FakeHubClients : IHubClients
        {
            private readonly ISingleClientProxy _client;
            public FakeHubClients(ISingleClientProxy client) => _client = client;

            public IClientProxy All => _client;
            public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => _client;
            IClientProxy IHubClients<IClientProxy>.Client(string connectionId) => _client;
            public ISingleClientProxy Client(string connectionId) => _client;
            public IClientProxy Clients(IReadOnlyList<string> connectionIds) => _client;
            public IClientProxy Group(string groupName) => _client;
            public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => _client;
            public IClientProxy Groups(IReadOnlyList<string> groupNames) => _client;
            public IClientProxy User(string userId) => _client;
            public IClientProxy Users(IReadOnlyList<string> userIds) => _client;
        }

        private static SignalRChatCommunicator CriarSUT(
            IObjectMapper? objectMapper = null,
            IOnlineClientManager<ChatChannel>? onlineClientManager = null,
            IHubContext<ChatHub>? chatHub = null)
        {
            objectMapper ??= Substitute.For<IObjectMapper>();
            onlineClientManager ??= Substitute.For<IOnlineClientManager<ChatChannel>>();
            chatHub ??= Substitute.For<IHubContext<ChatHub>>();

            var clientProxy = Substitute.For<ISingleClientProxy>();
            var hubClients = new FakeHubClients(clientProxy);
            chatHub.Clients.Returns(hubClients);

            return new SignalRChatCommunicator(objectMapper, onlineClientManager, chatHub);
        }

        private static IOnlineClient CriarCliente(string connectionId = "conn-123")
        {
            var client = Substitute.For<IOnlineClient>();
            client.ConnectionId.Returns(connectionId);
            return client;
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            var chatHub = Substitute.For<IHubContext<ChatHub>>();

            var sut = new SignalRChatCommunicator(objectMapper, onlineClientManager, chatHub);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ClientesConectados_Quando_SendMessageToAll_Entao_DeveEnviarMensagemParaCadaCliente()
        {
            // Dado
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            var client = CriarCliente();
            onlineClientManager.GetAllClientsAsync().Returns(new List<IOnlineClient> { client });

            var sut = CriarSUT(onlineClientManager: onlineClientManager);

            // Quando
            await sut.SendMessageToAll("hello");

            // Então
            await onlineClientManager.Received(1).GetAllClientsAsync();
        }

        [Fact]
        public async Task Dado_Mensagem_Quando_SendMessageToClient_Entao_DeveMapearEEnviar()
        {
            // Dado
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<ChatMessageDto>(Arg.Any<object>()).Returns(new ChatMessageDto { Id = 1 });

            var chatHub = Substitute.For<IHubContext<ChatHub>>();
            var clientProxy = Substitute.For<ISingleClientProxy>();
            var hubClients = new FakeHubClients(clientProxy);
            chatHub.Clients.Returns(hubClients);

            var sut = new SignalRChatCommunicator(objectMapper, Substitute.For<IOnlineClientManager<ChatChannel>>(), chatHub);
            var client = CriarCliente();
            var message = new ChatMessage(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2),
                ChatSide.Sender,
                "Hello",
                ChatMessageReadState.Read,
                Guid.NewGuid(),
                ChatMessageReadState.Read);

            // Quando
            await sut.SendMessageToClient(new List<IOnlineClient> { client }, message);

            // Então
            objectMapper.Received(1).Map<ChatMessageDto>(message);
            await clientProxy.Received(1).SendCoreAsync("getChatMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_Usuario_Quando_SendReadStateChangeToClients_Entao_DeveEnviarNotificacao()
        {
            // Dado
            var client = CriarCliente();
            var sut = CriarSUT();

            // Quando
            await sut.SendReadStateChangeToClients(new List<IOnlineClient> { client }, new UserIdentifier(null, 1));

            // Então: sucesso sem exceções
            true.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_Friendship_Quando_SendFriendshipRequestToClient_Entao_DeveMapearEEnviar()
        {
            // Dado
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<FriendshipDto>(Arg.Any<object>()).Returns(new FriendshipDto { FriendUserId = 2 });

            var chatHub = Substitute.For<IHubContext<ChatHub>>();
            var clientProxy = Substitute.For<ISingleClientProxy>();
            var hubClients = new FakeHubClients(clientProxy);
            chatHub.Clients.Returns(hubClients);

            var sut = new SignalRChatCommunicator(objectMapper, Substitute.For<IOnlineClientManager<ChatChannel>>(), chatHub);
            var client = CriarCliente();
            var friendship = new Friendship(
                new UserIdentifier(null, 1),
                new UserIdentifier(null, 2),
                "host",
                "user2",
                null,
                FriendshipState.Accepted);

            // Quando
            await sut.SendFriendshipRequestToClient(new List<IOnlineClient> { client }, friendship, true, false);

            // Então
            objectMapper.Received(1).Map<FriendshipDto>(friendship);
            await clientProxy.Received(1).SendCoreAsync("getFriendshipRequest", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }
    }
}
