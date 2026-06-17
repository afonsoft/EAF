using Eaf.Middleware.Features;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Features
{
    public class FeatureMetadataBddTests
    {
        [Fact]
        public void Dado_FeatureMetadata_Quando_CriarNovo_Entao_TextHtmlColorDeveSerInherit()
        {
            var metadata = new FeatureMetadata();

            metadata.TextHtmlColor("qualquerValor").ShouldBe("inherit");
        }

        [Fact]
        public void Dado_FeatureMetadata_Quando_CriarNovo_Entao_IsVisibleOnPricingTableDeveSerFalse()
        {
            var metadata = new FeatureMetadata();

            metadata.IsVisibleOnPricingTable.ShouldBeFalse();
        }

        [Fact]
        public void Dado_FeatureMetadata_Quando_CriarNovo_Entao_ValueTextNormalizerDeveSerNull()
        {
            var metadata = new FeatureMetadata();

            metadata.ValueTextNormalizer.ShouldBeNull();
        }

        [Fact]
        public void Dado_FeatureMetadata_Quando_DefinirIsVisibleOnPricingTable_Entao_DevePersistir()
        {
            var metadata = new FeatureMetadata { IsVisibleOnPricingTable = true };

            metadata.IsVisibleOnPricingTable.ShouldBeTrue();
        }

        [Fact]
        public void Dado_FeatureMetadata_Quando_DefinirTextHtmlColor_Entao_DevePersistir()
        {
            var metadata = new FeatureMetadata
            {
                TextHtmlColor = value => value == "true" ? "green" : "red"
            };

            metadata.TextHtmlColor("true").ShouldBe("green");
            metadata.TextHtmlColor("false").ShouldBe("red");
        }

        [Fact]
        public void Dado_FeatureMetadata_Quando_VerificarCustomFeatureKey_Entao_DeveSerFeatureMetadata()
        {
            FeatureMetadata.CustomFeatureKey.ShouldBe("FeatureMetadata");
        }
    }
}
