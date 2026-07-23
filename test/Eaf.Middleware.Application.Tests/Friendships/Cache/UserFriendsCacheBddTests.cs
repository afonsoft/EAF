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
using Eaf.Middleware.MultiTenancy;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IRepository<Friendship, long> _friendshipRepository;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ITenantCache _tenantCache;
        private readonly UserStore _userStore;

        public UserFriendsCacheBddTests()
        {
            _cacheManager = Substitute.For<ICacheManager>();
            _friendCache = new AbpMemoryCache(FriendCacheItem.CacheName, new MemoryCacheOptions());
            _cacheManager.GetCache(FriendCacheItem.CacheName).Returns(_friendCache);

            _friendshipRepository = Substitute.For<IRepository<Friendship, long>>();
            _chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            _tenantCache = Substitute.For<ITenantCache>();

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            unitOfWorkManager.Current.Returns(activeUow);

            _userStore = Substitute.For<UserStore>(new object[10]);
            _userRepository = Substitute.For<IRepository<User, long>>();

            _sut = new UserFriendsCache(
                _cacheManager,
                _friendshipRepository,
                _chatMessageRepository,
                _userRepository,
                _tenantCache,
                unitOfWorkManager,
                _userStore
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
        public void Dado_UsuarioComAmigo_Quando_RemoveFriend_Entao_DeveRemoverAmigoDoCache()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted };

            // Quando
            _sut.RemoveFriend(userIdentifier, friend);

            // Então
            var cached = _friendCache.AsTyped<string, UserWithFriendsCacheItem>().GetOrDefault(userIdentifier.ToUserIdentifierString());
            cached.ShouldNotBeNull();
            cached.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_UsuarioComAmigo_Quando_ResetUnreadMessageCount_Entao_DeveZerarNaoLidas()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, State = FriendshipState.Accepted, UnreadMessageCount = 5 });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            // Quando
            _sut.ResetUnreadMessageCount(userIdentifier, new UserIdentifier(2, 2));

            // Então
            var cached = _friendCache.AsTyped<string, UserWithFriendsCacheItem>().GetOrDefault(userIdentifier.ToUserIdentifierString());
            cached.ShouldNotBeNull();
            cached.Friends[0].UnreadMessageCount.ShouldBe(0);
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

        [Fact]
        public void Dado_UsuarioForaDoCacheComErroAoBuscarAmigo_Quando_GetCacheItem_Entao_DeveRetornarAmigoSemPreencherDetalhes()
        {
            var userIdentifier = new UserIdentifier(1, 1);
            var user = new User
            {
                Id = 1,
                UserName = "user1",
                Name = "User",
                Surname = "One",
                EmailAddress = "user1@example.com"
            };

            var friendship = new Friendship(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                "tenant2",
                "friend2",
                null,
                FriendshipState.Accepted
            );

            _friendshipRepository.GetAll().Returns(new List<Friendship> { friendship }.AsQueryable());
            _chatMessageRepository.GetAll().Returns(new List<ChatMessage>().AsQueryable());
            _tenantCache.GetOrNull(1).Returns(new TenantCacheItem { Id = 1, TenancyName = "Default" });
            _userStore.FindById("1", Arg.Any<CancellationToken>()).Returns(user);
            _userStore.When(x => x.FindById("2", Arg.Any<CancellationToken>())).Do(_ => throw new Exception("user not found"));

            var result = _sut.GetCacheItem(userIdentifier);

            result.ShouldNotBeNull();
            result.Friends.Count.ShouldBe(1);
            result.Friends[0].FriendUserId.ShouldBe(2);
            result.Friends[0].Name.ShouldBeNull();
            result.Friends[0].Surname.ShouldBeNull();
            result.Friends[0].Email.ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioForaDoCacheComAmigoDetalhado_Quando_GetCacheItem_Entao_DevePreencherDetalhesDoAmigo()
        {
            var userIdentifier = new UserIdentifier(1, 1);
            var user = new User
            {
                Id = 1,
                UserName = "user1",
                Name = "User",
                Surname = "One",
                EmailAddress = "user1@example.com"
            };

            var friendship = new Friendship(
                new UserIdentifier(1, 1),
                new UserIdentifier(2, 2),
                "tenant2",
                "friend2",
                null,
                FriendshipState.Accepted
            );

            var friendUser = new User
            {
                Id = 2,
                UserName = "friend2",
                Name = "Friend",
                Surname = "Two",
                EmailAddress = "friend2@example.com"
            };

            _friendshipRepository.GetAll().Returns(new List<Friendship> { friendship }.AsQueryable());
            _chatMessageRepository.GetAll().Returns(new List<ChatMessage>().AsQueryable());
            _tenantCache.GetOrNull(1).Returns(new TenantCacheItem { Id = 1, TenancyName = "Default" });
            _userStore.FindById("1", Arg.Any<CancellationToken>()).Returns(user);
            _userStore.FindById("2", Arg.Any<CancellationToken>()).Returns(friendUser);

            var result = _sut.GetCacheItem(userIdentifier);

            result.ShouldNotBeNull();
            result.Friends.Count.ShouldBe(1);
            result.Friends[0].Name.ShouldBe("Friend");
            result.Friends[0].Surname.ShouldBe("Two");
            result.Friends[0].Email.ShouldBe("friend2@example.com");
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_AddFriend_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 99);
            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2 };

            _sut.AddFriend(userIdentifier, friend);

            _friendCache.GetOrDefault(userIdentifier.ToUserIdentifierString()).ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_RemoveFriend_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 99);
            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2 };

            _sut.RemoveFriend(userIdentifier, friend);

            _friendCache.GetOrDefault(userIdentifier.ToUserIdentifierString()).ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_UpdateFriend_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 99);
            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2 };

            _sut.UpdateFriend(userIdentifier, friend);

            _friendCache.GetOrDefault(userIdentifier.ToUserIdentifierString()).ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_IncreaseUnreadMessageCount_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 99);
            var friendIdentifier = new UserIdentifier(2, 2);

            _sut.IncreaseUnreadMessageCount(userIdentifier, friendIdentifier, 1);

            _friendCache.GetOrDefault(userIdentifier.ToUserIdentifierString()).ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemAmigoCorrespondente_Quando_IncreaseUnreadMessageCount_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2 });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            _sut.IncreaseUnreadMessageCount(userIdentifier, new UserIdentifier(3, 3), 1);

            user.Friends[0].UnreadMessageCount.ShouldBe(0);
        }

        [Fact]
        public void Dado_UsuarioSemCache_Quando_ResetUnreadMessageCount_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 99);
            var friendIdentifier = new UserIdentifier(2, 2);

            _sut.ResetUnreadMessageCount(userIdentifier, friendIdentifier);

            _friendCache.GetOrDefault(userIdentifier.ToUserIdentifierString()).ShouldBeNull();
        }

        [Fact]
        public void Dado_UsuarioSemAmigoCorrespondente_Quando_ResetUnreadMessageCount_Entao_DeveRetornarSemErro()
        {
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            user.Friends.Add(new FriendCacheItem { FriendTenantId = 2, FriendUserId = 2, UnreadMessageCount = 5 });
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            _sut.ResetUnreadMessageCount(userIdentifier, new UserIdentifier(3, 3));

            user.Friends[0].UnreadMessageCount.ShouldBe(5);
        }

        [Fact]
        public void Dado_UsuarioEmCache_Quando_GetCacheItem_Entao_DeveRetornarUsuarioSemChamarRepositorio()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = CriarUsuarioEmCache();
            _friendCache.Set(userIdentifier.ToUserIdentifierString(), user);

            // Quando
            var result = _sut.GetCacheItem(userIdentifier);

            // Então
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(1);
            _friendshipRepository.Received(0).GetAll();
            _chatMessageRepository.Received(0).GetAll();
        }

        [Fact]
        public void Dado_UsuarioForaDoCache_Quando_GetCacheItem_Entao_DeveCarregarDoRepositorio()
        {
            // Dado
            var userIdentifier = new UserIdentifier(1, 1);
            var user = new User
            {
                Id = 1,
                UserName = "user1",
                Name = "User",
                Surname = "One",
                EmailAddress = "user1@example.com"
            };

            _friendshipRepository.GetAll().Returns(new List<Friendship>().AsQueryable());
            _chatMessageRepository.GetAll().Returns(new List<ChatMessage>().AsQueryable());
            _tenantCache.GetOrNull(1).Returns(new TenantCacheItem { Id = 1, TenancyName = "Default" });
            _userStore.FindById("1", Arg.Any<CancellationToken>()).Returns(user);

            // Quando
            var result = _sut.GetCacheItem(userIdentifier);

            // Então
            result.ShouldNotBeNull();
            result.UserId.ShouldBe(1);
            result.UserName.ShouldBe("user1");
            result.Friends.ShouldNotBeNull();
        }
    }
}
