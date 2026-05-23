using Abp.Modules;
using Eaf.KeyVault;
using Eaf.KeyVault.AspNetCore;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.AspNetCore.Tests
{
    public class EafKeyVaultAspNetCoreModuleTests
    {
        [Fact]
        public void Module_ShouldInitialize()
        {
            // Arrange & Act
            var module = new EafKeyVaultAspNetCoreModule();

            // Assert
            module.ShouldNotBeNull();
        }

        [Fact]
        public void Module_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(EafKeyVaultAspNetCoreModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeEmpty();
            var dependsOn = (DependsOnAttribute)dependsOnAttribute[0];
            dependsOn.DependedModuleTypes.ShouldContain(typeof(EafKeyVaultModule));
        }

        [Fact]
        public void Module_ShouldInheritFromAbpModule()
        {
            // Arrange
            var module = new EafKeyVaultAspNetCoreModule();

            // Act & Assert
            module.ShouldBeAssignableTo<AbpModule>();
        }
    }
}