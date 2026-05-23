using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Friendships.Cache
{
    public class FriendCacheItemTests
    {
        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveSerEafUserFriendCache()
        {
            FriendCacheItem.CacheName.ShouldBe("EafUserFriendCache");
        }

        [Fact]
        public void Dado_FriendCacheItem_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var pictureId = Guid.NewGuid();
            var item = new FriendCacheItem
            {
                FriendProfilePictureId = pictureId,
                FriendTenancyName = "tenant1",
                FriendTenantId = 1,
                FriendUserId = 42,
                FriendUserName = "john",
                State = FriendshipState.Accepted,
                UnreadMessageCount = 5,
                Name = "John",
                Surname = "Doe",
                Email = "john@test.com"
            };

            item.FriendProfilePictureId.ShouldBe(pictureId);
            item.FriendTenancyName.ShouldBe("tenant1");
            item.FriendTenantId.ShouldBe(1);
            item.FriendUserId.ShouldBe(42);
            item.FriendUserName.ShouldBe("john");
            item.State.ShouldBe(FriendshipState.Accepted);
            item.UnreadMessageCount.ShouldBe(5);
            item.Name.ShouldBe("John");
            item.Surname.ShouldBe("Doe");
            item.Email.ShouldBe("john@test.com");
        }

        [Fact]
        public void Dado_ListaComAmigo_Quando_ChamarContainsFriend_Entao_DeveRetornarTrue()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 },
                new FriendCacheItem { FriendTenantId = 2, FriendUserId = 20 }
            };

            var searchItem = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 };
            items.ContainsFriend(searchItem).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ListaSemAmigo_Quando_ChamarContainsFriend_Entao_DeveRetornarFalse()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 }
            };

            var searchItem = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 99 };
            items.ContainsFriend(searchItem).ShouldBeFalse();
        }

        [Fact]
        public void Dado_ListaVazia_Quando_ChamarContainsFriend_Entao_DeveRetornarFalse()
        {
            var items = new List<FriendCacheItem>();
            var searchItem = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 };
            items.ContainsFriend(searchItem).ShouldBeFalse();
        }
    }
}
