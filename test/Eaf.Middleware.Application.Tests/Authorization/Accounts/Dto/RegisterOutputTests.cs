using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class RegisterOutputTests
    {
        [Fact]
        public void Dado_RegisterOutput_Quando_Criado_Entao_CanLoginDeveSerFalso()
        {
            var output = new RegisterOutput();
            output.CanLogin.ShouldBeFalse();
        }

        [Fact]
        public void Dado_RegisterOutput_Quando_AtribuirCanLogin_Entao_DeveRetornarValor()
        {
            var output = new RegisterOutput { CanLogin = true };
            output.CanLogin.ShouldBeTrue();
        }
    }
}
