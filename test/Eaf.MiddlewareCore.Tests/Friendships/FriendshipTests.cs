using Abp;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Friendships
{
    public class FriendshipTests
    {
        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarFriendship_Entao_DeveDefinirPropriedades()
        {
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var pictureId = Guid.NewGuid();

            var friendship = new Friendship(user, friend, "tenant2", "friendUser", pictureId, FriendshipState.Accepted);

            friendship.UserId.ShouldBe(10);
            friendship.TenantId.ShouldBe(1);
            friendship.FriendUserId.ShouldBe(20);
            friendship.FriendTenantId.ShouldBe(2);
            friendship.FriendTenancyName.ShouldBe("tenant2");
            friendship.FriendUserName.ShouldBe("friendUser");
            friendship.FriendProfilePictureId.ShouldBe(pictureId);
            friendship.State.ShouldBe(FriendshipState.Accepted);
            friendship.CreationTime.ShouldNotBe(default);
        }

        [Fact]
        public void Dado_UserNulo_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            var friend = new UserIdentifier(1, 2);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(null, friend, "t", "u", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_FriendNulo_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            var user = new UserIdentifier(1, 1);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(user, null, "t", "u", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_StateInvalido_Quando_CriarFriendship_Entao_DeveLancarAbpException()
        {
            var user = new UserIdentifier(1, 1);
            var friend = new UserIdentifier(1, 2);
            Should.Throw<AbpException>(() =>
                new Friendship(user, friend, "t", "u", null, (FriendshipState)999));
        }

        [Fact]
        public void Dado_FriendshipStateEnum_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)FriendshipState.Accepted).ShouldBe(1);
            ((int)FriendshipState.Blocked).ShouldBe(2);
        }
    }
}
