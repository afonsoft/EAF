using Abp;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para a entidade Friendship seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class FriendshipEntityBddTests
    {
        [Fact]
        public void Dado_ConstrutorComParametros_Quando_Criar_Entao_DeveDefinirPropriedades()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            var pictureId = Guid.NewGuid();

            var friendship = new Friendship(user, friend, "tenant2", "amigo", pictureId, FriendshipState.Accepted);

            friendship.UserId.ShouldBe(100);
            friendship.TenantId.ShouldBe(1);
            friendship.FriendUserId.ShouldBe(200);
            friendship.FriendTenantId.ShouldBe(2);
            friendship.FriendTenancyName.ShouldBe("tenant2");
            friendship.FriendUserName.ShouldBe("amigo");
            friendship.FriendProfilePictureId.ShouldBe(pictureId);
            friendship.State.ShouldBe(FriendshipState.Accepted);
        }

        [Fact]
        public void Dado_UserNull_Quando_Criar_Entao_DeveLancarArgumentNullException()
        {
            var friend = new UserIdentifier(1, 200);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(null, friend, "tenant", "user", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_ProbableFriendNull_Quando_Criar_Entao_DeveLancarArgumentNullException()
        {
            var user = new UserIdentifier(1, 100);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(user, null, "tenant", "user", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_EstadoInvalido_Quando_Criar_Entao_DeveLancarAbpException()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(1, 200);
            Should.Throw<AbpException>(() =>
                new Friendship(user, friend, "tenant", "user", null, (FriendshipState)999));
        }

        [Fact]
        public void Dado_Friendship_Quando_CriarComTenantNull_Entao_TenantDeveSerNull()
        {
            var user = new UserIdentifier(null, 100);
            var friend = new UserIdentifier(null, 200);

            var friendship = new Friendship(user, friend, "host", "user2", null, FriendshipState.Accepted);

            friendship.TenantId.ShouldBeNull();
            friendship.FriendTenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_Friendship_Quando_Criar_Entao_CreationTimeDeveSerPreenchido()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(1, 200);

            var friendship = new Friendship(user, friend, "tenant", "user", null, FriendshipState.Blocked);

            friendship.CreationTime.ShouldNotBe(default(DateTime));
            friendship.State.ShouldBe(FriendshipState.Blocked);
        }

        [Fact]
        public void Dado_FriendshipState_Quando_VerificarValores_Entao_DeveEstarCorreto()
        {
            ((int)FriendshipState.Accepted).ShouldBe(1);
            ((int)FriendshipState.Blocked).ShouldBe(2);
        }
    }
}
