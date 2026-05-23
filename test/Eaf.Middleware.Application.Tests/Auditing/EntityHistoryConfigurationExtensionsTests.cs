using Eaf.Middleware.Auditing;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Auditing
{
    public class EntityHistoryConfigurationExtensionsTests
    {
        [Fact]
        public void EntityHistoryConfigurationExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(EntityHistoryConfigurationExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void EntityHistoryConfigurationExtensions_ShouldHaveExtensionMethods()
        {
            // Arrange & Act
            var type = typeof(EntityHistoryConfigurationExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
