using Eaf.Middleware.Editions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class FeatureInputTypeDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FeatureInputTypeDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new FeatureInputTypeDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }
    }
}
