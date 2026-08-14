using Eaf.Runtime.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes adicionais de serialização para EafRedisCache.
    /// </summary>
    public class EafRedisCacheSerializationTests
    {
        private readonly IDistributedCache _distributedCache;
        private readonly EafRedisCache _cache;

        public EafRedisCacheSerializationTests()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _cache = new EafRedisCache("serialization-tests", _distributedCache, new EafRedisCacheOptions());
        }

        [Theory]
        [InlineData(42)]
        [InlineData(3.14159)]
        [InlineData(true)]
        public void Dado_ValorPrimitivo_Quando_Set_Entao_DeveSerializarSemErro(object value)
        {
            // Dado
            var key = $"{value.GetType().Name}-key";

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
            _distributedCache.Received(1).Set(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>());
        }

        [Fact]
        public void Dado_ValorDateTime_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "datetime-key";
            var value = DateTime.UtcNow;

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ValorGuid_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "guid-key";
            var value = Guid.NewGuid();

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ListaDeObjetos_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "list-key";
            var value = new List<object> { "item1", 2, true };

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ValorNulo_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var key = "null-value-key";

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, null));
        }

        [Fact]
        public async Task Dado_ValorAsync_Quando_SetAsync_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "async-key";
            var value = "async-value";

            // Quando
            await _cache.SetAsync(key, value);

            // Então
            await _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<System.Threading.CancellationToken>());
        }
    }
}
