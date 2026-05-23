using System;
using System.Reflection;
using Abp.Runtime.Caching.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    public class EafSqliteCacheOptionsTests
    {
        private static string GetConnectionString(EafSqliteCacheOptions options)
        {
            var property = typeof(EafSqliteCacheOptions).GetProperty("ConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)property!.GetValue(options)!;
        }

        [Fact]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            // Act
            var options = new EafSqliteCacheOptions();

            // Assert
            options.MemoryOnly.ShouldBeFalse();
            options.CachePath.ShouldBe("SqliteCache.db");
            options.CleanupInterval.ShouldBe(TimeSpan.FromMinutes(30));
            ((IOptions<EafSqliteCacheOptions>)options).Value.ShouldBe(options);
        }

        [Fact]
        public void CachePath_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var path = "custom_cache.db";

            // Act
            options.CachePath = path;

            // Assert
            options.CachePath.ShouldBe(path);
        }

        [Fact]
        public void CachePath_WhenSetWithDataSourcePrefix_ShouldRemovePrefix()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var path = "Data Source=custom_cache.db";

            // Act
            options.CachePath = path;

            // Assert
            options.CachePath.ShouldBe("custom_cache.db");
        }

        [Fact]
        public void CachePath_WhenSetWithConnectionString_ShouldThrowException()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var connectionString = "Data Source=test.db;Mode=ReadWriteCreate";

            // Act & Assert
            Should.Throw<ArgumentException>(() => options.CachePath = connectionString)
                .Message.ShouldBe("CachePath must be a path and not a connection string!");
        }

        [Fact]
        public void MemoryOnly_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.MemoryOnly = true;

            // Assert
            options.MemoryOnly.ShouldBeTrue();
        }

        [Fact]
        public void CleanupInterval_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var interval = TimeSpan.FromMinutes(15);

            // Act
            options.CleanupInterval = interval;

            // Assert
            options.CleanupInterval.ShouldBe(interval);
        }

        [Fact]
        public void CleanupInterval_WhenSetToNull_ShouldDisableCleanup()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CleanupInterval = null;

            // Assert
            options.CleanupInterval.ShouldBeNull();
        }

        [Fact]
        public void ConnectionString_WhenMemoryOnly_ShouldReturnMemoryConnectionString()
        {
            // Arrange
            var options = new EafSqliteCacheOptions { MemoryOnly = true };

            // Act
            var connectionString = GetConnectionString(options);

            // Assert
            connectionString.ShouldContain("Data Source=:memory:");
            connectionString.ShouldContain("Mode=Memory");
            connectionString.ShouldContain("Cache=Shared");
        }

        [Fact]
        public void ConnectionString_WhenNotMemoryOnly_ShouldReturnFileConnectionString()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = false,
                CachePath = "test_cache.db"
            };

            // Act
            var connectionString = GetConnectionString(options);

            // Assert
            connectionString.ShouldContain("Data Source=test_cache.db");
            connectionString.ShouldContain("Mode=ReadWriteCreate");
            connectionString.ShouldContain("Cache=Shared");
        }

        [Theory]
        [InlineData("  test.db  ", "test.db")]
        [InlineData("Data Source=test.db", "test.db")]
        public void CachePath_ShouldHandleVariousInputFormats(string input, string expected)
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CachePath = input;

            // Assert
            options.CachePath.ShouldBe(expected);
        }

        [Fact]
        public void ConnectionString_WithCustomPath_ShouldIncludeCorrectPath()
        {
            // Arrange
            var customPath = "custom/path/cache.db";
            var options = new EafSqliteCacheOptions
            {
                CachePath = customPath,
                MemoryOnly = false
            };

            // Act
            var connectionString = GetConnectionString(options);

            // Assert
            connectionString.ShouldContain($"Data Source={customPath}");
        }

        [Fact]
        public void ConnectionString_ShouldAlwaysIncludeSharedCache()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            var connectionString = GetConnectionString(options);

            // Assert
            connectionString.ShouldContain("Cache=Shared");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MemoryOnly_ShouldAffectConnectionString(bool memoryOnly)
        {
            // Arrange
            var options = new EafSqliteCacheOptions { MemoryOnly = memoryOnly };

            // Act
            var connectionString = GetConnectionString(options);

            // Assert
            if (memoryOnly)
            {
                connectionString.ShouldContain("Data Source=:memory:");
                connectionString.ShouldContain("Mode=Memory");
            }
            else
            {
                connectionString.ShouldContain("Mode=ReadWriteCreate");
                connectionString.ShouldNotContain(":memory:");
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(60)]
        [InlineData(120)]
        public void CleanupInterval_WithDifferentMinutes_ShouldSetCorrectly(int minutes)
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var interval = TimeSpan.FromMinutes(minutes);

            // Act
            options.CleanupInterval = interval;

            // Assert
            options.CleanupInterval.ShouldBe(interval);
            options.CleanupInterval.Value.TotalMinutes.ShouldBe(minutes);
        }

        [Fact]
        public void CleanupInterval_WithZeroTimeSpan_ShouldSetCorrectly()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var interval = TimeSpan.Zero;

            // Act
            options.CleanupInterval = interval;

            // Assert
            options.CleanupInterval.ShouldBe(interval);
        }

        [Fact]
        public void CleanupInterval_WithNegativeTimeSpan_ShouldSetCorrectly()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();
            var interval = TimeSpan.FromMinutes(-5);

            // Act
            options.CleanupInterval = interval;

            // Assert
            options.CleanupInterval.ShouldBe(interval);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void CachePath_WithEmptyOrWhitespace_ShouldSetToEmptyString(string input)
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CachePath = input;

            // Assert
            options.CachePath.ShouldBe("");
        }

        [Fact]
        public void CachePath_WithNullValue_ShouldSetToEmptyString()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CachePath = null;

            // Assert
            options.CachePath.ShouldBe("");
        }

        [Theory]
        [InlineData("cache.db")]
        [InlineData("folder/cache.db")]
        [InlineData("../cache.db")]
        [InlineData("./cache.db")]
        [InlineData("C:\\temp\\cache.db")]
        [InlineData("/tmp/cache.db")]
        public void CachePath_WithValidPaths_ShouldSetCorrectly(string path)
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CachePath = path;

            // Assert
            options.CachePath.ShouldBe(path);
        }

        [Theory]
        [InlineData("Data Source=test.db;Version=3")]
        [InlineData("Data Source=test.db;Pooling=true")]
        [InlineData("Data Source=test.db;Journal Mode=WAL")]
        public void CachePath_WithConnectionStringLikeValues_ShouldThrowException(string invalidPath)
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act & Assert
            Should.Throw<ArgumentException>(() => options.CachePath = invalidPath);
        }

        [Fact]
        public void Value_Property_ShouldReturnSameInstance()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            var value = ((IOptions<EafSqliteCacheOptions>)options).Value;

            // Assert
            value.ShouldBeSameAs(options);
        }

        [Fact]
        public void MultiplePropertyChanges_ShouldMaintainConsistency()
        {
            // Arrange
            var options = new EafSqliteCacheOptions();

            // Act
            options.CachePath = "test_cache.db";
            options.MemoryOnly = true;
            options.CleanupInterval = TimeSpan.FromMinutes(45);

            // Assert
            options.CachePath.ShouldBe("test_cache.db");
            options.MemoryOnly.ShouldBeTrue();
            options.CleanupInterval.ShouldBe(TimeSpan.FromMinutes(45));

            // Connection string should reflect memory-only setting
            var connectionString = GetConnectionString(options);
            connectionString.ShouldContain("Data Source=:memory:");
        }
    }
}