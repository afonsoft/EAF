using Eaf.AspNetCore.SignalR.Chat;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.SignalR.Chat
{
    public class SignalRChatDtoTests
    {
        [Fact]
        public void SendChatMessageInput_ShouldSetAll()
        {
            var pid = Guid.NewGuid();
            var dto = new SendChatMessageInput
            {
                Message = "m",
                ProfilePictureId = pid,
                TenancyName = "tn",
                TenantId = 1,
                UserId = 2,
                UserName = "u",
                GroupId = 3
            };
            dto.Message.ShouldBe("m");
            dto.ProfilePictureId.ShouldBe(pid);
            dto.TenancyName.ShouldBe("tn");
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(2);
            dto.UserName.ShouldBe("u");
            dto.GroupId.ShouldBe(3);
        }

        [Fact]
        public void SendFriendshipRequestInput_ShouldSet()
        {
            var dto = new SendFriendshipRequestInput { TenantId = 1, UserId = 5 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(5);
        }
    }
}
