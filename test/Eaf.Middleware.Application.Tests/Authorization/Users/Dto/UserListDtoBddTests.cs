using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAuthenticationSource_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.AuthenticationSource = "test_value";
            sut.AuthenticationSource.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEmailAddress_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.EmailAddress = "test_value";
            sut.EmailAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEmailConfirmed_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.IsEmailConfirmed = true;
            sut.IsEmailConfirmed.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirLastLoginTime_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            var dt = System.DateTime.UtcNow; sut.LastLoginTime = dt;
            sut.LastLoginTime.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirProfilePictureId_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            var guid = System.Guid.NewGuid(); sut.ProfilePictureId = guid;
            sut.ProfilePictureId.ShouldBe(guid);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSurname_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.Surname = "test_value";
            sut.Surname.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new UserListDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
