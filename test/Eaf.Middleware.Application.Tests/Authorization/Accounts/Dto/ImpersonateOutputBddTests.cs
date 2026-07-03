using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class ImpersonateOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ImpersonateOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new ImpersonateOutput();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }
    }
}
