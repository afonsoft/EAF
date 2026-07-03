using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEmailAddress_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.EmailAddress = "test_value";
            sut.EmailAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.Id = 100L;
            sut.Id.ShouldBe(100L);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsLockoutEnabled_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.IsLockoutEnabled = true;
            sut.IsLockoutEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPassword_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.Password = "test_value";
            sut.Password.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirShouldChangePasswordOnNextLogin_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.ShouldChangePasswordOnNextLogin = true;
            sut.ShouldChangePasswordOnNextLogin.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSurname_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.Surname = "test_value";
            sut.Surname.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPhoneNumber_Entao_DeveArmazenar()
        {
            var sut = new UserEditDto();
            sut.PhoneNumber = "test_value";
            sut.PhoneNumber.ShouldBe("test_value");
        }
    }
}
