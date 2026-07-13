using Eaf.Hosting.Configuration;
using Eaf.KeyVault;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class EafKeyVaultConfigurationProviderTests_Fixed
    {
        private readonly EafKeyVaultOptions _options;

        public EafKeyVaultConfigurationProviderTests_Fixed()
        {
            _options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.None
            };
        }

        [Fact]
        public void Constructor_WithValidOptions_ShouldCreateProvider()
        {
            // Arrange & Act
            var provider = new EafKeyVaultConfigurationProvider(_options);

            // Assert
            provider.ShouldNotBeNull();
        }

        [Fact]
        public void Load_WithNoneProvider_ShouldNotLoadSecrets()
        {
            // Arrange
            var provider = new EafKeyVaultConfigurationProvider(_options);

            // Act
            provider.Load();

            // Assert
            provider.TryGet("test-key", out var value).ShouldBeFalse();
        }

        [Fact]
        public void Load_WithAzureProvider_ShouldHandleErrors()
        {
            // Arrange
            var azureOptions = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new System.Uri("https://test-vault.vault.azure.net/")
            };
            var provider = new EafKeyVaultConfigurationProvider(azureOptions);

            // Act & Assert - Should not throw
            Should.NotThrow(() => provider.Load());
        }

        [Fact]
        public void Load_WithOCIProvider_ShouldHandleErrors()
        {
            // Arrange
            var ociOptions = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.OCI,
                Endpoint = new System.Uri("https://test-vault.oci.com/")
            };
            var provider = new EafKeyVaultConfigurationProvider(ociOptions);

            // Act & Assert - Should not throw
            Should.NotThrow(() => provider.Load());
        }

        [Fact]
        public void Load_WithUnknownProvider_ShouldLoadEmptyDictionary()
        {
            // Arrange
            var unknownOptions = new EafKeyVaultOptions
            {
                Provider = (EnumKeyVault)99
            };
            var provider = new EafKeyVaultConfigurationProvider(unknownOptions);

            // Act
            Should.NotThrow(() => provider.Load());

            // Assert
            provider.TryGet("any-key", out var value).ShouldBeFalse();
        }
    }
}