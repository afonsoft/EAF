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
    public class FriendshipDtoTests
    {
        #region FriendshipDto

        [Fact]
        public void Dado_FriendshipDto_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Dado & Quando
            var pictureId = Guid.NewGuid();
            var dto = new FriendshipDto
            {
                FriendProfilePictureId = pictureId,
                FriendTenancyName = "acme",
                FriendTenantId = 1,
                FriendUserId = 100,
                FriendUserName = "joao",
                IsOnline = true,
                State = FriendshipState.Accepted,
                UnreadMessageCount = 5,
                Name = "João",
                Surname = "Silva",
                Email = "joao@acme.com",
                GroupId = 10
            };

            // Então
            dto.FriendProfilePictureId.ShouldBe(pictureId);
            dto.FriendTenancyName.ShouldBe("acme");
            dto.FriendTenantId.ShouldBe(1);
            dto.FriendUserId.ShouldBe(100);
            dto.FriendUserName.ShouldBe("joao");
            dto.IsOnline.ShouldBeTrue();
            dto.State.ShouldBe(FriendshipState.Accepted);
            dto.UnreadMessageCount.ShouldBe(5);
            dto.Name.ShouldBe("João");
            dto.Surname.ShouldBe("Silva");
            dto.Email.ShouldBe("joao@acme.com");
            dto.GroupId.ShouldBe(10);
        }

        #endregion

        #region AcceptFriendshipRequestInput

        [Fact]
        public void Dado_AcceptFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new AcceptFriendshipRequestInput
            {
                TenantId = 1,
                UserId = 42
            };

            // Então
            input.TenantId.ShouldBe(1);
            input.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_AcceptFriendshipRequestInput_Quando_TenantIdNull_Entao_DeveAceitar()
        {
            // Dado & Quando
            var input = new AcceptFriendshipRequestInput { UserId = 1 };

            // Então
            input.TenantId.ShouldBeNull();
        }

        #endregion

        #region BlockUserInput

        [Fact]
        public void Dado_BlockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new BlockUserInput
            {
                TenantId = 2,
                UserId = 99
            };

            // Então
            input.TenantId.ShouldBe(2);
            input.UserId.ShouldBe(99);
        }

        #endregion

        #region CreateFriendshipRequestInput

        [Fact]
        public void Dado_CreateFriendshipRequestInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new CreateFriendshipRequestInput
            {
                TenantId = 3,
                UserId = 77
            };

            // Então
            input.TenantId.ShouldBe(3);
            input.UserId.ShouldBe(77);
        }

        #endregion

        #region CreateFriendshipRequestByUserNameInput

        [Fact]
        public void Dado_CreateByUserNameInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new CreateFriendshipRequestByUserNameInput
            {
                TenancyName = "acme",
                UserName = "maria"
            };

            // Então
            input.TenancyName.ShouldBe("acme");
            input.UserName.ShouldBe("maria");
        }

        #endregion

        #region UnblockUserInput

        [Fact]
        public void Dado_UnblockUserInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new UnblockUserInput
            {
                TenantId = 4,
                UserId = 55
            };

            // Então
            input.TenantId.ShouldBe(4);
            input.UserId.ShouldBe(55);
        }

        #endregion
    }
}
