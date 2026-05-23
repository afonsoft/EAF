using Abp.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using NSubstitute;
using Shouldly;
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
    }
}