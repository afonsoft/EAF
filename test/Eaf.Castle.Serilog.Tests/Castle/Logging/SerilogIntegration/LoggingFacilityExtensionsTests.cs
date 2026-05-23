using Eaf.Castle.Logging.SerilogIntegration;
using Shouldly;
using Xunit;

namespace Eaf.Castle.Serilog.Tests.Castle.Logging.SerilogIntegration
{
    public class LoggingFacilityExtensionsTests
    {
        [Fact]
        public void LoggingFacilityExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(LoggingFacilityExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void LoggingFacilityExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(LoggingFacilityExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
