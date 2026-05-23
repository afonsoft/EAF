using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Friendships.Dto
{
    public class FriendshipDtoCoverageTests
    {
        [Fact]
        public void CreateFriendshipRequestInput_ShouldSet()
        {
            var dto = new CreateFriendshipRequestInput { TenantId = 1, UserId = 5 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(5);
        }

        [Fact]
        public void BlockUserInput_ShouldSet()
        {
            var dto = new BlockUserInput { TenantId = 1, UserId = 2 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(2);
        }

        [Fact]
        public void FriendshipDto_ShouldSetAll()
        {
            var pid = Guid.NewGuid();
            var dto = new FriendshipDto
            {
                FriendProfilePictureId = pid,
                FriendTenancyName = "tn",
                FriendTenantId = 1,
                FriendUserId = 5,
                FriendUserName = "fu",
                IsOnline = true,
                State = FriendshipState.Blocked,
                UnreadMessageCount = 2,
                Name = "n",
                Surname = "s",
                Email = "e@b.com",
                GroupId = 10
            };
            dto.FriendProfilePictureId.ShouldBe(pid);
            dto.FriendTenancyName.ShouldBe("tn");
            dto.FriendTenantId.ShouldBe(1);
            dto.FriendUserId.ShouldBe(5);
            dto.FriendUserName.ShouldBe("fu");
            dto.IsOnline.ShouldBeTrue();
            dto.State.ShouldBe(FriendshipState.Blocked);
            dto.UnreadMessageCount.ShouldBe(2);
            dto.Name.ShouldBe("n");
            dto.Surname.ShouldBe("s");
            dto.Email.ShouldBe("e@b.com");
            dto.GroupId.ShouldBe(10);
        }

        [Fact]
        public void AcceptFriendshipRequestInput_ShouldSet()
        {
            var dto = new AcceptFriendshipRequestInput { TenantId = 1, UserId = 5 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(5);
        }

        [Fact]
        public void UnblockUserInput_ShouldSet()
        {
            var dto = new UnblockUserInput { TenantId = 1, UserId = 5 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(5);
        }

        [Fact]
        public void CreateFriendshipRequestByUserNameInput_ShouldSet()
        {
            var dto = new CreateFriendshipRequestByUserNameInput { TenancyName = "tn", UserName = "u" };
            dto.TenancyName.ShouldBe("tn");
            dto.UserName.ShouldBe("u");
        }
    }
}
