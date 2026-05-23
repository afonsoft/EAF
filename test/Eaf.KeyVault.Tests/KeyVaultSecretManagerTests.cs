using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class KeyVaultSecretManagerTests
    {
        [Fact]
        public void Constructor_WithNullOptions_ShouldCreateNullKeyVaultManager()
        {
            // Arrange & Act
            var manager = new KeyVaultSecretManager((EafKeyVaultOptions?)null);

            // Assert
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Constructor_WithOptionsWrapper_ShouldCreateManager()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var optionsWrapper = Options.Create(options);

            // Act
            var manager = new KeyVaultSecretManager(optionsWrapper);

            // Assert
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Constructor_WithAzureProvider_ShouldCreateAzureKeyVaultManager()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new Uri("https://test.vault.azure.net/"),
                Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "test-client-id",
                    ClientSecret = "test-client-secret",
                    TenantId = "test-tenant-id"
                }
            };

            // Act & Assert
            Should.NotThrow(() => new KeyVaultSecretManager(options));
        }

        [Fact(Skip = "Requires OCI configuration files")]
        public void Constructor_WithOCIProvider_ShouldCreateOCIKeyVaultManager()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.OCI,
                Endpoint = new Uri("https://test.oci.vault.com/"),
                Oci = new OciKeyVaultOptions
                {
                    SecretId = "test-secret-id",
                    VaultId = "test-vault-id",
                    TenantId = "test-tenant-id",
                    UserId = "test-user-id",
                    Fingerprint = "test-fingerprint",
                    Region = "us-ashburn-1"
                }
            };

            // Act & Assert
            Should.NotThrow(() => new KeyVaultSecretManager(options));
        }

        [Fact]
        public void Constructor_WithNoneProvider_ShouldCreateNullKeyVaultManager()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };

            // Act
            var manager = new KeyVaultSecretManager(options);

            // Assert
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void GetKeyValues_WithNullProvider_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result = manager.GetKeyValues();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetKeyValuesAsync_WithNullProvider_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result = await manager.GetKeyValuesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GetValue_WithNullProvider_ShouldReturnNull()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result = manager.GetValue("test-key");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetValueAsync_WithNullProvider_ShouldReturnNull()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result = await manager.GetValueAsync("test-key");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void SetValue_WithNullProvider_ShouldNotThrow()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act & Assert
            Should.NotThrow(() => manager.SetValue("test-key", "test-value"));
        }

        [Fact]
        public async Task SetValueAsync_WithNullProvider_ShouldNotThrow()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act & Assert
            await Should.NotThrowAsync(async () => await manager.SetValueAsync("test-key", "test-value"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("test-key")]
        [InlineData("very-long-key-name-with-special-characters-123")]
        public void GetValue_WithDifferentKeys_ShouldHandleCorrectly(string? key)
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act & Assert
            Should.NotThrow(() => manager.GetValue(key));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("test-value")]
        [InlineData("very-long-value-with-special-characters-!@#$%^&*()")]
        public void SetValue_WithDifferentValues_ShouldHandleCorrectly(string? value)
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act & Assert
            Should.NotThrow(() => manager.SetValue("test-key", value));
        }

        [Fact]
        public void Logger_ShouldNotBeNull()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act & Assert
            manager.Logger.ShouldNotBeNull();
        }

        [Fact]
        public void Constructor_WithInvalidAzureOptions_ShouldNotThrow()
        {
            // Arrange
            var options = new EafKeyVaultOptions
            {
                Provider = EnumKeyVault.Azure,
                Endpoint = new Uri("invalid://endpoint"),
                Azure = new AzureKeyVaultOptions()
            };

            // Act & Assert
            Should.NotThrow(() => new KeyVaultSecretManager(options));
        }

        [Fact]
        public void GetKeyValues_MultipleCallsWithNullProvider_ShouldReturnConsistentResults()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result1 = manager.GetKeyValues();
            var result2 = manager.GetKeyValues();

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();
            result1.Count.ShouldBe(result2.Count);
        }

        [Fact]
        public async Task GetKeyValuesAsync_MultipleCallsWithNullProvider_ShouldReturnConsistentResults()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            var result1 = await manager.GetKeyValuesAsync();
            var result2 = await manager.GetKeyValuesAsync();

            // Assert
            result1.ShouldNotBeNull();
            result2.ShouldNotBeNull();
            result1.Count.ShouldBe(result2.Count);
        }

        [Fact]
        public void SetValue_ThenGetValue_WithNullProvider_ShouldReturnNull()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            manager.SetValue("test-key", "test-value");
            var result = manager.GetValue("test-key");

            // Assert
            result.ShouldBeNull(); // NullKeyVaultManager doesn't store values
        }

        [Fact]
        public async Task SetValueAsync_ThenGetValueAsync_WithNullProvider_ShouldReturnNull()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);

            // Act
            await manager.SetValueAsync("test-key", "test-value");
            var result = await manager.GetValueAsync("test-key");

            // Assert
            result.ShouldBeNull(); // NullKeyVaultManager doesn't store values
        }

        [Theory]
        [InlineData(EnumKeyVault.None)]
        [InlineData(EnumKeyVault.Azure)]
        [InlineData(EnumKeyVault.OCI)]
        public void Constructor_WithDifferentProviders_ShouldCreateCorrectManager(EnumKeyVault provider)
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = provider };

            if (provider == EnumKeyVault.Azure)
            {
                options.Endpoint = new Uri("https://test.vault.azure.net/");
                options.Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "test-client-id",
                    ClientSecret = "test-client-secret",
                    TenantId = "test-tenant-id"
                };
            }

            // Act & Assert
            if (provider == EnumKeyVault.OCI)
            {
                // OCI requires configuration files, so we expect an exception
                Should.Throw<Exception>(() => new KeyVaultSecretManager(options));
            }
            else
            {
                Should.NotThrow(() => new KeyVaultSecretManager(options));
            }
        }

        [Fact]
        public void Constructor_WithDefaultOptions_ShouldUseNullProvider()
        {
            // Arrange
            var options = new EafKeyVaultOptions(); // Default provider is None

            // Act
            var manager = new KeyVaultSecretManager(options);

            // Assert
            manager.ShouldNotBeNull();
            manager.GetKeyValues().ShouldBeEmpty();
        }

        [Fact]
        public void GetValue_WithSpecialCharacterKeys_ShouldHandleCorrectly()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);
            var specialKeys = new[] { "key-with-dashes", "key_with_underscores", "key.with.dots", "key:with:colons" };

            // Act & Assert
            foreach (var key in specialKeys)
            {
                Should.NotThrow(() => manager.GetValue(key));
                Should.NotThrow(() => manager.SetValue(key, "test-value"));
            }
        }

        [Fact]
        public async Task GetValueAsync_WithSpecialCharacterKeys_ShouldHandleCorrectly()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);
            var specialKeys = new[] { "key-with-dashes", "key_with_underscores", "key.with.dots", "key:with:colons" };

            // Act & Assert
            foreach (var key in specialKeys)
            {
                await Should.NotThrowAsync(async () => await manager.GetValueAsync(key));
                await Should.NotThrowAsync(async () => await manager.SetValueAsync(key, "test-value"));
            }
        }

        [Fact]
        public void Constructor_WithNullOptionsValue_ShouldCreateDefaultOptions()
        {
            // Arrange
            EafKeyVaultOptions nullOptions = null;

            // Act
            var manager = new KeyVaultSecretManager((EafKeyVaultOptions?)nullOptions);

            // Assert
            manager.ShouldNotBeNull();
            manager.GetKeyValues().ShouldBeEmpty();
        }

        [Fact]
        public void SetValue_WithLargeValue_ShouldHandleCorrectly()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);
            var largeValue = new string('A', 10000); // 10KB string

            // Act & Assert
            Should.NotThrow(() => manager.SetValue("large-key", largeValue));
        }

        [Fact]
        public async Task SetValueAsync_WithLargeValue_ShouldHandleCorrectly()
        {
            // Arrange
            var options = new EafKeyVaultOptions { Provider = EnumKeyVault.None };
            var manager = new KeyVaultSecretManager(options);
            var largeValue = new string('A', 10000); // 10KB string

            // Act & Assert
            await Should.NotThrowAsync(async () => await manager.SetValueAsync("large-key", largeValue));
        }
    }
}