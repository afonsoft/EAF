using Abp;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    public class FriendshipExtensionsBddTests
    {
        #region FriendshipExtensions

        [Fact]
        public void Dado_Friendship_Quando_ToFriendIdentifier_Entao_DeveRetornarIdentifierDoAmigo()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            var friendship = new Friendship(user, friend, "acme", "joao", null, FriendshipState.Accepted);

            var result = friendship.ToFriendIdentifier();

            result.TenantId.ShouldBe(2);
            result.UserId.ShouldBe(200);
        }

        [Fact]
        public void Dado_Friendship_Quando_ToUserIdentifier_Entao_DeveRetornarIdentifierDoUsuario()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            var friendship = new Friendship(user, friend, "acme", "joao", null, FriendshipState.Accepted);

            var result = friendship.ToUserIdentifier();

            result.TenantId.ShouldBe(1);
            result.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_FriendshipSemTenant_Quando_ToFriendIdentifier_Entao_TenantIdDeveSerNull()
        {
            var user = new UserIdentifier(null, 100);
            var friend = new UserIdentifier(null, 200);
            var friendship = new Friendship(user, friend, null, "joao", null, FriendshipState.Accepted);

            var result = friendship.ToFriendIdentifier();

            result.TenantId.ShouldBeNull();
            result.UserId.ShouldBe(200);
        }

        #endregion

        #region FriendCacheItemExtensions

        [Fact]
        public void Dado_ListaComAmigo_Quando_ContainsFriend_Entao_DeveRetornarTrue()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 100 },
                new FriendCacheItem { FriendTenantId = 2, FriendUserId = 200 }
            };

            var search = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 100 };

            items.ContainsFriend(search).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ListaSemAmigo_Quando_ContainsFriend_Entao_DeveRetornarFalse()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 100 }
            };

            var search = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 999 };

            items.ContainsFriend(search).ShouldBeFalse();
        }

        [Fact]
        public void Dado_ListaVazia_Quando_ContainsFriend_Entao_DeveRetornarFalse()
        {
            var items = new List<FriendCacheItem>();
            var search = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 100 };

            items.ContainsFriend(search).ShouldBeFalse();
        }

        [Fact]
        public void Dado_FriendCacheItem_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var pictureId = Guid.NewGuid();
            var item = new FriendCacheItem
            {
                FriendProfilePictureId = pictureId,
                FriendTenancyName = "acme",
                FriendTenantId = 1,
                FriendUserId = 100,
                FriendUserName = "joao",
                State = FriendshipState.Accepted,
                UnreadMessageCount = 5,
                Name = "João",
                Surname = "Silva",
                Email = "joao@acme.com"
            };

            item.FriendProfilePictureId.ShouldBe(pictureId);
            item.FriendTenancyName.ShouldBe("acme");
            item.FriendTenantId.ShouldBe(1);
            item.FriendUserId.ShouldBe(100);
            item.FriendUserName.ShouldBe("joao");
            item.State.ShouldBe(FriendshipState.Accepted);
            item.UnreadMessageCount.ShouldBe(5);
            item.Name.ShouldBe("João");
            item.Surname.ShouldBe("Silva");
            item.Email.ShouldBe("joao@acme.com");
        }

        [Fact]
        public void Dado_FriendCacheItem_Quando_VerificarCacheName_Entao_DeveSerEafUserFriendCache()
        {
            FriendCacheItem.CacheName.ShouldBe("EafUserFriendCache");
        }

        #endregion

        #region Friendship Entity

        [Fact]
        public void Dado_Friendship_Quando_CriarComParametros_Entao_DeveDefinirPropriedades()
        {
            var pictureId = Guid.NewGuid();
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);

            var friendship = new Friendship(user, friend, "acme", "joao", pictureId, FriendshipState.Accepted);

            friendship.UserId.ShouldBe(100);
            friendship.TenantId.ShouldBe(1);
            friendship.FriendUserId.ShouldBe(200);
            friendship.FriendTenantId.ShouldBe(2);
            friendship.FriendTenancyName.ShouldBe("acme");
            friendship.FriendUserName.ShouldBe("joao");
            friendship.FriendProfilePictureId.ShouldBe(pictureId);
            friendship.State.ShouldBe(FriendshipState.Accepted);
        }

        [Fact]
        public void Dado_UserNull_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            var friend = new UserIdentifier(2, 200);

            Should.Throw<ArgumentNullException>(() =>
                new Friendship(null, friend, "acme", "joao", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_FriendNull_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            var user = new UserIdentifier(1, 100);

            Should.Throw<ArgumentNullException>(() =>
                new Friendship(user, null, "acme", "joao", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_FriendshipBlocked_Quando_CriarComBlocked_Entao_StateDeveSerBlocked()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);

            var friendship = new Friendship(user, friend, "acme", "joao", null, FriendshipState.Blocked);

            friendship.State.ShouldBe(FriendshipState.Blocked);
        }

        #endregion
    }
}
