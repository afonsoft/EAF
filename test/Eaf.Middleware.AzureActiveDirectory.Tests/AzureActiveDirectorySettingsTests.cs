using Abp.Configuration;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests
{
    public class AzureActiveDirectorySettingsTests
    {
        private readonly ISettingManager _settingManager;
        private readonly AzureActiveDirectorySettings _settings;

        public AzureActiveDirectorySettingsTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _settings = new AzureActiveDirectorySettings(_settingManager);
        }

        [Fact]
        public async Task GetClientId_ShouldReturnValueFromSettingManager()
        {
            // Arrange
            var expectedClientId = "test-client-id";
            _settingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientId)
                .Returns(Task.FromResult(expectedClientId));

            // Act
            var result = await _settings.GetClientId();

            // Assert
            result.ShouldBe(expectedClientId);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientId);
        }

        [Fact]
        public async Task GetClientSecret_ShouldReturnValueFromSettingManager()
        {
            // Arrange
            var expectedClientSecret = "test-client-secret";
            _settingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret)
                .Returns(Task.FromResult(expectedClientSecret));

            // Act
            var result = await _settings.GetClientSecret();

            // Assert
            result.ShouldBe(expectedClientSecret);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.ClientSecret);
        }

        [Fact]
        public async Task GetTenant_ShouldReturnValueFromSettingManager()
        {
            // Arrange
            var expectedTenant = "test-tenant";
            _settingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.Tenant)
                .Returns(Task.FromResult(expectedTenant));

            // Act
            var result = await _settings.GetTenant();

            // Assert
            result.ShouldBe(expectedTenant);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.Tenant);
        }

        [Fact]
        public void Constructor_WithValidSettingManager_ShouldNotThrow()
        {
            // Act & Assert
            Should.NotThrow(() => new AzureActiveDirectorySettings(_settingManager));
        }

        [Fact]
        public void Constructor_WithNullSettingManager_ShouldThrow()
        {
            // Act & Assert
            Should.Throw<System.ArgumentNullException>(() => new AzureActiveDirectorySettings(null));
        }
    }
}