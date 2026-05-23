using Abp.Zero.Configuration;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.Configuration
{
    public class EafMiddlewareAzureActiveDirectoryModuleConfigTests
    {
        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModuleConfig_ShouldImplementIEafMiddlewareAzureActiveDirectoryModuleConfig()
        {
            // Arrange & Act
            var type = typeof(EafMiddlewareAzureActiveDirectoryModuleConfig);

            // Assert
            type.ShouldNotBeNull();
            typeof(IEafMiddlewareAzureActiveDirectoryModuleConfig).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModuleConfig_ShouldBeInstantiable()
        {
            // Arrange
            var middlewareConfig = Substitute.For<IAbpZeroConfig>();

            // Act
            var config = new EafMiddlewareAzureActiveDirectoryModuleConfig(middlewareConfig);

            // Assert
            config.ShouldNotBeNull();
            config.ShouldBeOfType<EafMiddlewareAzureActiveDirectoryModuleConfig>();
        }
    }
}
