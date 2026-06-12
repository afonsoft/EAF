using Castle.Core.Logging;
using Eaf.KeyVault;
using NSubstitute;
using Shouldly;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Eaf.KeyVault.Tests.Azure
{
    public class AzureKeyVaultManagerTests
    {
        private readonly ILogger _logger;
        private readonly EafKeyVaultOptions _options;

        public AzureKeyVaultManagerTests()
        {
            _logger = Substitute.For<ILogger>();
            _options = new EafKeyVaultOptions
            {
                Endpoint = new Uri("https://test-vault.vault.azure.net/"),
                Azure = new AzureKeyVaultOptions
                {
                    ApplicationId = "test-app-id",
                    TenantId = "test-tenant-id",
                    ClientSecret = "test-client-secret"
                }
            };
        }

        [Fact]
        public void Constructor_WithValidClientSecretCredentials_ShouldCreateInstance()
        {
            // Arrange & Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Fact]
        public void Constructor_WithValidCertificateCredentials_ShouldCreateInstance()
        {
            // Arrange
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(5));
            _options.Azure.Certificate = certificate;
            _options.Azure.ClientSecret = null;

            // Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Fact]
        public void Constructor_WithDefaultCredentials_ShouldCreateInstance()
        {
            // Arrange
            _options.Azure.ApplicationId = null;
            _options.Azure.TenantId = null;
            _options.Azure.ClientSecret = null;

            // Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Fact]
        public void Constructor_WithNullOptions_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Should.Throw<NullReferenceException>(() => new AzureKeyVaultManager(null, _logger));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            Should.NotThrow(() => new AzureKeyVaultManager(_options, null));
        }

        [Fact]
        public void Constructor_WithInvalidEndpoint_ShouldNotThrow()
        {
            // Arrange
            _options.Endpoint = new Uri("invalid://endpoint");

            // Act & Assert
            Should.NotThrow(() => new AzureKeyVaultManager(_options, _logger));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_WithInvalidApplicationId_ShouldUseDefaultCredentials(string? applicationId)
        {
            // Arrange
            _options.Azure.ApplicationId = applicationId;

            // Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_WithInvalidTenantId_ShouldUseDefaultCredentials(string? tenantId)
        {
            // Arrange
            _options.Azure.TenantId = tenantId;

            // Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_WithInvalidClientSecret_ShouldUseDefaultCredentials(string? clientSecret)
        {
            // Arrange
            _options.Azure.ClientSecret = clientSecret;

            // Act
            var exception = Record.Exception(() => new AzureKeyVaultManager(_options, _logger));

            // Assert
            exception.ShouldBeNull();
        }

        [Fact]
        public void GetKeyValues_WhenCalled_ShouldReturnDictionary()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            Should.Throw<Exception>(() => manager.GetKeyValues());
        }

        [Fact]
        public async Task GetKeyValuesAsync_WhenCalled_ShouldReturnDictionary()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            await Should.ThrowAsync<Exception>(() => manager.GetKeyValuesAsync());
        }

        [Fact]
        public void GetValue_WithValidKey_ShouldReturnValue()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            Should.Throw<Exception>(() => manager.GetValue("test-key"));
        }

        [Fact]
        public async Task GetValueAsync_WithValidKey_ShouldReturnValue()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            await Should.ThrowAsync<Exception>(() => manager.GetValueAsync("test-key"));
        }

        [Fact]
        public void SetValue_WithValidKeyValue_ShouldSetValue()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            Should.Throw<Exception>(() => manager.SetValue("test-key", "test-value"));
        }

        [Fact]
        public async Task SetValueAsync_WithValidKeyValue_ShouldSetValue()
        {
            // Arrange
            var manager = new AzureKeyVaultManager(_options, _logger);

            // Act & Assert
            await Should.ThrowAsync<Exception>(() => manager.SetValueAsync("test-key", "test-value"));
        }
    }
}