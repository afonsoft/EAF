using Abp.Modules;
using Abp.Zero;
using Eaf.Middleware.Ldap;
using Shouldly;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests
{
    public class EafMiddlewareLdapModuleTests
    {
        [Fact]
        public void EafMiddlewareLdapModule_ShouldHaveCorrectDependencies()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareLdapModule);
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
                .FirstOrDefault() as DependsOnAttribute;

            // Assert
            dependsOnAttribute.ShouldNotBeNull();
            dependsOnAttribute.DependedModuleTypes.ShouldNotBeEmpty();
            dependsOnAttribute.DependedModuleTypes.ShouldContain(typeof(AbpZeroCommonModule));
        }

        [Fact]
        public void EafMiddlewareLdapModule_ShouldBeInstantiable()
        {
            // Act & Assert
            Should.NotThrow(() => new EafMiddlewareLdapModule());
        }

        [Fact]
        public void EafMiddlewareLdapModule_ShouldHavePreInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareLdapModule);
            var preInitializeMethod = moduleType.GetMethod("PreInitialize");

            // Assert
            preInitializeMethod.ShouldNotBeNull();
            preInitializeMethod.IsPublic.ShouldBeTrue();
            preInitializeMethod.IsVirtual.ShouldBeTrue();
        }

        [Fact]
        public void EafMiddlewareLdapModule_ShouldHaveInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareLdapModule);
            var initializeMethod = moduleType.GetMethod("Initialize");

            // Assert
            initializeMethod.ShouldNotBeNull();
            initializeMethod.IsPublic.ShouldBeTrue();
            initializeMethod.IsVirtual.ShouldBeTrue();
        }
    }
}