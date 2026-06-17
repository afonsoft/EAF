using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Friendship seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class FriendshipDtoBddTests
    {
        #region AcceptFriendshipRequestInput

        [Fact]
        public void Dado_AcceptFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new AcceptFriendshipRequestInput { TenantId = 1, UserId = 42 };
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_AcceptFriendshipRequestInput_SemTenantId_Quando_Verificar_Entao_DeveSerNull()
        {
            var input = new AcceptFriendshipRequestInput { UserId = 1 };
            input.TenantId.ShouldBeNull();
        }

        #endregion

        #region BlockUserInput

        [Fact]
        public void Dado_BlockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new BlockUserInput { TenantId = 2, UserId = 99 };
            input.TenantId.ShouldBe(2);
            input.UserId.ShouldBe(99);
        }

        #endregion

        #region UnblockUserInput

        [Fact]
        public void Dado_UnblockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new UnblockUserInput { TenantId = 3, UserId = 50 };
            input.TenantId.ShouldBe(3);
            input.UserId.ShouldBe(50);
        }

        #endregion

        #region CreateFriendshipRequestInput

        [Fact]
        public void Dado_CreateFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new CreateFriendshipRequestInput { TenantId = 1, UserId = 10 };
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(10);
        }

        #endregion

        #region CreateFriendshipRequestByUserNameInput

        [Fact]
        public void Dado_CreateFriendshipRequestByUserNameInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new CreateFriendshipRequestByUserNameInput
            {
                TenancyName = "acme",
                UserName = "john.doe"
            };

            input.TenancyName.ShouldBe("acme");
            input.UserName.ShouldBe("john.doe");
        }

        #endregion

        #region FriendshipDto

        [Fact]
        public void Dado_FriendshipDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var pictureId = Guid.NewGuid();
            var dto = new FriendshipDto
            {
                FriendUserId = 100,
                FriendUserName = "maria.silva",
                FriendTenantId = 1,
                FriendTenancyName = "acme",
                FriendProfilePictureId = pictureId,
                State = FriendshipState.Accepted,
                IsOnline = true,
                UnreadMessageCount = 5,
                Name = "Maria",
                Surname = "Silva",
                Email = "maria@acme.com",
                GroupId = 10
            };

            dto.FriendUserId.ShouldBe(100);
            dto.FriendUserName.ShouldBe("maria.silva");
            dto.FriendTenantId.ShouldBe(1);
            dto.FriendTenancyName.ShouldBe("acme");
            dto.FriendProfilePictureId.ShouldBe(pictureId);
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.IsOnline.ShouldBeTrue();
            dto.UnreadMessageCount.ShouldBe(5);
            dto.Name.ShouldBe("Maria");
            dto.Surname.ShouldBe("Silva");
            dto.Email.ShouldBe("maria@acme.com");
            dto.GroupId.ShouldBe(10);
        }

        [Fact]
        public void Dado_FriendshipDto_SemOptionals_Quando_Verificar_Entao_DevemSerNull()
        {
            var dto = new FriendshipDto { FriendUserId = 1 };
            dto.FriendTenantId.ShouldBeNull();
            dto.FriendProfilePictureId.ShouldBeNull();
            dto.GroupId.ShouldBeNull();
        }

        #endregion
    }
}
