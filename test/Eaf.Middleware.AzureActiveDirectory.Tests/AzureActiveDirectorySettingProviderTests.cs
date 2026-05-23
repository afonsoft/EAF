using Abp.Configuration;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests
{
    public class AzureActiveDirectorySettingProviderTests
    {
        [Fact]
        public void AzureActiveDirectorySettingProvider_ShouldBeSettingProvider()
        {
            // Arrange & Act
            var type = typeof(AzureActiveDirectorySettingProvider);

            // Assert
            type.ShouldNotBeNull();
            typeof(SettingProvider).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void AzureActiveDirectorySettingProvider_ShouldBeInstantiable()
        {
            // Arrange & Act
            var provider = new AzureActiveDirectorySettingProvider();

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<AzureActiveDirectorySettingProvider>();
        }
    }
}
