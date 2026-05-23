using Abp.Modules;
using Eaf.Middleware.AzureActiveDirectory;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests
{
    public class EafMiddlewareAzureActiveDirectoryModuleTests
    {
        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModule_ShouldHaveCorrectDependencies()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareAzureActiveDirectoryModule);
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
                .FirstOrDefault() as DependsOnAttribute;

            // Assert
            dependsOnAttribute.ShouldNotBeNull();
            dependsOnAttribute.DependedModuleTypes.ShouldNotBeEmpty();
        }

        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModule_ShouldBeInstantiable()
        {
            // Act & Assert
            Should.NotThrow(() => new EafMiddlewareAzureActiveDirectoryModule());
        }

        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModule_ShouldHavePreInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareAzureActiveDirectoryModule);
            var preInitializeMethod = moduleType.GetMethod("PreInitialize");

            // Assert
            preInitializeMethod.ShouldNotBeNull();
            preInitializeMethod.IsPublic.ShouldBeTrue();
            preInitializeMethod.IsVirtual.ShouldBeTrue();
        }

        [Fact]
        public void EafMiddlewareAzureActiveDirectoryModule_ShouldHaveInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(EafMiddlewareAzureActiveDirectoryModule);
            var initializeMethod = moduleType.GetMethod("Initialize");

            // Assert
            initializeMethod.ShouldNotBeNull();
            initializeMethod.IsPublic.ShouldBeTrue();
            initializeMethod.IsVirtual.ShouldBeTrue();
        }
    }
}