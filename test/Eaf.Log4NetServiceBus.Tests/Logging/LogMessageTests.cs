using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests.Logging
{
    public class LogMessageTests
    {
        [Fact]
        public void LogMessage_ShouldBeInstantiable()
        {
            // Arrange & Act
            var logMessage = new LogMessage();

            // Assert
            logMessage.ShouldNotBeNull();
            logMessage.ShouldBeOfType<LogMessage>();
        }
    }
}
