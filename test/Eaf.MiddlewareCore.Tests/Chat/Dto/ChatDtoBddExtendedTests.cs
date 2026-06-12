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
    /// Testes BDD estendidos para DTOs de Chat seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatDtoBddExtendedTests
    {
        #region ChatMessageDto

        [Fact]
        public void Dado_ChatMessageDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatMessageDto
            {
                Id = 1,
                UserId = 100,
                TargetUserId = 200,
                TargetUserName = "user2",
                Message = "Olá!",
                CreationTime = new DateTime(2026, 6, 1),
                Side = ChatSide.Sender,
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread,
                SharedMessageId = "msg-123",
                TenantId = 1,
                TargetTenantId = 2
            };

            dto.Id.ShouldBe(1);
            dto.UserId.ShouldBe(100);
            dto.TargetUserId.ShouldBe(200);
            dto.TargetUserName.ShouldBe("user2");
            dto.Message.ShouldBe("Olá!");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.SharedMessageId.ShouldBe("msg-123");
            dto.TenantId.ShouldBe(1);
            dto.TargetTenantId.ShouldBe(2);
        }

        #endregion

        #region ChatUserDto

        [Fact]
        public void Dado_ChatUserDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatUserDto
            {
                FriendUserId = 200,
                FriendUserName = "amigo",
                FriendTenancyName = "tenant2",
                FriendTenantId = 2,
                State = FriendshipState.Accepted,
                UnreadMessageCount = 5,
                IsOnline = true,
                FriendProfilePictureId = Guid.NewGuid()
            };

            dto.FriendUserId.ShouldBe(200);
            dto.FriendUserName.ShouldBe("amigo");
            dto.FriendTenancyName.ShouldBe("tenant2");
            dto.FriendTenantId.ShouldBe(2);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.UnreadMessageCount.ShouldBe(5);
            dto.IsOnline.ShouldBeTrue();
            dto.FriendProfilePictureId.ShouldNotBeNull();
        }

        #endregion

        #region ChatUserWithMessagesDto

        [Fact]
        public void Dado_ChatUserWithMessagesDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatUserWithMessagesDto
            {
                FriendUserId = 200,
                Messages = new List<ChatMessageDto>
                {
                    new ChatMessageDto { Message = "Oi" },
                    new ChatMessageDto { Message = "Tudo bem?" }
                }
            };

            dto.FriendUserId.ShouldBe(200);
            dto.Messages.Count.ShouldBe(2);
        }

        #endregion

        #region ChatMessageExportDto

        [Fact]
        public void Dado_ChatMessageExportDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatMessageExportDto
            {
                From = "user1",
                To = "user2",
                Message = "Teste",
                ReadState = ChatMessageReadState.Read,
                CreationTime = new DateTime(2026, 6, 1)
            };

            dto.From.ShouldBe("user1");
            dto.To.ShouldBe("user2");
            dto.Message.ShouldBe("Teste");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
        }

        #endregion

        #region GetUserChatMessagesInput

        [Fact]
        public void Dado_GetUserChatMessagesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetUserChatMessagesInput
            {
                UserId = 200,
                TenantId = 1,
                MinMessageId = 100
            };

            input.UserId.ShouldBe(200);
            input.TenantId.ShouldBe(1);
            input.MinMessageId.ShouldBe(100);
        }

        #endregion

        #region MarkAllUnreadMessagesOfUserAsReadInput

        [Fact]
        public void Dado_MarkAllUnreadMessagesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = 200,
                TenantId = 1
            };

            input.UserId.ShouldBe(200);
            input.TenantId.ShouldBe(1);
        }

        #endregion

        #region GetUserChatFriendsWithSettingsOutput

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new GetUserChatFriendsWithSettingsOutput
            {
                Friends = new List<ChatUserDto>
                {
                    new ChatUserDto { FriendUserName = "amigo1" },
                    new ChatUserDto { FriendUserName = "amigo2" }
                },
                ServerTime = new DateTime(2026, 6, 1)
            };

            output.Friends.Count.ShouldBe(2);
            output.ServerTime.ShouldBe(new DateTime(2026, 6, 1));
        }

        #endregion
    }
}
