using Eaf.KeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shouldly;
using System;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class EafKeyVaultHostExtensionsTests_Fixed
    {
        [Fact]
        public void UseEafKeyVault_WithoutOptions_ShouldConfigureServices()
        {
            // Arrange
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

            // Act
            var result = hostBuilder.UseEafKeyVault();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(hostBuilder);
        }

        [Fact]
        public void UseEafKeyVault_WithOptions_ShouldConfigureServices()
        {
            // Arrange
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

            // Act
            var result = hostBuilder.UseEafKeyVault(options =>
            {
                options.Provider = EnumKeyVault.Azure;
                options.Endpoint = new System.Uri("https://test-vault.vault.azure.net/");
            });

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(hostBuilder);
        }

        [Fact]
        public void UseEafKeyVault_ShouldRegisterOptionsInDI()
        {
            // Arrange
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

            // Act
            hostBuilder.UseEafKeyVault(options =>
            {
                options.Provider = EnumKeyVault.OCI;
                options.Endpoint = new System.Uri("https://test-vault.oci.com/");
            });

            var host = hostBuilder.Build();

            // Assert
            var optionsService = host.Services.GetService<IOptions<EafKeyVaultOptions>>();
            optionsService.ShouldNotBeNull();
        }

        [Fact]
        public void UseEafKeyVault_WithNullOptions_ShouldNotThrow()
        {
            // Arrange
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

            // Act & Assert
            Should.NotThrow(() => hostBuilder.UseEafKeyVault(null));
        }
    }
}