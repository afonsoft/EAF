using Abp.Modules;
using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    public class EafSqliteCacheModuleTests
    {
        [Fact]
        public void EafSqliteCacheModule_ShouldBeAbpModule()
        {
            // Arrange
            var moduleType = typeof(EafSqliteCacheModule);

            // Act & Assert
            typeof(AbpModule).IsAssignableFrom(moduleType).ShouldBeTrue();
        }

        [Fact]
        public void EafSqliteCacheModule_ShouldHaveCorrectDependencies()
        {
            // Arrange
            var moduleType = typeof(EafSqliteCacheModule);

            // Act
            var dependsOnAttribute = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), false);

            // Assert
            dependsOnAttribute.ShouldNotBeEmpty();
        }

        [Fact]
        public void EafSqliteCacheModule_ShouldBeInstantiable()
        {
            // Act
            var module = new EafSqliteCacheModule();

            // Assert
            module.ShouldNotBeNull();
            module.ShouldBeOfType<EafSqliteCacheModule>();
        }
    }
}