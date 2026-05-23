using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests.Logging
{
    public class LogExtensionsTests
    {
        [Fact]
        public void LogExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(LogExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void LogExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(LogExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
