using Abp.Modules;
using Eaf.Runtime.Caching.SqlServer;
using Shouldly;
using Xunit;

namespace Eaf.SqlServerCache.Tests
{
    public class EafSqlServerCacheModuleTests
    {
        [Fact]
        public void EafSqlServerCacheModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafSqlServerCacheModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void EafSqlServerCacheModule_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(EafSqlServerCacheModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeEmpty();
        }

        [Fact]
        public void EafSqlServerCacheModule_ShouldBeInstantiable()
        {
            // Act
            var module = new EafSqlServerCacheModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafSqlServerCacheModule>();
        }
    }
}