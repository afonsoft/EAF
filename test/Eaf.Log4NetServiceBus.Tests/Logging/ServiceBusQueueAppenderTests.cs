using Eaf.Log4NetServiceBus.Logging;
using Shouldly;
using Xunit;

namespace Eaf.Log4NetServiceBus.Tests.Logging
{
    public class ServiceBusQueueAppenderTests
    {
        [Fact]
        public void ServiceBusQueueAppender_ShouldBeInstantiable()
        {
            // Arrange & Act
            var appender = new ServiceBusQueueAppender();

            // Assert
            appender.ShouldNotBeNull();
            appender.ShouldBeOfType<ServiceBusQueueAppender>();
        }
    }
}
