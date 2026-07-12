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
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ISingleClientProxy _clientProxy;
        private readonly IObjectMapper _objectMapper;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;

        public SignalRChatCommunicatorBddTests()
        {
            _chatHub = Substitute.For<IHubContext<ChatHub>>();
            _clientProxy = Substitute.For<ISingleClientProxy>();
            _objectMapper = Substitute.For<IObjectMapper>();
            _onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();

            _chatHub.Clients.Returns(new FakeHubClients(_clientProxy));
        }

        [Fact]
        public async Task Dado_UsuarioOnline_Quando_SendReadStateChangeToClients_Entao_DeveChamarSendAsync()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-1", 1);

            await communicator.SendReadStateChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1));

            await _clientProxy.Received(1).SendCoreAsync("getReadStateChange", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_UsuarioOffline_Quando_SendReadStateChangeToClients_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-2", 2);

            await communicator.SendReadStateChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1));

            await _clientProxy.DidNotReceive().SendCoreAsync("getReadStateChange", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_Mensagem_Quando_SendMessageToAll_Entao_DeveChamarSendAsyncParaCadaCliente()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-3", 1);
            _onlineClientManager.GetAllClientsAsync().Returns(new List<IOnlineClient> { onlineClient });
            _objectMapper.Map<ChatMessageDto>(Arg.Any<ChatMessage>()).Returns(new ChatMessageDto());

            await communicator.SendMessageToAll("hello");

            await _clientProxy.Received(1).SendCoreAsync("getMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_PedidoAmizade_Quando_SendFriendshipRequestToClient_Entao_DeveMapearEEnviar()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-4", 1);
            var friendship = CriarFriendship();
            _objectMapper.Map<FriendshipDto>(friendship).Returns(new FriendshipDto());

            await communicator.SendFriendshipRequestToClient(new List<IOnlineClient> { onlineClient }, friendship, true, false);

            await _clientProxy.Received(1).SendCoreAsync("getFriendshipRequest", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_MensagemChat_Quando_SendMessageToClient_Entao_DeveMapearEEnviar()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-5", 1);
            var message = CriarChatMessage();
            _objectMapper.Map<ChatMessageDto>(message).Returns(new ChatMessageDto());

            await communicator.SendMessageToClient(new List<IOnlineClient> { onlineClient }, message);

            await _clientProxy.Received(1).SendCoreAsync("getChatMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_Usuario_Quando_SendUserConnectionChangeToClients_Entao_DeveChamarSendAsync()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-6", 1);

            await communicator.SendUserConnectionChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1), true);

            await _clientProxy.Received(1).SendCoreAsync("getUserConnectNotification", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_Usuario_Quando_SendUserStateChangeToClients_Entao_DeveChamarSendAsync()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-7", 1);

            await communicator.SendUserStateChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1), FriendshipState.Accepted);

            await _clientProxy.Received(1).SendCoreAsync("getUserStateChange", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_UsuarioComMensagensNaoLidas_Quando_SendAllUnreadMessagesOfUserReadToClients_Entao_DeveChamarSendAsync()
        {
            var communicator = new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
            var onlineClient = CriarOnlineClient("conn-8", 1);

            await communicator.SendAllUnreadMessagesOfUserReadToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1));

            await _clientProxy.Received(1).SendCoreAsync("getallUnreadMessagesOfUserRead", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendAllUnreadMessagesOfUserReadToClients_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);

            await communicator.SendAllUnreadMessagesOfUserReadToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1));

            await _clientProxy.DidNotReceive().SendCoreAsync("getallUnreadMessagesOfUserRead", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendFriendshipRequestToClient_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);
            var friendship = CriarFriendship();

            await communicator.SendFriendshipRequestToClient(new List<IOnlineClient> { onlineClient }, friendship, true, false);

            await _clientProxy.DidNotReceive().SendCoreAsync("getFriendshipRequest", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendMessageToAll_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);
            _onlineClientManager.GetAllClientsAsync().Returns(new List<IOnlineClient> { onlineClient });

            await communicator.SendMessageToAll("hello");

            await _clientProxy.DidNotReceive().SendCoreAsync("getMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendMessageToClient_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);
            var message = CriarChatMessage();

            await communicator.SendMessageToClient(new List<IOnlineClient> { onlineClient }, message);

            await _clientProxy.DidNotReceive().SendCoreAsync("getChatMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendUserConnectionChangeToClients_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);

            await communicator.SendUserConnectionChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1), true);

            await _clientProxy.DidNotReceive().SendCoreAsync("getUserConnectNotification", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Dado_ClientesOffline_Quando_SendUserStateChangeToClients_Entao_NaoDeveChamarSendAsync()
        {
            var communicator = CriarComunicadorComClienteNulo();
            var onlineClient = CriarOnlineClient("conn-off", 1);

            await communicator.SendUserStateChangeToClients(new List<IOnlineClient> { onlineClient }, new UserIdentifier(null, 1), FriendshipState.Accepted);

            await _clientProxy.DidNotReceive().SendCoreAsync("getUserStateChange", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
        }

        private static IOnlineClient CriarOnlineClient(string connectionId, long userId)
        {
            var client = Substitute.For<IOnlineClient>();
            client.ConnectionId.Returns(connectionId);
            client.UserId.Returns(userId);
            return client;
        }

        private SignalRChatCommunicator CriarComunicadorComClienteNulo()
        {
            _chatHub.Clients.Returns(new FakeHubClients(null!));
            return new SignalRChatCommunicator(_objectMapper, _onlineClientManager, _chatHub);
        }

        private static Friendship CriarFriendship()
        {
            return (Friendship)Activator.CreateInstance(typeof(Friendship), true)!;
        }

        private static ChatMessage CriarChatMessage()
        {
            return (ChatMessage)Activator.CreateInstance(typeof(ChatMessage), true)!;
        }

        private class NullClientProxy : IClientProxy
        {
            public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }
        }

        private class FakeHubClients : IHubClients
        {
            private readonly IClientProxy _clientProxy;

            public FakeHubClients(IClientProxy clientProxy)
            {
                _clientProxy = clientProxy;
            }

            public IClientProxy All => _clientProxy;

            public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => _clientProxy;

            public IClientProxy Client(string connectionId) => _clientProxy;

            ISingleClientProxy IHubClients.Client(string connectionId) => _clientProxy as ISingleClientProxy;

            public IClientProxy Clients(IReadOnlyList<string> connectionIds) => _clientProxy;

            public IClientProxy Group(string groupName) => _clientProxy;

            public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => _clientProxy;

            public IClientProxy Groups(IReadOnlyList<string> groupNames) => _clientProxy;

            public IClientProxy User(string userId) => _clientProxy;

            public IClientProxy Users(IReadOnlyList<string> userIds) => _clientProxy;
        }
    }
}
