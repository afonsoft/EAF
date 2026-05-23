using Eaf.Castle.Logging.SerilogIntegration;
using NSubstitute;
using Serilog;
using Shouldly;
using Xunit;

namespace Eaf.Castle.Serilog.Tests.Castle.Logging.SerilogIntegration
{
    public class SerilogLoggerTests
    {
        [Fact]
        public void SerilogLogger_ShouldBeInstantiable()
        {
            // Arrange
            var logger = Substitute.For<ILogger>();
            var factory = new SerilogLoggerFactory();

            // Act
            var serilogLogger = new SerilogLogger(logger, factory);

            // Assert
            serilogLogger.ShouldNotBeNull();
            serilogLogger.ShouldBeOfType<SerilogLogger>();
        }
    }
}
