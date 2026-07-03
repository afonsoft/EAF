using Eaf.Middleware.Editions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class FlatFeatureDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FlatFeatureDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDescription_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureDto();
            sut.Description = "test_value";
            sut.Description.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirParentName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureDto();
            sut.ParentName = "test_value";
            sut.ParentName.ShouldBe("test_value");
        }
    }
}
