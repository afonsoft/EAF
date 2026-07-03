using Eaf.Middleware.Editions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class FlatFeatureSelectDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FlatFeatureSelectDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDescription_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureSelectDto();
            sut.Description = "test_value";
            sut.Description.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureSelectDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureSelectDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirParentName_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureSelectDto();
            sut.ParentName = "test_value";
            sut.ParentName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTextHtmlColor_Entao_DeveArmazenar()
        {
            var sut = new FlatFeatureSelectDto();
            sut.TextHtmlColor = "test_value";
            sut.TextHtmlColor.ShouldBe("test_value");
        }
    }
}
