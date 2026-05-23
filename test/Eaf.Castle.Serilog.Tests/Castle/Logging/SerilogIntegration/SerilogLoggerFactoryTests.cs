using Eaf.Castle.Logging.SerilogIntegration;
using Shouldly;
using Xunit;

namespace Eaf.Castle.Serilog.Tests.Castle.Logging.SerilogIntegration
{
    public class SerilogLoggerFactoryTests
    {
        [Fact]
        public void SerilogLoggerFactory_ShouldBeInstantiable()
        {
            // Arrange & Act
            var factory = new SerilogLoggerFactory();

            // Assert
            factory.ShouldNotBeNull();
            factory.ShouldBeOfType<SerilogLoggerFactory>();
        }
    }
}
