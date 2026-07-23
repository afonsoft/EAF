using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para UserFriendsCache seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserFriendsCacheBddTests
    {
        private readonly ICacheManager _cacheManager;
        private readonly ICache _cache;
        private readonly IRepository<Friendship, long> _friendshipRepository;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ITenantCache _tenantCache;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserStore _userStore;
        private readonly UserFriendsCache _sut;

        public UserFriendsCacheBddTests()
        {
            _cacheManager = Substitute.For<ICacheManager>();
            _cache = Substitute.For<ICache>();
            _cacheManager.GetCache(FriendCacheItem.CacheName).Returns(_cache);

            _friendshipRepository = Substitute.For<IRepository<Friendship, long>>();
            _chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            _userRepository = Substitute.For<IRepository<User, long>>();
            _tenantCache = Substitute.For<ITenantCache>();
            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _userStore = Substitute.For<UserStore>(new object[10]);

            _sut = new UserFriendsCache(
                _cacheManager,
                _friendshipRepository,
                _chatMessageRepository,
                _userRepository,
                _tenantCache,
                _unitOfWorkManager,
                _userStore
            );
        }

        [Fact]
        public void Dado_TipoUserFriendsCache_Quando_Verificar_Entao_DeveImplementarIUserFriendsCache()
        {
            typeof(IUserFriendsCache).IsAssignableFrom(typeof(UserFriendsCache)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_TipoUserFriendsCache_Quando_Verificar_Entao_DeveSerSingletonDependency()
        {
            typeof(ISingletonDependency).IsAssignableFrom(typeof(UserFriendsCache)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_CacheContendoUsuario_Quando_GetCacheItem_Entao_DeveRetornarItemSemAcionarRepositorio()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var cacheItem = CriarCacheItem(userIdentifier);

            _cache.Get(cacheKey, Arg.Any<Func<string, object>>()).Returns(cacheItem);

            // Quando
            var result = _sut.GetCacheItem(userIdentifier);

            // Então
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(10);
            _friendshipRepository.DidNotReceive().GetAll();
        }

        [Fact]
        public void Dado_CacheContendoUsuario_Quando_GetCacheItemOrNull_Entao_DeveRetornarItem()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var cacheItem = CriarCacheItem(userIdentifier);

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            var result = _sut.GetCacheItemOrNull(userIdentifier);

            // Então
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(10);
        }

        [Fact]
        public void Dado_CacheSemUsuario_Quando_GetCacheItemOrNull_Entao_DeveRetornarNulo()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            _cache.GetOrDefault(cacheKey).Returns(null);

            // Quando
            var result = _sut.GetCacheItemOrNull(userIdentifier);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemAmigo_Quando_AddFriend_Entao_DeveAdicionarEAtualizarCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var cacheItem = CriarCacheItem(userIdentifier);
            var newFriend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20 };

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.AddFriend(userIdentifier, newFriend);

            // Então
            cacheItem.Friends.Count.ShouldBe(1);
            cacheItem.Friends.ContainsFriend(newFriend).ShouldBeTrue();
            _cache.Received(1).Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_AddFriendExistente_Entao_NaoDeveDuplicar()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var existingFriend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20 };
            var cacheItem = CriarCacheItem(userIdentifier, new List<FriendCacheItem> { existingFriend });

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.AddFriend(userIdentifier, existingFriend);

            // Então
            cacheItem.Friends.Count.ShouldBe(1);
            _cache.DidNotReceive().Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_RemoveFriend_Entao_DeveRemoverEAtualizarCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20 };
            var cacheItem = CriarCacheItem(userIdentifier, new List<FriendCacheItem> { friend });

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.RemoveFriend(userIdentifier, friend);

            // Então
            cacheItem.Friends.Count.ShouldBe(0);
            _cache.Received(1).Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_IncreaseUnreadMessageCount_Entao_DeveIncrementar()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var friendIdentifier = new UserIdentifier(1, 20);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20, UnreadMessageCount = 3 };
            var cacheItem = CriarCacheItem(userIdentifier, new List<FriendCacheItem> { friend });

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.IncreaseUnreadMessageCount(userIdentifier, friendIdentifier, 2);

            // Então
            friend.UnreadMessageCount.ShouldBe(5);
            _cache.Received(1).Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_ResetUnreadMessageCount_Entao_DeveZerar()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var friendIdentifier = new UserIdentifier(1, 20);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20, UnreadMessageCount = 5 };
            var cacheItem = CriarCacheItem(userIdentifier, new List<FriendCacheItem> { friend });

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.ResetUnreadMessageCount(userIdentifier, friendIdentifier);

            // Então
            friend.UnreadMessageCount.ShouldBe(0);
            _cache.Received(1).Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_UpdateFriend_Entao_DeveAtualizarCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 10);
            var cacheKey = userIdentifier.ToUserIdentifierString();
            var existingFriend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20, UnreadMessageCount = 0 };
            var updatedFriend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20, UnreadMessageCount = 9 };
            var cacheItem = CriarCacheItem(userIdentifier, new List<FriendCacheItem> { existingFriend });

            _cache.GetOrDefault(cacheKey).Returns(cacheItem);

            // Quando
            _sut.UpdateFriend(userIdentifier, updatedFriend);

            // Então
            cacheItem.Friends[0].UnreadMessageCount.ShouldBe(9);
            _cache.Received(1).Set(cacheKey, cacheItem, Arg.Any<TimeSpan?>(), Arg.Any<DateTimeOffset?>());
        }

        private static UserWithFriendsCacheItem CriarCacheItem(UserIdentifier userIdentifier, List<FriendCacheItem> friends = null)
        {
            return new UserWithFriendsCacheItem
            {
                TenantId = userIdentifier.TenantId,
                UserId = userIdentifier.UserId,
                UserName = "john",
                Friends = friends ?? new List<FriendCacheItem>()
            };
        }
    }
}
