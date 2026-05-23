using Abp.Modules;
using Eaf.Middleware.Worker;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    public class MiddlewareWorkerModuleTests
    {
        [Fact]
        public void MiddlewareWorkerModule_ShouldBeAbpModule()
        {
            // Arrange & Act
            var moduleType = typeof(MiddlewareWorkerModule);

            // Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void MiddlewareWorkerModule_ShouldHaveCorrectDependencies()
        {
            // Arrange & Act
            var moduleType = typeof(MiddlewareWorkerModule);
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false)
                .FirstOrDefault() as DependsOnAttribute;

            // Assert
            dependsOnAttribute.ShouldNotBeNull();
            dependsOnAttribute.DependedModuleTypes.ShouldNotBeEmpty();
        }

        [Fact]
        public void MiddlewareWorkerModule_ShouldBeInstantiableWithHostEnvironment()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Test");

            // Act & Assert
            Should.NotThrow(() => new MiddlewareWorkerModule(hostEnvironment));
        }

        [Fact]
        public void MiddlewareWorkerModule_ShouldHavePreInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(MiddlewareWorkerModule);
            var preInitializeMethod = moduleType.GetMethod("PreInitialize");

            // Assert
            preInitializeMethod.ShouldNotBeNull();
            preInitializeMethod.IsPublic.ShouldBeTrue();
            preInitializeMethod.IsVirtual.ShouldBeTrue();
        }

        [Fact]
        public void MiddlewareWorkerModule_ShouldHaveInitializeMethod()
        {
            // Arrange & Act
            var moduleType = typeof(MiddlewareWorkerModule);
            var initializeMethod = moduleType.GetMethod("Initialize");

            // Assert
            initializeMethod.ShouldNotBeNull();
            initializeMethod.IsPublic.ShouldBeTrue();
            initializeMethod.IsVirtual.ShouldBeTrue();
        }

        [Fact]
        public void MiddlewareWorkerModule_WithValidHostEnvironment_ShouldInitializeCorrectly()
        {
            // Arrange
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            // Act
            var module = new MiddlewareWorkerModule(hostEnvironment);

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeAssignableTo<AbpModule>();
        }
    }
}