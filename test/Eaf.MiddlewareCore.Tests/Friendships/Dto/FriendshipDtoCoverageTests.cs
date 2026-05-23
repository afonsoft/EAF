using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Friendships.Dto
{
    public class FriendshipDtoCoverageTests
    {
        [Fact]
        public void Dado_FriendshipDto_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var pictureId = Guid.NewGuid();
            var dto = new FriendshipDto
            {
                FriendProfilePictureId = pictureId,
                FriendTenancyName = "tenant1",
                FriendTenantId = 1,
                FriendUserId = 42,
                FriendUserName = "john",
                IsOnline = true,
                State = FriendshipState.Accepted,
                UnreadMessageCount = 3,
                Name = "John",
                Surname = "Doe",
                Email = "john@test.com",
                GroupId = 10
            };

            dto.FriendProfilePictureId.ShouldBe(pictureId);
            dto.FriendTenancyName.ShouldBe("tenant1");
            dto.FriendTenantId.ShouldBe(1);
            dto.FriendUserId.ShouldBe(42);
            dto.FriendUserName.ShouldBe("john");
            dto.IsOnline.ShouldBeTrue();
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.UnreadMessageCount.ShouldBe(3);
            dto.Name.ShouldBe("John");
            dto.Surname.ShouldBe("Doe");
            dto.Email.ShouldBe("john@test.com");
            dto.GroupId.ShouldBe(10);
        }

        [Fact]
        public void Dado_CreateFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new CreateFriendshipRequestInput { TenantId = 1, UserId = 42 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_CreateFriendshipRequestByUserNameInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new CreateFriendshipRequestByUserNameInput { TenancyName = "default", UserName = "admin" };
            dto.TenancyName.ShouldBe("default");
            dto.UserName.ShouldBe("admin");
        }

        [Fact]
        public void Dado_AcceptFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new AcceptFriendshipRequestInput { TenantId = 1, UserId = 42 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_BlockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new BlockUserInput { TenantId = 1, UserId = 42 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_UnblockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UnblockUserInput { TenantId = 1, UserId = 42 };
            dto.TenantId.ShouldBe(1);
            dto.UserId.ShouldBe(42);
        }
    }
}
