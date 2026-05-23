using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class ImpersonateOutputTests
    {
        [Fact]
        public void Dado_ImpersonateOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new ImpersonateOutput();
            output.ImpersonationToken.ShouldBeNull();
            output.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_ImpersonateOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new ImpersonateOutput
            {
                ImpersonationToken = "token-123",
                TenancyName = "Default"
            };

            output.ImpersonationToken.ShouldBe("token-123");
            output.TenancyName.ShouldBe("Default");
        }
    }
}
