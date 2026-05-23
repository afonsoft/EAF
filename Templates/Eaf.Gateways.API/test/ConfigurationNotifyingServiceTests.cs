using Eaf.Gateways.API;
using Microsoft.Extensions.Logging;
using Moq;
using Ocelot.Configuration.ChangeTracking;
using Xunit;

namespace Eaf.Gateways.API.Tests
{
    public class ConfigurationNotifyingServiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange
            var mockTokenSource = new Mock<IOcelotConfigurationChangeTokenSource>();
            var mockLogger = new Mock<ILogger<ConfigurationNotifyingService>>();

            // Act
            var service = new ConfigurationNotifyingService(mockTokenSource.Object, mockLogger.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithNullTokenSource()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ConfigurationNotifyingService>>();

            // Act
            var service = new ConfigurationNotifyingService(null!, mockLogger.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithNullLogger()
        {
            // Arrange
            var mockTokenSource = new Mock<IOcelotConfigurationChangeTokenSource>();

            // Act
            var service = new ConfigurationNotifyingService(mockTokenSource.Object, null!);

            // Assert
            Assert.NotNull(service);
        }
    }
}
