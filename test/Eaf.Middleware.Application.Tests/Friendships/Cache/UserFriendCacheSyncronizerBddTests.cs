using Abp;
using Abp.Events.Bus.Entities;
using Abp.ObjectMapping;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Friendships.Cache
{
    /// <summary>
    /// Testes BDD para UserFriendCacheSyncronizer seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserFriendCacheSyncronizerBddTests
    {
        private readonly UserFriendCacheSyncronizer _sut;
        private readonly IUserFriendsCache _userFriendsCache;
        private readonly IObjectMapper _objectMapper;

        public UserFriendCacheSyncronizerBddTests()
        {
            _userFriendsCache = Substitute.For<IUserFriendsCache>();
            _objectMapper = Substitute.For<IObjectMapper>();
            _sut = new UserFriendCacheSyncronizer(_userFriendsCache, _objectMapper);
        }

        private static Friendship CreateFriendship()
        {
            return new Friendship(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                "tenant2",
                "friend2",
                Guid.NewGuid(),
                FriendshipState.Accepted
            );
        }

        [Fact]
        public void Dado_AmizadeCriada_Quando_HandleEvent_Entao_DeveAdicionarAmigoNoCache()
        {
            // Dado
            var friendship = CreateFriendship();
            var friendCacheItem = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted };
            _objectMapper.Map<FriendCacheItem>(friendship).Returns(friendCacheItem);

            // Quando
            _sut.HandleEvent(new EntityCreatedEventData<Friendship>(friendship));

            // Então
            _userFriendsCache.Received(1).AddFriend(
                new UserIdentifier(1, 1),
                friendCacheItem
            );
        }

        [Fact]
        public void Dado_AmizadeExcluida_Quando_HandleEvent_Entao_DeveRemoverAmigoDoCache()
        {
            // Dado
            var friendship = CreateFriendship();
            var friendCacheItem = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted };
            _objectMapper.Map<FriendCacheItem>(friendship).Returns(friendCacheItem);

            // Quando
            _sut.HandleEvent(new EntityDeletedEventData<Friendship>(friendship));

            // Então
            _userFriendsCache.Received(1).RemoveFriend(
                new UserIdentifier(1, 1),
                friendCacheItem
            );
        }

        [Fact]
        public void Dado_AmizadeAtualizada_Quando_HandleEvent_Entao_DeveAtualizarAmigoNoCache()
        {
            // Dado
            var friendship = CreateFriendship();
            var friendCacheItem = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted };
            _objectMapper.Map<FriendCacheItem>(friendship).Returns(friendCacheItem);

            // Quando
            _sut.HandleEvent(new EntityUpdatedEventData<Friendship>(friendship));

            // Então
            _userFriendsCache.Received(1).UpdateFriend(
                new UserIdentifier(1, 1),
                friendCacheItem
            );
        }

        [Fact]
        public void Dado_MensagemNaoLida_Quando_HandleEvent_Entao_DeveIncrementarContadorNaoLidas()
        {
            // Dado
            var message = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                ChatSide.Receiver,
                "hello",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread
            );

            // Quando
            _sut.HandleEvent(new EntityCreatedEventData<ChatMessage>(message));

            // Então
            _userFriendsCache.Received(1).IncreaseUnreadMessageCount(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                1
            );
        }

        [Fact]
        public void Dado_MensagemJaLida_Quando_HandleEvent_Entao_NaoDeveIncrementarContadorNaoLidas()
        {
            // Dado
            var message = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                ChatSide.Receiver,
                "hello",
                ChatMessageReadState.Read,
                Guid.NewGuid(),
                ChatMessageReadState.Read
            );

            // Quando
            _sut.HandleEvent(new EntityCreatedEventData<ChatMessage>(message));

            // Então
            _userFriendsCache.DidNotReceive().IncreaseUnreadMessageCount(
                Arg.Any<UserIdentifier>(),
                Arg.Any<UserIdentifier>(),
                Arg.Any<int>()
            );
        }
    }
}
