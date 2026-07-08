using Eaf.Middleware.Friendships.Cache;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships.Cache
{
    public class FriendCacheItemExtensionsBddTests
    {
        [Fact]
        public void Dado_ListaVazia_Quando_ContainsFriend_Entao_DeveRetornarFalso()
        {
            var items = new List<FriendCacheItem>();
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 };

            items.ContainsFriend(friend).ShouldBeFalse();
        }

        [Fact]
        public void Dado_ListaContendoAmigo_Quando_ContainsFriend_Entao_DeveRetornarVerdadeiro()
        {
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 };
            var items = new List<FriendCacheItem> { friend };

            items.ContainsFriend(friend).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ListaComOutroAmigo_Quando_ContainsFriend_Entao_DeveRetornarFalso()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 20 }
            };
            var friend = new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 };

            items.ContainsFriend(friend).ShouldBeFalse();
        }

        [Fact]
        public void Dado_AmigoComMesmoUsuarioMasTenantDiferente_Quando_ContainsFriend_Entao_DeveRetornarFalso()
        {
            var items = new List<FriendCacheItem>
            {
                new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10 }
            };
            var friend = new FriendCacheItem { FriendTenantId = 2, FriendUserId = 10 };

            items.ContainsFriend(friend).ShouldBeFalse();
        }
    }
}
