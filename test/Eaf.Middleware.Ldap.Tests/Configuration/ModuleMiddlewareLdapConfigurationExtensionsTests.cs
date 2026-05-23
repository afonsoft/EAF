using Eaf.Middleware.Ldap.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests.Configuration
{
    public class ModuleMiddlewareLdapConfigurationExtensionsTests
    {
        [Fact]
        public void ModuleMiddlewareLdapConfigurationExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(ModuleMiddlewareLdapConfigurationExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void ModuleMiddlewareLdapConfigurationExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(ModuleMiddlewareLdapConfigurationExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
