using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests.Runtime.Caching.Sqlite
{
    public class SqliteCacheConfigurationExtensionsTests
    {
        [Fact]
        public void SqliteCacheConfigurationExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(SqliteCacheConfigurationExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void SqliteCacheConfigurationExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(SqliteCacheConfigurationExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }
    }
}
