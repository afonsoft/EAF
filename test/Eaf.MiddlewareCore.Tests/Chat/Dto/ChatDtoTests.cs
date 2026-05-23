using Abp;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Chat.Dto
{
    public class ChatDtoTests
    {
        [Fact]
        public void Dado_ChatMessageDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var now = DateTime.UtcNow;
            var dto = new ChatMessageDto
            {
                Id = 1,
                CreationTime = now,
                Message = "Olá",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread,
                SharedMessageId = "abc-123",
                Side = ChatSide.Sender,
                TargetTenantId = 2,
                TargetUserId = 200,
                TargetUserName = "targetUser",
                TenantId = 1,
                UserId = 100
            };

            dto.Id.ShouldBe(1);
            dto.CreationTime.ShouldBe(now);
            dto.Message.ShouldBe("Olá");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.SharedMessageId.ShouldBe("abc-123");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.TargetTenantId.ShouldBe(2);
            dto.TargetUserId.ShouldBe(200);
            dto.TargetUserName.ShouldBe("targetUser");
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_ChatMessageExportDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var now = DateTime.UtcNow;
            var dto = new ChatMessageExportDto
            {
                CreationTime = now,
                Message = "Exportação",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Read,
                Side = ChatSide.Receiver,
                TargetTenantId = 3,
                TargetTenantName = "TenantX",
                TargetUserId = 300,
                TargetUserName = "userX"
            };

            dto.CreationTime.ShouldBe(now);
            dto.Message.ShouldBe("Exportação");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Read);
            dto.Side.ShouldBe(ChatSide.Receiver);
            dto.TargetTenantId.ShouldBe(3);
            dto.TargetTenantName.ShouldBe("TenantX");
            dto.TargetUserId.ShouldBe(300);
            dto.TargetUserName.ShouldBe("userX");
        }

        [Fact]
        public void Dado_ChatUserDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var pictureId = Guid.NewGuid();
            var dto = new ChatUserDto
            {
                Id = 10,
                IsOnline = true,
                ProfilePictureId = pictureId,
                State = FriendshipState.Accepted,
                TenancyName = "TenantA",
                TenantId = 1,
                UnreadMessageCount = 5,
                UserName = "john"
            };

            dto.Id.ShouldBe(10);
            dto.IsOnline.ShouldBeTrue();
            dto.ProfilePictureId.ShouldBe(pictureId);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.TenancyName.ShouldBe("TenantA");
            dto.TenantId.ShouldBe(1);
            dto.UnreadMessageCount.ShouldBe(5);
            dto.UserName.ShouldBe("john");
        }

        [Fact]
        public void Dado_ChatUserWithMessagesDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var messages = new List<ChatMessageDto>
            {
                new ChatMessageDto { Message = "msg1" },
                new ChatMessageDto { Message = "msg2" }
            };

            var dto = new ChatUserWithMessagesDto
            {
                Id = 10,
                UserName = "user1",
                Messages = messages
            };

            dto.Messages.Count.ShouldBe(2);
            dto.Messages[0].Message.ShouldBe("msg1");
            dto.Messages[1].Message.ShouldBe("msg2");
        }

        [Fact]
        public void Dado_GetUserChatMessagesInput_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var input = new GetUserChatMessagesInput
            {
                MinMessageId = 50,
                TenantId = 1,
                UserId = 100,
                GroupId = 10
            };

            input.MinMessageId.ShouldBe(50);
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(100);
            input.GroupId.ShouldBe(10);
        }

        [Fact]
        public void Dado_GetUserChatMessagesInput_Quando_PropriedadesNull_Entao_DeveRetornarNull()
        {
            var input = new GetUserChatMessagesInput();

            input.MinMessageId.ShouldBeNull();
            input.TenantId.ShouldBeNull();
            input.UserId.ShouldBeNull();
            input.GroupId.ShouldBeNull();
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_Quando_ToUserIdentifier_Entao_DeveRetornarIdentifierCorreto()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                TenantId = 1,
                UserId = 100
            };

            var identifier = input.ToUserIdentifier();
            identifier.TenantId.ShouldBe(1);
            identifier.UserId.ShouldBe(100);
        }

        [Fact]
        public void Dado_MarkAllUnreadMessagesOfUserAsReadInput_ComGroupId_Quando_ToUserIdentifier_Entao_DeveUsarGroupId()
        {
            var input = new MarkAllUnreadMessagesOfUserAsReadInput
            {
                TenantId = 2,
                GroupId = 50
            };

            var identifier = input.ToUserIdentifier();
            identifier.TenantId.ShouldBe(2);
            identifier.UserId.ShouldBe(50);
        }

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_Instanciar_Entao_FriendsNaoDeveSerNull()
        {
            var output = new GetUserChatFriendsWithSettingsOutput();

            output.Friends.ShouldNotBeNull();
            output.Friends.Count.ShouldBe(0);
        }

        [Fact]
        public void Dado_GetUserChatFriendsWithSettingsOutput_Quando_DefinirServerTime_Entao_DeveRetornarCorreto()
        {
            var now = DateTime.UtcNow;
            var output = new GetUserChatFriendsWithSettingsOutput
            {
                ServerTime = now
            };

            output.ServerTime.ShouldBe(now);
        }
    }
}
