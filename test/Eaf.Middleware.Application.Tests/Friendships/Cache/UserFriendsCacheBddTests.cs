#nullable disable

using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Friendships.Cache
{
    /// <summary>
    /// Testes BDD para UserFriendsCache seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserFriendsCacheBddTests
    {
        private readonly UserFriendsCache _sut;
        private readonly ICacheManager _cacheManager;
        private readonly ICache _friendCache;

        public UserFriendsCacheBddTests()
        {
            _cacheManager = Substitute.For<ICacheManager>();
            _friendCache = new AbpMemoryCache(FriendCacheItem.CacheName, new MemoryCacheOptions());
            _cacheManager.GetCache(FriendCacheItem.CacheName).Returns(_friendCache);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            unitOfWorkManager.Current.Returns(activeUow);

            _sut = new UserFriendsCache(
                _cacheManager,
                Substitute.For<IRepository<Friendship, long>>(),
                Substitute.For<IRepository<ChatMessage, long>>(),
                Substitute.For<ITenantCache>(),
                unitOfWorkManager,
                Substitute.For<UserStore>(new object[10])
            );
        }

        private static UserWithFriendsCacheItem CriarUsuarioEmCache()
        {
            return new UserWithFriendsCacheItem
            {
                UserId = 1,
                TenantId = 1,
                UserName = "user1",
                Friends = new List<FriendCacheItem>()
            };
        }

        [Fact]
        public void Dado_UsuarioSemAmigo_Quando_AddFriend_Entao_DeveAdicionarAmigoEAtualizarCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted };

            // Quando
            _sut.AddFriend(userIdentifier, friend);

            // Então
            var cached = _friendCache.AsTyped<string, UserWithFriendsCacheItem>().GetOrDefault(userIdentifier.ToUserIdentifierString());
            cached.ShouldNotBeNull();
            cached.Friends.Count.ShouldBe(1);
            cached.Friends[0].FriendUserId.ShouldBe(2);
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_UpdateFriend_Entao_DeveAtualizarDadosDoAmigoNoCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted, FriendUserName = "old" });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            var updatedFriend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Blocked, FriendUserName = "new" };

            // Quando
            _sut.UpdateFriend(userIdentifier, updatedFriend);

            // Então
            var cached = _friendCache.AsTyped<string, UserWithFriendsCacheItem>().GetOrDefault(userIdentifier.ToUserIdentifierString());
            cached.ShouldNotBeNull();
            cached.Friends[0].State.ShouldBe(FriendshipState.Blocked);
            cached.Friends[0].FriendUserName.ShouldBe("new");
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_IncreaseUnreadMessageCount_Entao_DeveIncrementarNaoLidas()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted, UnreadMessageCount = 0 });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            // Quando
            _sut.IncreaseUnreadMessageCount(userIdentifier, new UserIdentifier(2, 2), 1);

            // Então
            var cached = _friendCache.AsTyped<string, UserWithFriendsCacheItem>().GetOrDefault(userIdentifier.ToUserIdentifierString());
            cached.ShouldNotBeNull();
            cached.Friends[0].UnreadMessageCount.ShouldBe(1);
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_GetCacheItemOrNull_Entao_DeveRetornarNull()
        {
            // Dado/Quando
            var result = _sut.GetCacheItemOrNull(new UserIdentifier(1, 99));

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioEmCache_Quando_GetCacheItemOrNull_Entao_DeveRetornarUsuario()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            // Quando
            var result = _sut.GetCacheItemOrNull(userIdentifier);

            // Então
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(1);
        }
    }
}
