using Eaf.Middleware.Friendships.Cache;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Friendships.Cache
{
    public class UserWithFriendsCacheItemTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var pictureId = Guid.NewGuid();
            var item = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>(),
                ProfilePictureId = pictureId,
                TenancyName = "default",
                TenantId = 1,
                UserId = 42,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                Email = "admin@test.com"
            };

            item.Friends.ShouldNotBeNull();
            item.Friends.Count.ShouldBe(0);
            item.ProfilePictureId.ShouldBe(pictureId);
            item.TenancyName.ShouldBe("default");
            item.TenantId.ShouldBe(1);
            item.UserId.ShouldBe(42);
            item.UserName.ShouldBe("admin");
            item.Name.ShouldBe("Admin");
            item.Surname.ShouldBe("User");
            item.Email.ShouldBe("admin@test.com");
        }

        [Fact]
        public void Dado_NullTenantId_Quando_Verificar_Entao_DeveSerNull()
        {
            var item = new UserWithFriendsCacheItem { TenantId = null };
            item.TenantId.ShouldBeNull();
        }
    }
}
