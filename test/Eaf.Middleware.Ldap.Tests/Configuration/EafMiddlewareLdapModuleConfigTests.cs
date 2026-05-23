using Abp.Zero.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests.Configuration
{
    public class EafMiddlewareLdapModuleConfigTests
    {
        [Fact]
        public void EafMiddlewareLdapModuleConfig_ShouldImplementIEafMiddlewareLdapModuleConfig()
        {
            // Arrange & Act
            var type = typeof(EafMiddlewareLdapModuleConfig);

            // Assert
            type.ShouldNotBeNull();
            typeof(IEafMiddlewareLdapModuleConfig).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void EafMiddlewareLdapModuleConfig_ShouldBeInstantiable()
        {
            // Arrange
            var middlewareConfig = Substitute.For<IAbpZeroConfig>();

            // Act
            var config = new EafMiddlewareLdapModuleConfig(middlewareConfig);

            // Assert
            config.ShouldNotBeNull();
            config.ShouldBeOfType<EafMiddlewareLdapModuleConfig>();
        }
    }
}
