using Abp.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests
{
    public class LdapSettingsTests
    {
        private readonly ISettingManager _settingManager;
        private readonly LdapSettings _ldapSettings;

        public LdapSettingsTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _ldapSettings = new LdapSettings(_settingManager);
        }

        [Fact]
        public void LdapSettings_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var settings = new LdapSettings(_settingManager);

            // Assert
            settings.ShouldNotBeNull();
            settings.ShouldBeAssignableTo<ILdapSettings>();
        }

        [Fact]
        public async Task GetDomain_WithTenantId_ShouldCallCorrectMethod()
        {
            // Arrange
            var tenantId = 1;
            var domain = "example.com";
            _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Domain, tenantId)
                .Returns(Task.FromResult(domain));

            // Act
            var result = await _ldapSettings.GetDomain(tenantId);

            // Assert
            result.ShouldBe(domain);
            await _settingManager.Received(1).GetSettingValueForTenantAsync(LdapSettingNames.Domain, tenantId);
        }

        [Fact]
        public async Task GetUserName_WithoutTenantId_ShouldCallCorrectMethod()
        {
            // Arrange
            var userName = "testuser";
            _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.UserName)
                .Returns(Task.FromResult(userName));

            // Act
            var result = await _ldapSettings.GetUserName(null);

            // Assert
            result.ShouldBe(userName);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(LdapSettingNames.UserName);
        }

        [Fact]
        public async Task GetPassword_WithTenantId_ShouldCallCorrectMethod()
        {
            // Arrange
            var tenantId = 2;
            var password = "testpassword";
            _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Password, tenantId)
                .Returns(Task.FromResult(password));

            // Act
            var result = await _ldapSettings.GetPassword(tenantId);

            // Assert
            result.ShouldBe(password);
            await _settingManager.Received(1).GetSettingValueForTenantAsync(LdapSettingNames.Password, tenantId);
        }

        [Fact]
        public async Task GetContainer_WithoutTenantId_ShouldCallCorrectMethod()
        {
            // Arrange
            var container = "CN=Users,DC=example,DC=com";
            _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Container)
                .Returns(Task.FromResult(container));

            // Act
            var result = await _ldapSettings.GetContainer(null);

            // Assert
            result.ShouldBe(container);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(LdapSettingNames.Container);
        }

        [Fact]
        public async Task GetIsEnabled_WithoutTenantId_ShouldCallCorrectMethod()
        {
            _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.IsEnabled)
                .Returns(Task.FromResult("true"));

            var result = await _ldapSettings.GetIsEnabled(null);

            result.ShouldBeTrue();
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(LdapSettingNames.IsEnabled);
        }

        [Fact]
        public async Task GetContextType_WithoutTenantId_ShouldReturnNullOnNonWindows()
        {
            if (!OperatingSystem.IsWindows())
            {
                var result = await _ldapSettings.GetContextType(null);
                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task GetContextType_WithTenantId_ShouldReturnNullOnNonWindows()
        {
            if (!OperatingSystem.IsWindows())
            {
                var result = await _ldapSettings.GetContextType(1);
                result.ShouldBeNull();
            }
        }

        [Fact]
        public async Task GetIsEnabled_WithTenantId_ShouldCallCorrectMethod()
        {
            _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.IsEnabled, 1).Returns(Task.FromResult("true"));

            var result = await _ldapSettings.GetIsEnabled(1);

            result.ShouldBeTrue();
            await _settingManager.Received(1).GetSettingValueForTenantAsync(LdapSettingNames.IsEnabled, 1);
        }

        [Fact]
        public async Task GetUserName_WithTenantId_ShouldCallCorrectMethod()
        {
            var userName = "tenantuser";
            _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.UserName, 1).Returns(Task.FromResult(userName));

            var result = await _ldapSettings.GetUserName(1);

            result.ShouldBe(userName);
            await _settingManager.Received(1).GetSettingValueForTenantAsync(LdapSettingNames.UserName, 1);
        }

        [Fact]
        public async Task GetPassword_WithoutTenantId_ShouldCallCorrectMethod()
        {
            var password = "password";
            _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Password).Returns(Task.FromResult(password));

            var result = await _ldapSettings.GetPassword(null);

            result.ShouldBe(password);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(LdapSettingNames.Password);
        }

        [Fact]
        public async Task GetContainer_WithTenantId_ShouldCallCorrectMethod()
        {
            var container = "CN=Users,DC=example,DC=com";
            _settingManager.GetSettingValueForTenantAsync(LdapSettingNames.Container, 1).Returns(Task.FromResult(container));

            var result = await _ldapSettings.GetContainer(1);

            result.ShouldBe(container);
            await _settingManager.Received(1).GetSettingValueForTenantAsync(LdapSettingNames.Container, 1);
        }

        [Fact]
        public async Task GetDomain_WithoutTenantId_ShouldCallCorrectMethod()
        {
            var domain = "example.com";
            _settingManager.GetSettingValueForApplicationAsync(LdapSettingNames.Domain).Returns(Task.FromResult(domain));

            var result = await _ldapSettings.GetDomain(null);

            result.ShouldBe(domain);
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(LdapSettingNames.Domain);
        }
    }
}