using Eaf.AspNetCore.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.OpenTelemetry.Tests.AspNetCore.Configuration
{
    public class EafOpenTelemetryServiceCollectionExtensionsTests
    {
        [Fact]
        public void EafOpenTelemetryServiceCollectionExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(EafOpenTelemetryServiceCollectionExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void EafOpenTelemetryServiceCollectionExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(EafOpenTelemetryServiceCollectionExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
