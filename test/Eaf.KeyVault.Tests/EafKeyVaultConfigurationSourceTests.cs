using Eaf.Hosting.Configuration;
using Eaf.KeyVault;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class EafKeyVaultConfigurationSourceTests_Fixed
    {
        [Fact]
        public void Constructor_WithValidOptions_ShouldCreateSource()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.None
            };

            // Act
            var source = new EafKeyVaultConfigurationSource(options);

            // Assert
            source.ShouldNotBeNull();
        }

        [Fact]
        public void Build_ShouldReturnConfigurationProvider()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new System.Uri("https://test-vault.vault.azure.net/")
            };
            var source = new EafKeyVaultConfigurationSource(options);
            var builder = new ConfigurationBuilder();

            // Act
            var provider = source.Build(builder);

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<EafKeyVaultConfigurationProvider>();
        }

        [Fact]
        public void Build_WithNullBuilder_ShouldReturnProvider()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.OCI
            };
            var source = new EafKeyVaultConfigurationSource(options);

            // Act
            var provider = source.Build(null);

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<EafKeyVaultConfigurationProvider>();
        }
    }
}