using Abp;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para Friendship seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class FriendshipBddTests
    {
        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarFriendship_Entao_DeveInicializarCorretamente()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(1, 200);
            var pictureId = Guid.NewGuid();

            // Quando
            var friendship = new Friendship(user, friend, "acme", "maria", pictureId, FriendshipState.Accepted);

            // Então
            friendship.UserId.ShouldBe(100);
            friendship.TenantId.ShouldBe(1);
            friendship.FriendUserId.ShouldBe(200);
            friendship.FriendTenantId.ShouldBe(1);
            friendship.FriendTenancyName.ShouldBe("acme");
            friendship.FriendUserName.ShouldBe("maria");
            friendship.FriendProfilePictureId.ShouldBe(pictureId);
            friendship.State.ShouldBe(FriendshipState.Accepted);
        }

        [Fact]
        public void Dado_UserNull_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            // Dado
            var friend = new UserIdentifier(1, 200);

            // Quando & Então
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(null, friend, "acme", "maria", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_FriendNull_Quando_CriarFriendship_Entao_DeveLancarArgumentNullException()
        {
            // Dado
            var user = new UserIdentifier(1, 100);

            // Quando & Então
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(user, null, "acme", "maria", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_EstadoInvalido_Quando_CriarFriendship_Entao_DeveLancarAbpException()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(1, 200);

            // Quando & Então
            Should.Throw<AbpException>(() =>
                new Friendship(user, friend, "acme", "maria", null, (FriendshipState)999));
        }

        [Fact]
        public void Dado_FriendshipState_Quando_VerificarValores_Entao_DevemEstarCorretos()
        {
            ((int)FriendshipState.Accepted).ShouldBe(1);
            ((int)FriendshipState.Blocked).ShouldBe(2);
        }

        [Fact]
        public void Dado_Friendship_Quando_AlterarEstado_Entao_DeveAceitar()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(1, 200);
            var friendship = new Friendship(user, friend, "acme", "maria", null, FriendshipState.Accepted);

            // Quando
            friendship.State = FriendshipState.Blocked;

            // Então
            friendship.State.ShouldBe(FriendshipState.Blocked);
        }
    }
}
