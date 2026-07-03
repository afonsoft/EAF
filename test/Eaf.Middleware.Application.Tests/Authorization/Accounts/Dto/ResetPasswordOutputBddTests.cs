using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class ResetPasswordOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ResetPasswordOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new ResetPasswordOutput();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}
