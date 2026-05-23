using Eaf.Runtime.Caching.SqlServer;
using Shouldly;
using Xunit;

namespace Eaf.SqlServerCache.Tests
{
    public class SqlServerCacheHelperTests
    {
        [Fact]
        public void EafSqlServerCache_ShouldBeInstantiable()
        {
            // Arrange & Act
            var cache = new EafSqlServerCache("TestCache", null);

            // Assert
            cache.ShouldNotBeNull();
            cache.ShouldBeOfType<EafSqlServerCache>();
        }

        [Fact]
        public void EafSqlServerCache_ShouldHaveName()
        {
            // Arrange & Act
            var cache = new EafSqlServerCache("TestCache", null);

            // Assert
            cache.Name.ShouldBe("TestCache");
        }
    }
}
