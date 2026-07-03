using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class GetTenantFeaturesEditOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetTenantFeaturesEditOutput();
            sut.ShouldNotBeNull();
        }
    }
}
