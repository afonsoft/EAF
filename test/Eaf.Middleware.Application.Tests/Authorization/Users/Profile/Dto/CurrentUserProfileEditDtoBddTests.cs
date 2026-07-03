using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Profile
{
    public class CurrentUserProfileEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CurrentUserProfileEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new CurrentUserProfileEditDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSurname_Entao_DeveArmazenar()
        {
            var sut = new CurrentUserProfileEditDto();
            sut.Surname = "test_value";
            sut.Surname.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTimezone_Entao_DeveArmazenar()
        {
            var sut = new CurrentUserProfileEditDto();
            sut.Timezone = "test_value";
            sut.Timezone.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new CurrentUserProfileEditDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
