using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class ActivateEmailInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ActivateEmailInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_Definirc_Entao_DeveArmazenar()
        {
            var sut = new ActivateEmailInput();
            sut.c = "test_value";
            sut.c.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirConfirmationCode_Entao_DeveArmazenar()
        {
            var sut = new ActivateEmailInput();
            sut.ConfirmationCode = "test_value";
            sut.ConfirmationCode.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new ActivateEmailInput();
            sut.UserId = 100L;
            sut.UserId.ShouldBe(100L);
        }
    }
}
