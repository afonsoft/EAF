using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class IsTenantAvailableOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new IsTenantAvailableOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirServerRootAddress_Entao_DeveArmazenar()
        {
            var sut = new IsTenantAvailableOutput();
            sut.ServerRootAddress = "test_value";
            sut.ServerRootAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new IsTenantAvailableOutput();
            sut.TenantId = 42;
            sut.TenantId.ShouldBe(42);
        }
    }
}
