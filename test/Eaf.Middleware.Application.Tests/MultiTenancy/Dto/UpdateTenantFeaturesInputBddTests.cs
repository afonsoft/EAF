using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class UpdateTenantFeaturesInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UpdateTenantFeaturesInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new UpdateTenantFeaturesInput();
            sut.Id = 42;
            sut.Id.ShouldBe(42);
        }
    }
}
