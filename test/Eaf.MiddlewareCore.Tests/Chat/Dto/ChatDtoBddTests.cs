using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
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
        public void Dado_ChatMessageDto_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Dado & Quando
            var now = DateTime.UtcNow;
            var dto = new ChatMessageDto
            {
                Id = 1,
                CreationTime = now,
                Message = "Olá!",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread,
                SharedMessageId = "msg-001",
                Side = ChatSide.Sender,
                TargetTenantId = 2,
                TargetUserId = 100,
                TargetUserName = "maria",
                TenantId = 1,
                UserId = 50
            };

            // Então
            dto.Id.ShouldBe(1);
            dto.CreationTime.ShouldBe(now);
            dto.Message.ShouldBe("Olá!");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.SharedMessageId.ShouldBe("msg-001");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.TargetTenantId.ShouldBe(2);
            dto.TargetUserId.ShouldBe(100);
            dto.TargetUserName.ShouldBe("maria");
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(50);
        }

        #endregion

        #region ChatMessageExportDto

        [Fact]
        public void Dado_ChatMessageExportDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new ChatMessageExportDto
            {
                CreationTime = new DateTime(2026, 1, 15),
                Message = "Exportação",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Read,
                Side = ChatSide.Receiver,
                TargetTenantId = 3,
                TargetTenantName = "acme",
                TargetUserId = 200,
                TargetUserName = "pedro"
            };

            // Então
            dto.CreationTime.ShouldBe(new DateTime(2026, 1, 15));
            dto.Message.ShouldBe("Exportação");
            dto.TargetTenantName.ShouldBe("acme");
            dto.TargetUserId.ShouldBe(200);
            dto.TargetUserName.ShouldBe("pedro");
        }

        #endregion

        #region ChatUserDto

        [Fact]
        public void Dado_ChatUserDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var pictureId = Guid.NewGuid();
            var dto = new ChatUserDto
            {
                Id = 10,
                IsOnline = true,
                ProfilePictureId = pictureId,
                State = FriendshipState.Accepted,
                TenancyName = "acme",
                TenantId = 1,
                UnreadMessageCount = 3,
                UserName = "ana"
            };

            // Então
            dto.Id.ShouldBe(10);
            dto.IsOnline.ShouldBeTrue();
            dto.ProfilePictureId.ShouldBe(pictureId);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.TenancyName.ShouldBe("acme");
            dto.TenantId.ShouldBe(1);
            dto.UnreadMessageCount.ShouldBe(3);
            dto.UserName.ShouldBe("ana");
        }

        #endregion

        #region ChatUserWithMessagesDto

        [Fact]
        public void Dado_ChatUserWithMessagesDto_Quando_AdicionarMensagens_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new ChatUserWithMessagesDto
            {
                Id = 5,
                UserName = "carlos",
                Messages = new List<ChatMessageDto>
                {
                    new ChatMessageDto { Message = "Msg1" },
                    new ChatMessageDto { Message = "Msg2" }
                }
            };

            // Então
            dto.Messages.ShouldNotBeNull();
            dto.Messages.Count.ShouldBe(2);
            dto.Messages[0].Message.ShouldBe("Msg1");
        }

        #endregion

        #region GetUserChatFriendsWithSettingsOutput

        [Fact]
        public void Dado_OutputPadrao_Quando_Criar_Entao_FriendsDeveSerListaVazia()
        {
            // Dado & Quando
            var output = new GetUserChatFriendsWithSettingsOutput();

            // Então
            output.Friends.ShouldNotBeNull();
            output.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_Output_Quando_DefinirServerTime_Entao_DeveArmazenar()
        {
            // Dado
            var output = new GetUserChatFriendsWithSettingsOutput();
            var now = DateTime.UtcNow;

            // Quando
            output.ServerTime = now;

            // Então
            output.ServerTime.ShouldBe(now);
        }

        #endregion

        #region GetUserChatMessagesInput

        [Fact]
        public void Dado_GetUserChatMessagesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new GetUserChatMessagesInput
            {
                TenantId = 1,
                UserId = 42,
                MinMessageId = 10
            };

            // Então
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(42);
            input.MinMessageId.ShouldBe(10);
        }

        #endregion

        #region MarkAllUnreadMessagesOfUserAsReadInput

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                TenantId = 2,
                UserId = 55
            };

            // Então
            input.TenantId.ShouldBe(2);
            input.UserId.ShouldBe(55);
        }

        #endregion
    }
}
