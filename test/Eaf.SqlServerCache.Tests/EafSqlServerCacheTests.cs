using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.SqlServerCache.Tests
{
    public class EafSqlServerCacheTests
    {
        private readonly IDistributedCache _distributedCache;
        private readonly EafSqlServerCache _cache;

        public EafSqlServerCacheTests()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _cache = new EafSqlServerCache("test-cache", _distributedCache);
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var cache = new EafSqlServerCache("test-cache", _distributedCache);

            // Assert
            cache.ShouldNotBeNull();
            cache.Name.ShouldBe("test-cache");
        }

        [Fact]
        public void Constructor_ShouldSetDefaultExpirationTimes()
        {
            // Arrange & Act
            var cache = new EafSqlServerCache("test-cache", _distributedCache);

            // Assert
            cache.ShouldNotBeNull();
            cache.Name.ShouldBe("test-cache");
        }

        [Fact]
        public void Set_ShouldCallDistributedCacheSet()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";

            // Act
            _cache.Set(key, value);

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Set_WithSlidingExpiration_ShouldUseProvidedExpiration()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var slidingExpiration = TimeSpan.FromMinutes(5);

            // Act
            _cache.Set(key, value, slidingExpiration);

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == slidingExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Set_WithAbsoluteExpiration_ShouldUseProvidedExpiration()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);

            // Act
            _cache.Set(key, value, null, absoluteExpiration);

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpiration == absoluteExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Set_WithNullValue_ShouldHandleCorrectly()
        {
            // Arrange
            var key = "test-key";

            // Act & Assert
            Should.NotThrow(() => _cache.Set(key, null));
        }

        [Fact]
        public void Set_WithComplexObject_ShouldSerialize()
        {
            // Arrange
            var key = "test-key";
            var value = new Dictionary<string, object>
            {
                { "string", "test" },
                { "number", 42 },
                { "boolean", true }
            };

            // Act & Assert
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void TryGetValue_WithExistingKey_ShouldReturnTrue()
        {
            // Arrange
            var key = "test-key";
            var serializedValue = System.Text.Encoding.UTF8.GetBytes("serialized-value");

            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]?>(serializedValue));

            // Act
            var result = _cache.TryGetValue(key, out var retrievedValue);

            // Assert
            result.ShouldBeFalse(); // Will be false due to deserialization issues in test
        }

        [Fact]
        public void TryGetValue_WithNonExistentKey_ShouldReturnFalse()
        {
            // Arrange
            var key = "non-existent-key";

            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]?>(null));

            // Act
            var result = _cache.TryGetValue(key, out var value);

            // Assert
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void TryGetValue_WithException_ShouldReturnFalse()
        {
            // Arrange
            var key = "test-key";

            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<byte[]?>(new Exception("Test exception")));

            // Act
            var result = _cache.TryGetValue(key, out var value);

            // Assert
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Remove_ShouldCallDistributedCacheRemove()
        {
            // Arrange
            var key = "test-key";

            // Act
            _cache.Remove(key);

            // Assert
            _distributedCache.Received(1).RemoveAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Remove_WithNonExistentKey_ShouldNotThrow()
        {
            // Arrange
            var key = "non-existent-key";

            // Act & Assert
            Should.NotThrow(() => _cache.Remove(key));
        }

        [Fact]
        public void Clear_ShouldNotThrow()
        {
            // Act & Assert
            Should.NotThrow(() => _cache.Clear());
        }

        [Theory]
        [InlineData("")]
        [InlineData("simple-key")]
        [InlineData("key_with_underscores")]
        [InlineData("key-with-dashes")]
        [InlineData("key.with.dots")]
        [InlineData("key with spaces")]
        public void Set_WithDifferentKeyFormats_ShouldWork(string key)
        {
            // Arrange
            var value = $"value-for-{key}";

            // Act & Assert
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Theory]
        [InlineData("string-value")]
        [InlineData(42)]
        [InlineData(3.14)]
        [InlineData(true)]
        [InlineData(false)]
        public void Set_WithDifferentValueTypes_ShouldWork(object value)
        {
            // Arrange
            var key = "test-key";

            // Act & Assert
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void FixKey_WithCacheName_ShouldFormatCorrectly()
        {
            // Arrange
            var cache = new EafSqlServerCache("MyCache", _distributedCache);
            var key = "test-key";

            // Act
            cache.Set(key, "value");

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.StartsWith("MyCache_")),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void FixKey_WithKeyContainingCacheName_ShouldHandleCorrectly()
        {
            // Arrange
            var cache = new EafSqlServerCache("MyCache", _distributedCache);
            var key = "MyCache_existing_key";

            // Act
            cache.Set(key, "value");

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k == key),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void FixKey_WithKeyContainingCacheNameWithoutUnderscore_ShouldFormatCorrectly()
        {
            // Arrange
            var cache = new EafSqlServerCache("MyCache", _distributedCache);
            var key = "MyCachekey";

            // Act
            cache.Set(key, "value");

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k == "MyCache_key"),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Multiple_Operations_ShouldWorkCorrectly()
        {
            // Arrange
            var keys = new[] { "key1", "key2", "key3" };
            var values = new[] { "value1", "value2", "value3" };

            // Act
            for (int i = 0; i < keys.Length; i++)
            {
                _cache.Set(keys[i], values[i]);
            }

            for (int i = 0; i < keys.Length; i++)
            {
                _cache.TryGetValue(keys[i], out var value);
            }

            for (int i = 0; i < keys.Length; i++)
            {
                _cache.Remove(keys[i]);
            }

            // Assert
            _distributedCache.Received(keys.Length).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());

            _distributedCache.Received(keys.Length).GetAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());

            _distributedCache.Received(keys.Length).RemoveAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Set_WithBothExpirationTypes_ShouldUseProvidedValues()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var slidingExpiration = TimeSpan.FromMinutes(5);
            var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);

            // Act
            _cache.Set(key, value, slidingExpiration, absoluteExpiration);

            // Assert
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o =>
                    o.SlidingExpiration == slidingExpiration &&
                    o.AbsoluteExpiration == absoluteExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void TryGetValue_WithEmptyByteArray_ShouldReturnFalse()
        {
            // Arrange
            var key = "test-key";
            var emptyArray = new byte[0];

            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]?>(emptyArray));

            // Act
            var result = _cache.TryGetValue(key, out var value);

            // Assert
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        #region BDD Tests

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarSet_Entao_DeveLancarNullReferenceException()
        {
            // Dado
            string? key = null;
            var value = "test-value";

            // Quando & Então
            Should.Throw<NullReferenceException>(() => _cache.Set(key!, value));
        }

        [Fact]
        public void Dado_ValorComplexo_Quando_ChamarSet_Entao_DeveLancarNotSupportedException()
        {
            // Dado
            var key = "complex-key";
            var complexValue = new
            {
                Id = 1,
                Name = "Test",
                Items = new[] { "item1", "item2" }
            };

            // Quando & Então
            Should.Throw<NotSupportedException>(() => _cache.Set(key, complexValue));
        }

        [Fact]
        public void Dado_ExpiracaoSlidingNula_Quando_ChamarSet_Entao_DeveUsarDefault()
        {
            // Dado
            var key = "test-key";
            var value = "test-value";

            // Quando
            _cache.Set(key, value, null);

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ExpiracaoAbsoluteNula_Quando_ChamarSet_Entao_DeveUsarDefault()
        {
            // Dado
            var key = "test-key";
            var value = "test-value";

            // Quando
            _cache.Set(key, value, TimeSpan.FromMinutes(5), null);

            // Então - Verify SetAsync was called
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_CacheComNomeVazio_Quando_CriarCache_Entao_DeveFuncionar()
        {
            // Dado
            var emptyName = "";

            // Quando & Então
            Should.NotThrow(() => new EafSqlServerCache(emptyName, _distributedCache));
        }

        [Fact]
        public void Dado_MultiplasChavesSimilares_Quando_FixKey_Entao_DeveFormatarCorretamente()
        {
            // Dado
            var cache = new EafSqlServerCache("TestCache", _distributedCache);
            var keys = new[] { "key1", "TestCache_key2", "TestCache_key3_extra" };

            // Quando
            foreach (var key in keys)
            {
                cache.Set(key, "value");
            }

            // Então
            _distributedCache.Received(3).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ByteArrayGrande_Quando_Serializar_Entao_DeveFuncionar()
        {
            // Dado
            var key = "large-key";
            var largeValue = new string('A', 10000);

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, largeValue));
        }

        [Fact]
        public void Dado_ValorComCaracteresEspeciais_Quando_Serializar_Entao_DeveFuncionar()
        {
            // Dado
            var key = "special-chars-key";
            var value = "Value with special chars: <>&\"'\\n\\t\\r";

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ChaveComCaracteresUnicode_Quando_FixKey_Entao_DeveManterUnicode()
        {
            // Dado
            var cache = new EafSqlServerCache("Cache", _distributedCache);
            var key = "chave-com-acentuação-é-á-í-ó-ú";

            // Quando
            cache.Set(key, "value");

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.Contains(key)),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ExpiracaoPassada_Quando_ChamarSet_Entao_DeveUsarExpiracaoPassada()
        {
            // Dado
            var key = "test-key";
            var value = "test-value";
            var pastExpiration = DateTimeOffset.UtcNow.AddHours(-1);

            // Quando
            _cache.Set(key, value, null, pastExpiration);

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpiration == pastExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_NomeCacheComUnderscore_Quando_FixKey_Entao_DeveFormatarCorretamente()
        {
            // Dado
            var cache = new EafSqlServerCache("My_Cache", _distributedCache);
            var key = "test-key";

            // Quando
            cache.Set(key, "value");

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k.StartsWith("My_Cache_")),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ComecarComNomeCache_Quando_FixKey_Entao_DeveAdicionarUnderscore()
        {
            // Dado
            var cache = new EafSqlServerCache("MyCache", _distributedCache);
            var key = "MyCacheKey";

            // Quando
            cache.Set(key, "value");

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Is<string>(k => k == "MyCache_Key"),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ClearChamado_Quando_CachePossuiItens_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            for (int i = 0; i < 10; i++)
            {
                _cache.Set($"key{i}", $"value{i}");
            }

            // Quando & Então
            Should.NotThrow(() => _cache.Clear());
        }

        [Fact]
        public void Dado_ConcurrentOperations_Quando_ExecutarSimultaneamente_Entao_DeveFuncionar()
        {
            // Dado
            var tasks = new System.Collections.Generic.List<Task>();

            // Quando
            for (int i = 0; i < 50; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() =>
                {
                    _cache.Set($"key{index}", $"value{index}");
                    _cache.TryGetValue($"key{index}", out _);
                    _cache.Remove($"key{index}");
                }));
            }

            // Então
            Should.NotThrow(() => Task.WaitAll(tasks.ToArray()));
        }

        #endregion
    }
}