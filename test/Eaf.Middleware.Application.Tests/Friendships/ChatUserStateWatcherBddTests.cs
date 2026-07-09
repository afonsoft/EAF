using Abp;
using Abp.RealTime;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para ChatUserStateWatcher seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatUserStateWatcherBddTests
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IUserFriendsCache _userFriendsCache;
        private readonly ChatUserStateWatcher _sut;

        public ChatUserStateWatcherBddTests()
        {
            _chatCommunicator = Substitute.For<IChatCommunicator>();
            _onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            _userFriendsCache = Substitute.For<IUserFriendsCache>();

            _sut = new ChatUserStateWatcher(
                _chatCommunicator,
                _userFriendsCache,
                _onlineClientManager
            );
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuarioConectado_Quando_Initialize_Entao_DeveNotificarAmigosConectados()
        {
            // Dado
            var user = new UserIdentifier(1, 42);
            var client = Substitute.For<IOnlineClient>();

            var friendOnline = new FriendCacheItem
            {
                FriendTenantId = 1,
                FriendUserId = 10
            };

            var friendOffline = new FriendCacheItem
            {
                FriendTenantId = 1,
                FriendUserId = 11
            };

            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem> { friendOnline, friendOffline }
            };

            _userFriendsCache.GetCacheItem(user).Returns(cacheItem);

            var onlineClient = Substitute.For<IOnlineClient>();
            _onlineClientManager
                .GetAllByUserIdAsync(new UserIdentifier(friendOnline.FriendTenantId, friendOnline.FriendUserId))
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient> { onlineClient }));

            _onlineClientManager
                .GetAllByUserIdAsync(new UserIdentifier(friendOffline.FriendTenantId, friendOffline.FriendUserId))
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient>()));

            _chatCommunicator
                .SendUserConnectionChangeToClients(
                    Arg.Any<IReadOnlyList<IOnlineClient>>(),
                    user,
                    Arg.Any<bool>())
                .Returns(Task.CompletedTask);

            _sut.Initialize();

            // Quando
            _onlineClientManager.UserConnected += Raise.Event<EventHandler<OnlineUserEventArgs>>(
                this,
                new OnlineUserEventArgs(user, client));

            // Então
            await _chatCommunicator
                .Received(1)
                .SendUserConnectionChangeToClients(
                    Arg.Is<IReadOnlyList<IOnlineClient>>(list => list.Count == 1),
                    user,
                    true);

            await _chatCommunicator
                .DidNotReceive()
                .SendUserConnectionChangeToClients(
                    Arg.Any<IReadOnlyList<IOnlineClient>>(),
                    user,
                    false);
        }

        [Fact]
        public async Task Dado_UsuarioDesconectado_Quando_Initialize_Entao_DeveNotificarAmigosDesconectados()
        {
            // Dado
            var user = new UserIdentifier(1, 42);
            var client = Substitute.For<IOnlineClient>();

            var friend = new FriendCacheItem
            {
                FriendTenantId = 1,
                FriendUserId = 10
            };

            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem> { friend }
            };

            _userFriendsCache.GetCacheItem(user).Returns(cacheItem);

            var onlineClient = Substitute.For<IOnlineClient>();
            _onlineClientManager
                .GetAllByUserIdAsync(new UserIdentifier(friend.FriendTenantId, friend.FriendUserId))
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient> { onlineClient }));

            _chatCommunicator
                .SendUserConnectionChangeToClients(
                    Arg.Any<IReadOnlyList<IOnlineClient>>(),
                    user,
                    Arg.Any<bool>())
                .Returns(Task.CompletedTask);

            _sut.Initialize();

            // Quando
            _onlineClientManager.UserDisconnected += Raise.Event<EventHandler<OnlineUserEventArgs>>(
                this,
                new OnlineUserEventArgs(user, client));

            // Então
            await _chatCommunicator
                .Received(1)
                .SendUserConnectionChangeToClients(
                    Arg.Is<IReadOnlyList<IOnlineClient>>(list => list.Count == 1),
                    user,
                    false);
        }
    }
}
