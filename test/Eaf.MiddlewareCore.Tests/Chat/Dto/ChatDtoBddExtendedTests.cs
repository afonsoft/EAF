using Abp;
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
            var pictureId = Guid.NewGuid();
            var dto = new ChatUserDto
            {
                Id = 200,
                UserName = "amigo",
                TenancyName = "tenant2",
                TenantId = 2,
                State = FriendshipState.Accepted,
                UnreadMessageCount = 5,
                IsOnline = true,
                ProfilePictureId = pictureId
            };

            dto.Id.ShouldBe(200);
            dto.UserName.ShouldBe("amigo");
            dto.TenancyName.ShouldBe("tenant2");
            dto.TenantId.ShouldBe(2);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.UnreadMessageCount.ShouldBe(5);
            dto.IsOnline.ShouldBeTrue();
            dto.ProfilePictureId.ShouldBe(pictureId);
        }

        #endregion

        #region ChatUserWithMessagesDto

        [Fact]
        public void Dado_ChatUserWithMessagesDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatUserWithMessagesDto
            {
                Id = 200,
                UserName = "amigo",
                Messages = new List<ChatMessageDto>
                {
                    new ChatMessageDto { Message = "Oi" },
                    new ChatMessageDto { Message = "Tudo bem?" }
                }
            };

            dto.Id.ShouldBe(200);
            dto.Messages.Count.ShouldBe(2);
        }

        #endregion

        #region ChatMessageExportDto

        [Fact]
        public void Dado_ChatMessageExportDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChatMessageExportDto
            {
                TargetUserId = 200,
                TargetUserName = "user2",
                TargetTenantId = 2,
                TargetTenantName = "tenant2",
                Message = "Teste",
                CreationTime = new DateTime(2026, 6, 1),
                Side = ChatSide.Sender,
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread
            };

            dto.TargetUserId.ShouldBe(200);
            dto.TargetUserName.ShouldBe("user2");
            dto.TargetTenantId.ShouldBe(2);
            dto.TargetTenantName.ShouldBe("tenant2");
            dto.Message.ShouldBe("Teste");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
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
                MinMessageId = 100,
                GroupId = 50
            };

            input.UserId.ShouldBe(200);
            input.TenantId.ShouldBe(1);
            input.MinMessageId.ShouldBe(100);
            input.GroupId.ShouldBe(50);
        }

        #endregion

        #region MarkAllUnreadMessagesOfUserAsReadInput

        [Fact]
        public void Dado_MarkAllUnreadMessagesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = 200,
                TenantId = 1,
                GroupId = 10
            };

            input.UserId.ShouldBe(200);
            input.TenantId.ShouldBe(1);
            input.GroupId.ShouldBe(10);
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesInput_Quando_ToUserIdentifier_Entao_DeveRetornarIdentificador()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                UserId = 200,
                TenantId = 1
            };

            var identifier = input.ToUserIdentifier();
            identifier.UserId.ShouldBe(200);
            identifier.TenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesInputComGroupId_Quando_ToUserIdentifier_Entao_DeveUsarGroupId()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                GroupId = 999,
                TenantId = 2
            };

            var identifier = input.ToUserIdentifier();
            identifier.UserId.ShouldBe(999);
            identifier.TenantId.ShouldBe(2);
        }

        #endregion

        #region GetUserChatFriendsWithSettingsOutput

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_CriarComConstrutorPadrao_Entao_DeveInicializarLista()
        {
            var output = new GetUserChatFriendsWithSettingsOutput();
            output.Friends.ShouldNotBeNull();
            output.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new GetUserChatFriendsWithSettingsOutput
            {
                Friends = new List<FriendshipDto>
                {
                    new FriendshipDto { FriendUserName = "amigo1" },
                    new FriendshipDto { FriendUserName = "amigo2" }
                },
                ServerTime = new DateTime(2026, 6, 1)
            };

            output.Friends.Count.ShouldBe(2);
            output.ServerTime.ShouldBe(new DateTime(2026, 6, 1));
        }

        #endregion

        #region FriendshipDto

        [Fact]
        public void Dado_FriendshipDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var pictureId = Guid.NewGuid();
            var dto = new FriendshipDto
            {
                FriendUserId = 200,
                FriendUserName = "amigo",
                FriendTenancyName = "tenant2",
                FriendTenantId = 2,
                FriendProfilePictureId = pictureId,
                State = FriendshipState.Accepted,
                UnreadMessageCount = 3,
                IsOnline = true,
                Name = "João",
                Surname = "Silva",
                Email = "joao@acme.com",
                GroupId = 10
            };

            dto.FriendUserId.ShouldBe(200);
            dto.FriendUserName.ShouldBe("amigo");
            dto.FriendTenancyName.ShouldBe("tenant2");
            dto.FriendTenantId.ShouldBe(2);
            dto.FriendProfilePictureId.ShouldBe(pictureId);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.UnreadMessageCount.ShouldBe(3);
            dto.IsOnline.ShouldBeTrue();
            dto.Name.ShouldBe("João");
            dto.Surname.ShouldBe("Silva");
            dto.Email.ShouldBe("joao@acme.com");
            dto.GroupId.ShouldBe(10);
        }

        #endregion
    }
}
