using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class GetTenantsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetTenantsInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new GetTenantsInput();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }
    }
}
