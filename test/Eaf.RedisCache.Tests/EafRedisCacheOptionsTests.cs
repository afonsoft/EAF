using Eaf.Runtime.Caching.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes para as opções de cache Redis.
    /// </summary>
    public class EafRedisCacheOptionsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            // Act
            var options = new EafRedisCacheOptions();

            // Assert
            options.InstanceName.ShouldBe("EAF");
            options.ConnectionString.ShouldBeEmpty();
            ((IOptions<EafRedisCacheOptions>)options).Value.ShouldBe(options);
            ((IOptions<RedisCacheOptions>)options).Value.ShouldBe(options);
        }

        [Fact]
        public void ConnectionString_WhenSet_ShouldUpdateConfiguration()
        {
            // Arrange
            var options = new EafRedisCacheOptions();

            // Act
            options.ConnectionString = "localhost:6379";

            // Assert
            options.ConnectionString.ShouldBe("localhost:6379");
            options.Configuration.ShouldBe("localhost:6379");
        }

        [Fact]
        public void InstanceName_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var options = new EafRedisCacheOptions();

            // Act
            options.InstanceName = "EAF_Production";

            // Assert
            options.InstanceName.ShouldBe("EAF_Production");
        }

        [Fact]
        public void Value_Property_ShouldReturnSameInstance()
        {
            // Arrange
            var options = new EafRedisCacheOptions();

            // Act
            var value = ((IOptions<EafRedisCacheOptions>)options).Value;

            // Assert
            value.ShouldBeSameAs(options);
        }
    }
}
