using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    public class AppConfigurationAccessorTests
    {
        [Fact]
        public void AppConfigurationAccessor_WithValidHostEnvironment_ShouldInitializeConfiguration()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Test");

            // Act
            var accessor = new AppConfigurationAccessor(hostEnvironment);

            // Assert
            accessor.ShouldNotBeNull();
            accessor.Configuration.ShouldNotBeNull();
            accessor.ShouldBeAssignableTo<IAppConfigurationAccessor>();
        }

        [Fact]
        public void AppConfigurationAccessor_Configuration_ShouldBeReadOnly()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            // Act
            var accessor = new AppConfigurationAccessor(hostEnvironment);
            var configuration = accessor.Configuration;

            // Assert
            configuration.ShouldNotBeNull();
            accessor.Configuration.ShouldBeSameAs(configuration); // Should return same instance
        }

        [Fact]
        public void AppConfigurationAccessor_WithDifferentEnvironments_ShouldWork()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var environments = new[] { "Development", "Production", "Staging", "Test" };

            foreach (var env in environments)
            {
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns(env);

                // Act & Assert
                Should.NotThrow(() => new AppConfigurationAccessor(hostEnvironment));
            }
        }
    }
}