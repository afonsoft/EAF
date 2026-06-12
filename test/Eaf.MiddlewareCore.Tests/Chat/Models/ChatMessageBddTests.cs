using Abp;
using Eaf.Middleware.Chat;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Chat.Models
{
    /// <summary>
    /// Testes BDD para ChatMessage seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatMessageBddTests
    {
        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarChatMessage_Entao_DeveInicializarCorretamente()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(2, 200);
            var sharedId = Guid.NewGuid();

            // Quando
            var message = new ChatMessage(
                user, target, ChatSide.Sender, "Olá!",
                ChatMessageReadState.Unread, sharedId, ChatMessageReadState.Unread);

            // Então
            message.UserId.ShouldBe(100);
            message.TenantId.ShouldBe(1);
            message.TargetUserId.ShouldBe(200);
            message.TargetTenantId.ShouldBe(2);
            message.Side.ShouldBe(ChatSide.Sender);
            message.Message.ShouldBe("Olá!");
            message.ReadState.ShouldBe(ChatMessageReadState.Unread);
            message.SharedMessageId.ShouldBe(sharedId);
            message.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
        }

        [Fact]
        public void Dado_Mensagem_Quando_ChangeReadState_Entao_DeveAtualizarEstado()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var message = new ChatMessage(
                user, target, ChatSide.Sender, "Test",
                ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);

            // Quando
            message.ChangeReadState(ChatMessageReadState.Read);

            // Então
            message.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_Mensagem_Quando_ChangeReceiverReadState_Entao_DeveAtualizarEstado()
        {
            // Dado
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var message = new ChatMessage(
                user, target, ChatSide.Receiver, "Resposta",
                ChatMessageReadState.Read, Guid.NewGuid(), ChatMessageReadState.Unread);

            // Quando
            message.ChangeReceiverReadState(ChatMessageReadState.Read);

            // Então
            message.ReceiverReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_MaxMessageLength_Quando_Verificar_Entao_DeveSer4096()
        {
            ChatMessage.MaxMessageLength.ShouldBe(4096);
        }
    }
}
