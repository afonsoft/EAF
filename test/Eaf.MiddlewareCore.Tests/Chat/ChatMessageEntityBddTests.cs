using Abp;
using Eaf.Middleware.Chat;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Chat
{
    /// <summary>
    /// Testes BDD para a entidade ChatMessage seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatMessageEntityBddTests
    {
        [Fact]
        public void Dado_ConstrutorComParametros_Quando_Criar_Entao_DeveDefinirPropriedades()
        {
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(2, 200);
            var sharedId = Guid.NewGuid();

            var msg = new ChatMessage(
                user, target, ChatSide.Sender, "Olá mundo",
                ChatMessageReadState.Unread, sharedId, ChatMessageReadState.Unread);

            msg.UserId.ShouldBe(100);
            msg.TenantId.ShouldBe(1);
            msg.TargetUserId.ShouldBe(200);
            msg.TargetTenantId.ShouldBe(2);
            msg.Message.ShouldBe("Olá mundo");
            msg.Side.ShouldBe(ChatSide.Sender);
            msg.ReadState.ShouldBe(ChatMessageReadState.Unread);
            msg.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            msg.SharedMessageId.ShouldBe(sharedId);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_ChangeReadState_Entao_DeveAlterarEstado()
        {
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var msg = new ChatMessage(
                user, target, ChatSide.Sender, "teste",
                ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);

            msg.ReadState.ShouldBe(ChatMessageReadState.Unread);

            msg.ChangeReadState(ChatMessageReadState.Read);
            msg.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_ChangeReceiverReadState_Entao_DeveAlterarEstado()
        {
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var msg = new ChatMessage(
                user, target, ChatSide.Sender, "teste",
                ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);

            msg.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);

            msg.ChangeReceiverReadState(ChatMessageReadState.Read);
            msg.ReceiverReadState.ShouldBe(ChatMessageReadState.Read);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_Criar_Entao_CreationTimeDeveSerPreenchido()
        {
            var user = new UserIdentifier(1, 100);
            var target = new UserIdentifier(1, 200);
            var msg = new ChatMessage(
                user, target, ChatSide.Receiver, "teste",
                ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);

            msg.CreationTime.ShouldNotBe(default(DateTime));
        }

        [Fact]
        public void Dado_MaxMessageLength_Quando_Verificar_Entao_DeveSer4096()
        {
            ChatMessage.MaxMessageLength.ShouldBe(4096);
        }

        [Fact]
        public void Dado_ChatMessage_Quando_CriarComTenantNull_Entao_TenantDeveSerNull()
        {
            var user = new UserIdentifier(null, 100);
            var target = new UserIdentifier(null, 200);
            var msg = new ChatMessage(
                user, target, ChatSide.Sender, "host msg",
                ChatMessageReadState.Unread, Guid.NewGuid(), ChatMessageReadState.Unread);

            msg.TenantId.ShouldBeNull();
            msg.TargetTenantId.ShouldBeNull();
        }
    }
}
