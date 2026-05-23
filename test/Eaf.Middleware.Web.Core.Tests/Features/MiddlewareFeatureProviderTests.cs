using Eaf.Middleware.Web.Features;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Features
{
    public class MiddlewareFeatureProviderTests
    {
        [Fact]
        public void MiddlewareFeatureProvider_ShouldBeInstantiable()
        {
            // Arrange & Act
            var provider = new MiddlewareFeatureProvider();

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<MiddlewareFeatureProvider>();
        }
    }
}
