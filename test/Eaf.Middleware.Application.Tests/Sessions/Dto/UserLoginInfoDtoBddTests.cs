using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Sessions
{
    public class UserLoginInfoDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserLoginInfoDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAuthenticationSource_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.AuthenticationSource = "test_value";
            sut.AuthenticationSource.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEmailAddress_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.EmailAddress = "test_value";
            sut.EmailAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirProfilePictureId_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.ProfilePictureId = "test_value";
            sut.ProfilePictureId.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSurname_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.Surname = "test_value";
            sut.Surname.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new UserLoginInfoDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
