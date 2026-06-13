using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Chat.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Chat seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatDtoBddTests
    {
        #region ChatMessageDto

        [Fact]
        public void Dado_ChatMessageDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var now = DateTime.UtcNow;
            var dto = new ChatMessageDto
            {
                Id = 1,
                UserId = 100,
                TenantId = 1,
                TargetUserId = 200,
                TargetTenantId = 2,
                TargetUserName = "maria",
                Message = "Olá, tudo bem?",
                Side = ChatSide.Sender,
                ReadState = ChatMessageReadState.Unread,
                ReceiverReadState = ChatMessageReadState.Unread,
                SharedMessageId = "msg-001",
                CreationTime = now
            };

            dto.Id.ShouldBe(1);
            dto.UserId.ShouldBe(100);
            dto.TenantId.ShouldBe(1);
            dto.TargetUserId.ShouldBe(200);
            dto.TargetTenantId.ShouldBe(2);
            dto.TargetUserName.ShouldBe("maria");
            dto.Message.ShouldBe("Olá, tudo bem?");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.ReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.SharedMessageId.ShouldBe("msg-001");
            dto.CreationTime.ShouldBe(now);
        }

        #endregion

        #region ChatMessageExportDto

        [Fact]
        public void Dado_ChatMessageExportDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatMessageExportDto
            {
                TargetUserId = 200,
                TargetUserName = "maria",
                TargetTenantId = 2,
                TargetTenantName = "Acme",
                Message = "Mensagem exportada",
                Side = ChatSide.Receiver,
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Read,
                CreationTime = new DateTime(2026, 1, 1)
            };

            dto.TargetUserId.ShouldBe(200);
            dto.TargetUserName.ShouldBe("maria");
            dto.TargetTenantName.ShouldBe("Acme");
            dto.Side.ShouldBe(ChatSide.Receiver);
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        #endregion

        #region ChatUserDto

        [Fact]
        public void Dado_ChatUserDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var pictureId = Guid.NewGuid();
            var dto = new ChatUserDto
            {
                Id = 42,
                UserName = "joao",
                TenancyName = "acme",
                TenantId = 1,
                ProfilePictureId = pictureId,
                State = FriendshipState.Accepted,
                IsOnline = true,
                UnreadMessageCount = 3
            };

            dto.Id.ShouldBe(42);
            dto.UserName.ShouldBe("joao");
            dto.TenancyName.ShouldBe("acme");
            dto.ProfilePictureId.ShouldBe(pictureId);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.IsOnline.ShouldBeTrue();
            dto.UnreadMessageCount.ShouldBe(3);
        }

        #endregion

        #region ChatUserWithMessagesDto

        [Fact]
        public void Dado_ChatUserWithMessagesDto_Quando_DefinirMensagens_Entao_DeveArmazenar()
        {
            var dto = new ChatUserWithMessagesDto
            {
                Id = 42,
                UserName = "joao",
                Messages = new List<ChatMessageDto>
                {
                    new ChatMessageDto { Id = 1, Message = "Oi" },
                    new ChatMessageDto { Id = 2, Message = "Tchau" }
                }
            };

            dto.Messages.Count.ShouldBe(2);
            dto.Messages[0].Message.ShouldBe("Oi");
        }

        #endregion

        #region GetUserChatFriendsWithSettingsOutput

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_CriarPadrao_Entao_FriendsDeveSerVazio()
        {
            var output = new GetUserChatFriendsWithSettingsOutput();
            output.Friends.ShouldNotBeNull();
            output.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_AdicionarAmigos_Entao_DeveConterAmigos()
        {
            var output = new GetUserChatFriendsWithSettingsOutput();
            output.Friends.Add(new FriendshipDto { FriendUserId = 1, FriendUserName = "ana" });
            output.Friends.Count.ShouldBe(1);
            output.ServerTime.ShouldBe(DateTime.MinValue);
        }

        #endregion

        #region GetUserChatMessagesInput

        [Fact]
        public void Dado_GetUserChatMessagesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetUserChatMessagesInput
            {
                UserId = 100,
                TenantId = 1,
                MinMessageId = 50
            };

            input.UserId.ShouldBe(100);
            input.TenantId.ShouldBe(1);
            input.MinMessageId.ShouldBe(50);
        }

        #endregion

        #region MarkAllUnreadMessagesOfUserAsReadInput

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = 200,
                TenantId = 2,
                GroupId = null
            };

            input.UserId.ShouldBe(200);
            input.TenantId.ShouldBe(2);
            input.GroupId.ShouldBeNull();
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_ComUserId_Quando_ToUserIdentifier_Entao_DeveRetornarIdentifier()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = 42,
                TenantId = 1
            };

            var identifier = input.ToUserIdentifier();
            identifier.UserId.ShouldBe(42);
            identifier.TenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_ComGroupId_Quando_ToUserIdentifier_Entao_DeveUsarGroupId()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = null,
                GroupId = 99,
                TenantId = 1
            };

            var identifier = input.ToUserIdentifier();
            identifier.UserId.ShouldBe(99);
        }

        #endregion
    }
}
