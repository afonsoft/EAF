using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests.Runtime.Caching.Sqlite
{
    public class EafSqliteCacheOptionsTests
    {
        [Fact]
        public void EafSqliteCacheOptions_ShouldBeInstantiable()
        {
            // Arrange & Act
            var options = new EafSqliteCacheOptions();

            // Assert
            options.ShouldNotBeNull();
            options.ShouldBeOfType<EafSqliteCacheOptions>();
        }
    }
}
