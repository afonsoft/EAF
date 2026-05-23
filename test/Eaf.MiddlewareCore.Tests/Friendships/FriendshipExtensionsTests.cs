using Abp;
using Eaf.Middleware.Friendships;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Friendships
{
    public class FriendshipExtensionsTests
    {
        [Fact]
        public void Dado_Friendship_Quando_ChamarToFriendIdentifier_Entao_DeveRetornarIdentificadorDoAmigo()
        {
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "t2", "friendUser", null, FriendshipState.Accepted);

            var result = friendship.ToFriendIdentifier();

            result.TenantId.ShouldBe(2);
            result.UserId.ShouldBe(20);
        }

        [Fact]
        public void Dado_Friendship_Quando_ChamarToUserIdentifier_Entao_DeveRetornarIdentificadorDoUsuario()
        {
            var user = new UserIdentifier(1, 10);
            var friend = new UserIdentifier(2, 20);
            var friendship = new Friendship(user, friend, "t2", "friendUser", null, FriendshipState.Accepted);

            var result = friendship.ToUserIdentifier();

            result.TenantId.ShouldBe(1);
            result.UserId.ShouldBe(10);
        }
    }
}
