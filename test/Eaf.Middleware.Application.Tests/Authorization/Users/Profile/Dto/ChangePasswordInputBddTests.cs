using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Profile
{
    public class ChangePasswordInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ChangePasswordInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirNewPassword_Entao_DeveArmazenar()
        {
            var sut = new ChangePasswordInput();
            sut.NewPassword = "test_value";
            sut.NewPassword.ShouldBe("test_value");
        }
    }
}
