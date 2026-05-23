using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.Configuration
{
    public class ModuleMiddlewareAzureActiveDirectoryConfigurationExtensionsTests
    {
        [Fact]
        public void ModuleMiddlewareAzureActiveDirectoryConfigurationExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(ModuleMiddlewareAzureActiveDirectoryConfigurationExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void ModuleMiddlewareAzureActiveDirectoryConfigurationExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(ModuleMiddlewareAzureActiveDirectoryConfigurationExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
