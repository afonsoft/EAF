using Abp.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests
{
    public class LdapSettingProviderTests
    {
        [Fact]
        public void LdapSettingProvider_ShouldBeSettingProvider()
        {
            // Arrange & Act
            var type = typeof(LdapSettingProvider);

            // Assert
            type.ShouldNotBeNull();
            typeof(SettingProvider).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void LdapSettingProvider_ShouldBeInstantiable()
        {
            // Arrange & Act
            var provider = new LdapSettingProvider();

            // Assert
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<LdapSettingProvider>();
        }
    }
}
