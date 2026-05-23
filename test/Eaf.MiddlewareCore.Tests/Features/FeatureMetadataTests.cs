using Eaf.Middleware.Features;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Features
{
    public class FeatureMetadataTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveDefinirPadroes()
        {
            var metadata = new FeatureMetadata();
            metadata.IsVisibleOnPricingTable.ShouldBeFalse();
            metadata.TextHtmlColor.ShouldNotBeNull();
            metadata.ValueTextNormalizer.ShouldBeNull();
        }

        [Fact]
        public void Dado_TextHtmlColorPadrao_Quando_ChamarComValor_Entao_DeveRetornarInherit()
        {
            var metadata = new FeatureMetadata();
            metadata.TextHtmlColor("anything").ShouldBe("inherit");
        }

        [Fact]
        public void Dado_CustomFeatureKey_Quando_Verificar_Entao_DeveSerFeatureMetadata()
        {
            FeatureMetadata.CustomFeatureKey.ShouldBe("FeatureMetadata");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsVisibleOnPricingTable_Entao_DeveArmazenar()
        {
            var metadata = new FeatureMetadata { IsVisibleOnPricingTable = true };
            metadata.IsVisibleOnPricingTable.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTextHtmlColorCustom_Entao_DeveUsarFuncaoCustom()
        {
            var metadata = new FeatureMetadata
            {
                TextHtmlColor = value => value == "true" ? "green" : "red"
            };
            metadata.TextHtmlColor("true").ShouldBe("green");
            metadata.TextHtmlColor("false").ShouldBe("red");
        }
    }
}
