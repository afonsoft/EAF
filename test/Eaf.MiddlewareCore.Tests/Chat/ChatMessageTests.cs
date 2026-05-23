using Abp;
using Eaf.Middleware.Chat;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Chat
{
    public class ChatMessageTests
    {
        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarChatMessage_Entao_DeveDefinirPropriedades()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var targetUser = new UserIdentifier(2, 200);
            var sharedMessageId = Guid.NewGuid();
            var message = "Olá, mundo!";

            // Quando
            var chatMessage = new ChatMessage(
                user, targetUser, ChatSide.Sender, message,
                ChatMessageReadState.Unread, sharedMessageId,
                ChatMessageReadState.Unread);

            // Então
            chatMessage.UserId.ShouldBe(100);
            chatMessage.TenantId.ShouldBe(1);
            chatMessage.TargetUserId.ShouldBe(200);
            chatMessage.TargetTenantId.ShouldBe(2);
            chatMessage.Message.ShouldBe(message);
            chatMessage.Side.ShouldBe(ChatSide.Sender);
            chatMessage.ReadState.ShouldBe(ChatMessageReadState.Unread);
            chatMessage.SharedMessageId.ShouldBe(sharedMessageId);
            chatMessage.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            chatMessage.CreationTime.ShouldNotBe(default);
        }

        [Fact]
        public void Dado_ChatMessageExistente_Quando_ChangeReadState_Entao_DeveAtualizarEstado()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var targetUser = new UserIdentifier(2, 200);
            var chatMessage = new ChatMessage(
                user, targetUser, ChatSide.Sender, "teste",
                ChatMessageReadState.Unread, Guid.NewGuid(),
                ChatMessageReadState.Unread);

            // Quando
            chatMessage.ChangeReadState(ChatMessageReadState.Read);

            // Então
            chatMessage.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_ChatMessageExistente_Quando_ChangeReceiverReadState_Entao_DeveAtualizarEstado()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var targetUser = new UserIdentifier(2, 200);
            var chatMessage = new ChatMessage(
                user, targetUser, ChatSide.Sender, "teste",
                ChatMessageReadState.Unread, Guid.NewGuid(),
                ChatMessageReadState.Unread);

            // Quando
            chatMessage.ChangeReceiverReadState(ChatMessageReadState.Read);

            // Então
            chatMessage.ReceiverReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_MaxMessageLength_Quando_Verificar_Entao_DeveSer4096()
        {
            ChatMessage.MaxMessageLength.ShouldBe(4096);
        }

        [Fact]
        public void Dado_ChatSideEnum_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)ChatSide.Sender).ShouldBe(1);
            ((int)ChatSide.Receiver).ShouldBe(2);
        }

        [Fact]
        public void Dado_ChatMessageReadStateEnum_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)ChatMessageReadState.Unread).ShouldBe(1);
            ((int)ChatMessageReadState.Read).ShouldBe(2);
        }
    }
}
