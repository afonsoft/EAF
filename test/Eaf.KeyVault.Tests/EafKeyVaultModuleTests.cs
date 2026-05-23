using Abp.Modules;
using Eaf.KeyVault;
using Shouldly;
using Xunit;

namespace Eaf.KeyVault.Tests
{
    public class EafKeyVaultModuleTests
    {
        [Fact]
        public void Module_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(EafKeyVaultModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeEmpty();
        }

        [Fact]
        public void Module_ShouldBeInstantiable()
        {
            // Arrange & Act
            var module = new EafKeyVaultModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafKeyVaultModule>();
        }

        [Fact]
        public void Module_ShouldInheritFromAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafKeyVaultModule);

            // Act & Assert
            moduleType.IsSubclassOf(typeof(AbpModule)).ShouldBeTrue();
        }
    }
}