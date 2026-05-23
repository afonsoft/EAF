using Abp.Application.Features;
using Abp.Localization;
using Eaf.Middleware;
using Eaf.Middleware.Features;
using Moq;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware
{
    public class FeatureExtensions_Tests : EafMiddlewareTestBase
    {
        [Fact]
        public void GetValueText_WithNullValueTextNormalizer_ReturnsOriginalValue()
        {
            // Arrange
            var feature = new Feature("TestFeature", "Test Feature", scope: FeatureScopes.All);
            var value = "test_value";
            var localizationContextMock = new Mock<ILocalizationContext>();

            // Act
            var result = feature.GetValueText(value, localizationContextMock.Object);

            // Assert
            Assert.Equal("test_value", result);
        }

        [Fact]
        public void GetValueText_WithValueTextNormalizer_ReturnsLocalizedValue()
        {
            // Arrange
            var feature = new Feature("TestFeature", "Test Feature", scope: FeatureScopes.All);
            var value = "test_value";
            var localizationContextMock = new Mock<ILocalizationContext>();
            var localizedValue = "localized_test_value";

            var featureMetadata = new FeatureMetadata
            {
                ValueTextNormalizer = (val) => new FixedLocalizableString(localizedValue)
            };
            feature[FeatureMetadata.CustomFeatureKey] = featureMetadata;

            // Act
            var result = feature.GetValueText(value, localizationContextMock.Object);

            // Assert
            Assert.Equal(localizedValue, result);
        }
    }
}