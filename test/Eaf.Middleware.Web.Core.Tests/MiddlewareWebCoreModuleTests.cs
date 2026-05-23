using Abp.Modules;
using Eaf.Middleware.Web.Core;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests
{
    public class MiddlewareWebCoreModuleTests
    {
        [Fact]
        public void MiddlewareWebCoreModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(MiddlewareWebCoreModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void MiddlewareWebCoreModule_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(MiddlewareWebCoreModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeNull();
            dependsOnAttribute.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void MiddlewareWebCoreModule_ShouldRequireIHostEnvironment()
        {
            // Arrange
            var moduleType = typeof(MiddlewareWebCoreModule);
            var constructor = moduleType.GetConstructors()[0];
            var parameters = constructor.GetParameters();

            // Assert
            parameters.Length.ShouldBe(1);
            parameters[0].ParameterType.ShouldBe(typeof(Microsoft.Extensions.Hosting.IHostEnvironment));
        }
    }
}
