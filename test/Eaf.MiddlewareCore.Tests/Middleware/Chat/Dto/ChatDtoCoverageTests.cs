using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Friendships;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Chat.Dto
{
    public class ChatDtoCoverageTests
    {
        [Fact]
        public void ChatMessageDto_ShouldSetAll()
        {
            var dto = new ChatMessageDto
            {
                CreationTime = new DateTime(2024, 1, 1),
                Message = "hi",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread,
                SharedMessageId = "sid",
                Side = ChatSide.Sender,
                TargetTenantId = 2,
                TargetUserId = 10,
                TargetUserName = "t",
                TenantId = 1,
                UserId = 5
            };
            dto.CreationTime.ShouldBe(new DateTime(2024, 1, 1));
            dto.Message.ShouldBe("hi");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.SharedMessageId.ShouldBe("sid");
            dto.Side.ShouldBe(ChatSide.Sender);
            dto.TargetTenantId.ShouldBe(2);
            dto.TargetUserId.ShouldBe(10);
            dto.TargetUserName.ShouldBe("t");
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(5);
        }

        [Fact]
        public void ChatMessageExportDto_ShouldSetAll()
        {
            var dto = new ChatMessageExportDto
            {
                CreationTime = new DateTime(2024, 1, 1),
                Message = "m",
                ReadState = ChatMessageReadState.Read,
                ReceiverReadState = ChatMessageReadState.Unread,
                Side = ChatSide.Receiver,
                TargetTenantId = 1,
                TargetTenantName = "tn",
                TargetUserId = 5,
                TargetUserName = "u"
            };
            dto.CreationTime.ShouldBe(new DateTime(2024, 1, 1));
            dto.Message.ShouldBe("m");
            dto.ReadState.ShouldBe(ChatMessageReadState.Read);
            dto.ReceiverReadState.ShouldBe(ChatMessageReadState.Unread);
            dto.Side.ShouldBe(ChatSide.Receiver);
            dto.TargetTenantId.ShouldBe(1);
            dto.TargetTenantName.ShouldBe("tn");
            dto.TargetUserId.ShouldBe(5);
            dto.TargetUserName.ShouldBe("u");
        }

        [Fact]
        public void ChatUserDto_ShouldSet()
        {
            var pid = Guid.NewGuid();
            var dto = new ChatUserDto
            {
                IsOnline = true,
                ProfilePictureId = pid,
                State = FriendshipState.Accepted,
                TenancyName = "tn",
                TenantId = 1,
                UnreadMessageCount = 3,
                UserName = "u"
            };
            dto.IsOnline.ShouldBeTrue();
            dto.ProfilePictureId.ShouldBe(pid);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.TenancyName.ShouldBe("tn");
            dto.TenantId.ShouldBe(1);
            dto.UnreadMessageCount.ShouldBe(3);
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void ChatUserWithMessagesDto_ShouldSet()
        {
            var dto = new ChatUserWithMessagesDto
            {
                Messages = new List<ChatMessageDto>()
            };
            dto.Messages.ShouldNotBeNull();
        }

        [Fact]
        public void GetUserChatFriendsWithSettingsOutput_Defaults()
        {
            var dto = new GetUserChatFriendsWithSettingsOutput();
            dto.Friends.ShouldNotBeNull();
            dto.ServerTime = new DateTime(2024, 1, 1);
            dto.ServerTime.ShouldBe(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void GetUserChatMessagesInput_ShouldSet()
        {
            var dto = new GetUserChatMessagesInput { MinMessageId = 1, TenantId = 2, UserId = 3, GroupId = 4 };
            dto.MinMessageId.ShouldBe(1);
            dto.TenantId.ShouldBe(2);
            dto.UserId.ShouldBe(3);
            dto.GroupId.ShouldBe(4);
        }

        [Fact]
        public void MarkAllUnreadMessagesOfUserAsReadInput_ToUserIdentifier_UsesUserId()
        {
            var dto = new MarkAllUnreadMessagesOfUserAsReadInput { TenantId = 1, UserId = 5 };
            var ui = dto.ToUserIdentifier();
            ui.TenantId.ShouldBe(1);
            ui.UserId.ShouldBe(5);
        }

        [Fact]
        public void MarkAllUnreadMessagesOfUserAsReadInput_ToUserIdentifier_FallsBackToGroupId()
        {
            var dto = new MarkAllUnreadMessagesOfUserAsReadInput { TenantId = 1, UserId = null, GroupId = 7 };
            var ui = dto.ToUserIdentifier();
            ui.UserId.ShouldBe(7);
        }

        [Fact]
        public void ChatEnums_Values()
        {
            ((int)ChatMessageReadState.Unread).ShouldBe(1);
            ((int)ChatMessageReadState.Read).ShouldBe(2);
            ((int)ChatSide.Sender).ShouldBe(1);
            ((int)ChatSide.Receiver).ShouldBe(2);
        }
    }
}
