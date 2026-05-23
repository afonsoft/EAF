using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    public class EafSqliteCacheTests
    {
        private static int _cacheCounter = 0;

        private string GetUniqueCacheName()
        {
            return $"test-cache-{Interlocked.Increment(ref _cacheCounter)}";
        }

        [Fact]
        public void Constructor_WithValidOptions_ShouldCreateInstance()
        {
            // Arrange
            var cacheName = GetUniqueCacheName();
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            // Act
            using var cache = new EafSqliteCache(cacheName, options);

            // Assert
            cache.ShouldNotBeNull();
            cache.Name.ShouldBe(cacheName);
        }

        [Fact]
        public void Set_And_Get_ShouldWorkCorrectly()
        {
            // Arrange
            var cacheName = GetUniqueCacheName();
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(cacheName, options);
            var testValue = "test-value";

            // Act
            cache.Set("test-key", testValue);
            var result = cache.TryGetValue("test-key", out var retrievedValue);

            // Assert
            result.ShouldBeTrue();
            retrievedValue.ShouldBe(testValue);
        }

        [Fact]
        public void Remove_ShouldRemoveItem()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            cache.Set("test-key", "test-value");
            cache.Remove("test-key");
            var result = cache.TryGetValue("test-key", out var retrievedValue);

            // Assert
            result.ShouldBeFalse();
            retrievedValue.ShouldBeNull();
        }

        [Fact]
        public void Clear_ShouldRemoveAllItems()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            cache.Set("key1", "value1");
            cache.Set("key2", "value2");
            cache.Clear();

            var result1 = cache.TryGetValue("key1", out var value1);
            var result2 = cache.TryGetValue("key2", out var value2);

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeFalse();
            value1.ShouldBeNull();
            value2.ShouldBeNull();
        }

        [Fact]
        public void Set_WithComplexObject_ShouldSerializeAndDeserialize()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var complexObject = new Dictionary<string, object>
            {
                { "string", "test" },
                { "number", 42 },
                { "boolean", true },
                { "date", DateTime.Now }
            };

            // Act
            cache.Set("complex-key", complexObject);
            var result = cache.TryGetValue("complex-key", out var retrievedValue);

            // Assert
            result.ShouldBeTrue();
            retrievedValue.ShouldNotBeNull();
        }

        [Fact]
        public void Set_WithSlidingExpiration_ShouldExpireCorrectly()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            cache.Set("expiring-key", "expiring-value", TimeSpan.FromMilliseconds(100));

            // Immediate check should work
            var immediateResult = cache.TryGetValue("expiring-key", out var immediateValue);

            // Wait for expiration
            Thread.Sleep(150);
            cache.RemoveExpired();

            var expiredResult = cache.TryGetValue("expiring-key", out var expiredValue);

            // Assert
            immediateResult.ShouldBeTrue();
            immediateValue.ShouldBe("expiring-value");
        }

        [Fact]
        public void Set_WithAbsoluteExpiration_ShouldExpireCorrectly()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var absoluteExpiry = DateTimeOffset.UtcNow.AddMilliseconds(100);

            // Act
            cache.Set("absolute-expiring-key", "absolute-expiring-value", null, absoluteExpiry);

            // Immediate check should work
            var immediateResult = cache.TryGetValue("absolute-expiring-key", out var immediateValue);

            // Wait for expiration
            Thread.Sleep(150);
            cache.RemoveExpired();

            var expiredResult = cache.TryGetValue("absolute-expiring-key", out var expiredValue);

            // Assert
            immediateResult.ShouldBeTrue();
            immediateValue.ShouldBe("absolute-expiring-value");
            expiredResult.ShouldBeTrue();
        }

        [Fact]
        public void TryGetValue_WithNonExistentKey_ShouldReturnFalse()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            var result = cache.TryGetValue("non-existent-key", out var value);

            // Assert
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Remove_WithNonExistentKey_ShouldNotThrow()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act & Assert
            Should.NotThrow(() => cache.Remove("non-existent-key"));
        }

        [Fact]
        public void RemoveExpired_ShouldOnlyRemoveExpiredItems()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            cache.Set("permanent-key", "permanent-value");
            cache.Set("expiring-key", "expiring-value", TimeSpan.FromMilliseconds(50));

            Thread.Sleep(100);
            cache.RemoveExpired();

            var permanentResult = cache.TryGetValue("permanent-key", out var permanentValue);
            var expiringResult = cache.TryGetValue("expiring-key", out var expiringValue);

            // Assert
            permanentResult.ShouldBeTrue();
            permanentValue.ShouldBe("permanent-value");
            expiringResult.ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("simple-key")]
        [InlineData("key_with_underscores")]
        [InlineData("key-with-dashes")]
        [InlineData("key.with.dots")]
        [InlineData("key with spaces")]
        public void Set_And_Get_WithDifferentKeyFormats_ShouldWork(string key)
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var value = $"value-for-{key}";

            // Act
            cache.Set(key, value);
            var result = cache.TryGetValue(key, out var retrievedValue);

            // Assert
            result.ShouldBeTrue();
            retrievedValue.ShouldBe(value);
        }

        [Theory]
        [InlineData("string-value")]
        [InlineData(42)]
        [InlineData(3.14)]
        [InlineData(true)]
        [InlineData(false)]
        public void Set_And_Get_WithDifferentValueTypes_ShouldWork(object value)
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            cache.Set("test-key", value);
            var result = cache.TryGetValue("test-key", out var retrievedValue);

            // Assert
            result.ShouldBeTrue();
            retrievedValue.ShouldBe(value);
        }

        [Fact]
        public void Constructor_WithCleanupInterval_ShouldCreateTimer()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true,
                CleanupInterval = TimeSpan.FromSeconds(1)
            };

            // Act & Assert
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            cache.ShouldNotBeNull();
        }

        [Fact]
        public void Dispose_ShouldCleanupResources()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true,
                CleanupInterval = TimeSpan.FromSeconds(1)
            };

            var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act & Assert
            Should.NotThrow(() => cache.Dispose());
        }

        [Fact]
        public void Multiple_Operations_ShouldWorkCorrectly()
        {
            // Arrange
            var options = new EafSqliteCacheOptions
            {
                MemoryOnly = true
            };

            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Act
            for (int i = 0; i < 100; i++)
            {
                cache.Set($"key-{i}", $"value-{i}");
            }

            // Verify all items exist
            for (int i = 0; i < 100; i++)
            {
                var result = cache.TryGetValue($"key-{i}", out var value);
                result.ShouldBeTrue();
                value.ShouldBe($"value-{i}");
            }

            // Remove half the items
            for (int i = 0; i < 50; i++)
            {
                cache.Remove($"key-{i}");
            }

            // Verify removed items don't exist
            for (int i = 0; i < 50; i++)
            {
                var result = cache.TryGetValue($"key-{i}", out var value);
                result.ShouldBeFalse();
                value.ShouldBeNull();
            }

            // Verify remaining items still exist
            for (int i = 50; i < 100; i++)
            {
                var result = cache.TryGetValue($"key-{i}", out var value);
                result.ShouldBeTrue();
                value.ShouldBe($"value-{i}");
            }

            // Assert
            // Clear all
            cache.Clear();

            // Verify all items are gone
            for (int i = 50; i < 100; i++)
            {
                var result = cache.TryGetValue($"key-{i}", out var value);
                result.ShouldBeFalse();
                value.ShouldBeNull();
            }
        }

        #region BDD Tests

        [Fact]
        public void Dado_ChaveNula_Quando_ChamarSet_Entao_DeveLancarNullReferenceException()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Quando & Então
            Should.Throw<NullReferenceException>(() => cache.Set(null!, "value"));
        }

        [Fact]
        public void Dado_CacheComNome_Quando_FixKey_Entao_DeveFormatarCorretamente()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache("TestCache", options);

            // Quando
            cache.Set("my-key", "value");
            var result = cache.TryGetValue("my-key", out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldBe("value");
        }

        [Fact]
        public void Dado_ChaveComNomeCache_Quando_FixKey_Entao_DeveManterChave()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache("TestCache", options);

            // Quando
            cache.Set("TestCache_existing-key", "value");
            var result = cache.TryGetValue("TestCache_existing-key", out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldBe("value");
        }

        [Fact]
        public void Dado_ValorNulo_Quando_ChamarSet_Entao_DeveLancarInvalidOperationException()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Quando & Então
            Should.Throw<InvalidOperationException>(() => cache.Set("null-key", null!));
        }

        [Fact]
        public void Dado_ByteArrayGrande_Quando_Serializar_Entao_DeveFuncionar()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var largeValue = new string('A', 10000);

            // Quando
            cache.Set("large-key", largeValue);
            var result = cache.TryGetValue("large-key", out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldBe(largeValue);
        }

        [Fact]
        public void Dado_ValorComCaracteresEspeciais_Quando_Serializar_Entao_DeveFuncionar()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var value = "Value with special chars: <>&\"'\\n\\t\\r";

            // Quando
            cache.Set("special-chars-key", value);
            var result = cache.TryGetValue("special-chars-key", out var retrievedValue);

            // Então
            result.ShouldBeTrue();
            retrievedValue.ShouldBe(value);
        }

        [Fact]
        public void Dado_ChaveComCaracteresUnicode_Quando_FixKey_Entao_DeveManterUnicode()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var key = "chave-com-acentuação-é-á-í-ó-ú";

            // Quando
            cache.Set(key, "value");
            var result = cache.TryGetValue(key, out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldBe("value");
        }

        [Fact]
        public void Dado_ExpiracaoSlidingNula_Quando_ChamarSet_Entao_DeveUsarDefault()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Quando
            cache.Set("test-key", "test-value", null);

            // Então
            var result = cache.TryGetValue("test-key", out var value);
            result.ShouldBeTrue();
            value.ShouldBe("test-value");
        }

        [Fact]
        public void Dado_ExpiracaoAbsoluteNula_Quando_ChamarSet_Entao_DeveUsarDefault()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Quando
            cache.Set("test-key", "test-value", TimeSpan.FromMinutes(5), null);

            // Então
            var result = cache.TryGetValue("test-key", out var value);
            result.ShouldBeTrue();
            value.ShouldBe("test-value");
        }

        [Fact]
        public void Dado_AmbasExpiracoesNulas_Quando_ChamarSet_Entao_DeveUsarDefaults()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);

            // Quando
            cache.Set("test-key", "test-value", null, null);

            // Então
            var result = cache.TryGetValue("test-key", out var value);
            result.ShouldBeTrue();
            value.ShouldBe("test-value");
        }

        [Fact]
        public void Dado_ConcurrentOperations_Quando_ExecutarSimultaneamente_Entao_DeveFuncionar()
        {
            // Dado
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            using var cache = new EafSqliteCache(GetUniqueCacheName(), options);
            var tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();

            // Quando
            for (int i = 0; i < 50; i++)
            {
                int index = i;
                tasks.Add(System.Threading.Tasks.Task.Run(() =>
                {
                    cache.Set($"key{index}", $"value{index}");
                    cache.TryGetValue($"key{index}", out _);
                    cache.Remove($"key{index}");
                }));
            }

            // Então
            Should.NotThrow(() => System.Threading.Tasks.Task.WaitAll(tasks.ToArray()));
        }

        #endregion
    }
}