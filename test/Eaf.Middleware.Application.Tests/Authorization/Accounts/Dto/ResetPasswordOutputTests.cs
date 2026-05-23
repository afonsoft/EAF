using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class ResetPasswordOutputTests
    {
        [Fact]
        public void Dado_ResetPasswordOutput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var output = new ResetPasswordOutput();
            output.CanLogin.ShouldBeFalse();
            output.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_ResetPasswordOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new ResetPasswordOutput
            {
                CanLogin = true,
                UserName = "admin"
            };

            output.CanLogin.ShouldBeTrue();
            output.UserName.ShouldBe("admin");
        }
    }
}
