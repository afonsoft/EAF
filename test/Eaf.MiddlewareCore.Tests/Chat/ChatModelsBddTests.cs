using Abp;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Chat
{
    /// <summary>
    /// Testes BDD para modelos de Chat e Friendship seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatModelsBddTests
    {
        #region ChatMessage

        [Fact]
        public void Dado_ChatMessage_Quando_CriarComConstrutorCompleto_Entao_DeveDefinirPropriedades()
        {
            var sender = new UserIdentifier(1, 100);
            var receiver = new UserIdentifier(2, 200);
            var sharedId = Guid.NewGuid();

            var message = new ChatMessage(
                sender, receiver,
                ChatSide.Sender, "Olá!",
                ChatMessageReadState.Unread, sharedId,
                ChatMessageReadState.Unread);

            message.UserId.ShouldBe(100);
            message.TenantId.ShouldBe(1);
            message.TargetUserId.ShouldBe(200);
            message.TargetTenantId.ShouldBe(2);
            message.Message.ShouldBe("Olá!");
            message.Side.ShouldBe(ChatSide.Sender);
            message.ReadState.ShouldBe(ChatMessageReadState.Unread);
            message.SharedMessageId.ShouldBe(sharedId);
            message.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_CriarComLadoReceiver_Entao_DeveSerReceiver()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);

            var message = new ChatMessage(
                receiver, sender,
                ChatSide.Receiver, "Resposta",
                ChatMessageReadState.Read, Guid.NewGuid(),
                ChatMessageReadState.Read);

            message.Side.ShouldBe(ChatSide.Receiver);
            message.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_VerificarMaxMessageLength_Entao_DeveSer4KB()
        {
            ChatMessage.MaxMessageLength.ShouldBe(4 * 1024);
        }

        #endregion

        #region ChatEnums

        [Fact]
        public void Dado_ChatMessageReadState_Quando_VerificarValores_Entao_DevemEstarCorretos()
        {
            ((int)ChatMessageReadState.Unread).ShouldBe(1);
            ((int)ChatMessageReadState.Read).ShouldBe(2);
        }

        [Fact]
        public void Dado_ChatSide_Quando_VerificarValores_Entao_DevemEstarCorretos()
        {
            ((int)ChatSide.Sender).ShouldBe(1);
            ((int)ChatSide.Receiver).ShouldBe(2);
        }

        #endregion

        #region FriendshipState

        [Fact]
        public void Dado_FriendshipState_Quando_VerificarValores_Entao_DevemEstarCorretos()
        {
            ((int)FriendshipState.Accepted).ShouldBe(1);
            ((int)FriendshipState.Blocked).ShouldBe(2);
        }

        #endregion

        #region Friendship Entity

        [Fact]
        public void Dado_Friendship_Quando_CriarComParametrosValidos_Entao_DeveDefinirPropriedades()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            var pictureId = Guid.NewGuid();

            var friendship = new Friendship(user, friend, "acme", "maria", pictureId, FriendshipState.Accepted);

            friendship.UserId.ShouldBe(100);
            friendship.TenantId.ShouldBe(1);
            friendship.FriendUserId.ShouldBe(200);
            friendship.FriendTenantId.ShouldBe(2);
            friendship.FriendTenancyName.ShouldBe("acme");
            friendship.FriendUserName.ShouldBe("maria");
            friendship.FriendProfilePictureId.ShouldBe(pictureId);
            friendship.State.ShouldBe(FriendshipState.Accepted);
        }

        [Fact]
        public void Dado_Friendship_ComUserNulo_Quando_Criar_Entao_DeveLancarArgumentNullException()
        {
            var friend = new UserIdentifier(2, 200);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(null, friend, "acme", "maria", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_Friendship_ComFriendNulo_Quando_Criar_Entao_DeveLancarArgumentNullException()
        {
            var user = new UserIdentifier(1, 100);
            Should.Throw<ArgumentNullException>(() =>
                new Friendship(user, null, "acme", "maria", null, FriendshipState.Accepted));
        }

        [Fact]
        public void Dado_Friendship_ComStateInvalido_Quando_Criar_Entao_DeveLancarExcecao()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);
            Should.Throw<AbpException>(() =>
                new Friendship(user, friend, "acme", "maria", null, (FriendshipState)999));
        }

        [Fact]
        public void Dado_Friendship_ComStateBlocked_Quando_Criar_Entao_DeveSerBlocked()
        {
            var user = new UserIdentifier(1, 100);
            var friend = new UserIdentifier(2, 200);

            var friendship = new Friendship(user, friend, "acme", "maria", null, FriendshipState.Blocked);
            friendship.State.ShouldBe(FriendshipState.Blocked);
            friendship.FriendProfilePictureId.ShouldBeNull();
        }

        #endregion
    }
}
