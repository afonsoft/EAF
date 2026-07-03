using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class CreateOrUpdateUserInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateOrUpdateUserInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSendActivationEmail_Entao_DeveArmazenar()
        {
            var sut = new CreateOrUpdateUserInput();
            sut.SendActivationEmail = true;
            sut.SendActivationEmail.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSetRandomPassword_Entao_DeveArmazenar()
        {
            var sut = new CreateOrUpdateUserInput();
            sut.SetRandomPassword = true;
            sut.SetRandomPassword.ShouldBe(true);
        }
    }
}
