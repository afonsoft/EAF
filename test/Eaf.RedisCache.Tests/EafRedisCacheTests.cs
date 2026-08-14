using Eaf.Runtime.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes BDD em português para EafRedisCache.
    /// </summary>
    public class EafRedisCacheTests
    {
        private static int _cacheCounter;

        private static string GetUniqueCacheName()
        {
            return $"test-cache-{Interlocked.Increment(ref _cacheCounter)}";
        }

        private (IDistributedCache Cache, EafRedisCacheOptions Options, EafRedisCache RedisCache) CreateCache(string? cacheName = null, IConnectionMultiplexer? multiplexer = null)
        {
            cacheName ??= GetUniqueCacheName();

            var store = new Dictionary<string, byte[]>();
            var distributedCache = Substitute.For<IDistributedCache>();

            distributedCache.Get(Arg.Any<string>())
                .Returns(callInfo => store.TryGetValue(callInfo.ArgAt<string>(0), out var value) ? value : null);

            distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult<byte[]?>(store.TryGetValue(callInfo.ArgAt<string>(0), out var value) ? value : null));

            distributedCache.When(x => x.Set(
                    Arg.Any<string>(),
                    Arg.Any<byte[]>(),
                    Arg.Any<DistributedCacheEntryOptions>()))
                .Do(callInfo => store[callInfo.ArgAt<string>(0)] = callInfo.ArgAt<byte[]>(1));

            distributedCache.SetAsync(
                    Arg.Any<string>(),
                    Arg.Any<byte[]>(),
                    Arg.Any<DistributedCacheEntryOptions>(),
                    Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    store[callInfo.ArgAt<string>(0)] = callInfo.ArgAt<byte[]>(1);
                    return Task.CompletedTask;
                });

            distributedCache.When(x => x.Remove(Arg.Any<string>()))
                .Do(callInfo => store.Remove(callInfo.ArgAt<string>(0)));

            distributedCache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    store.Remove(callInfo.ArgAt<string>(0));
                    return Task.CompletedTask;
                });

            var options = new EafRedisCacheOptions();
            var redisCache = new EafRedisCache(cacheName, distributedCache, options, multiplexer);

            return (distributedCache, options, redisCache);
        }

        [Fact]
        public void Dado_CacheConfigurado_Quando_GravarELer_Entao_DeveRetornarValor()
        {
            // Dado
            var (_, _, cache) = CreateCache();

            // Quando
            cache.Set("key", "value");
            var result = cache.TryGetValue("key", out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldBe("value");
        }

        [Fact]
        public async Task Dado_CacheConfigurado_Quando_GravarELerAsync_Entao_DeveRetornarValor()
        {
            // Dado
            var (_, _, cache) = CreateCache();

            // Quando
            await cache.SetAsync("key", "value");
            var result = await cache.TryGetValueAsync("key");

            // Então
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBe("value");
        }

        [Fact]
        public void Dado_ChaveInexistente_Quando_Ler_Entao_DeveRetornarFalse()
        {
            // Dado
            var (_, _, cache) = CreateCache();

            // Quando
            var result = cache.TryGetValue("missing-key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Dado_ItemExistente_Quando_Remover_Entao_DeveNaoExistirMais()
        {
            // Dado
            var (_, _, cache) = CreateCache();
            cache.Set("key", "value");

            // Quando
            cache.Remove("key");
            var result = cache.TryGetValue("key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Dado_CacheComItens_Quando_Limpar_Entao_DeveRemoverPeloPrefixo()
        {
            // Dado
            var endpoint = new DnsEndPoint("localhost", 6379);
            var multiplexer = Substitute.For<IConnectionMultiplexer>();
            var server = Substitute.For<IServer>();
            var database = Substitute.For<IDatabase>();

            multiplexer.GetEndPoints().Returns(new EndPoint[] { endpoint });
            multiplexer.GetServer(endpoint).Returns(server);
            multiplexer.GetDatabase().Returns(database);
            server.Keys(pattern: Arg.Any<RedisValue>(), pageSize: Arg.Any<int>())
                .Returns(new RedisKey[] { "EAF:test-cache_Clear_key1", "EAF:test-cache_Clear_key2" });

            var (_, _, cache) = CreateCache("test-cache_Clear", multiplexer);
            cache.Set("key1", "value1");
            cache.Set("key2", "value2");

            // Quando
            cache.Clear();

            // Então
            database.Received(1).KeyDelete(Arg.Any<RedisKey[]>());
        }

        [Fact]
        public void Dado_SemConexaoRedis_Quando_Limpar_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var (_, _, cache) = CreateCache();

            // Quando & Então
            Should.NotThrow(() => cache.Clear());
        }

        [Fact]
        public void Dado_ExpiracaoSlidingCustom_Quando_Set_Entao_DevePassarParaDistributedCache()
        {
            // Dado
            var (distributedCache, _, cache) = CreateCache();
            var slidingExpiration = TimeSpan.FromMinutes(30);

            // Quando
            cache.Set("custom-sliding", "value", slidingExpiration);

            // Então
            distributedCache.Received(1).Set(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == slidingExpiration));
        }

        [Fact]
        public void Dado_ExpiracaoAbsoluteCustom_Quando_Set_Entao_DevePassarParaDistributedCache()
        {
            // Dado
            var (distributedCache, _, cache) = CreateCache();
            var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(2);

            // Quando
            cache.Set("custom-absolute", "value", null, absoluteExpiration);

            // Então
            distributedCache.Received(1).Set(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpiration == absoluteExpiration));
        }

        [Fact]
        public void Dado_ValorComplexo_Quando_GravarELer_Entao_DeveSerializarEDesserializar()
        {
            // Dado
            var (_, _, cache) = CreateCache();
            var complexValue = new Dictionary<string, object>
            {
                { "string", "test" },
                { "number", 42 },
                { "boolean", true },
                { "date", DateTime.UtcNow }
            };

            // Quando
            cache.Set("complex-key", complexValue);
            var result = cache.TryGetValue("complex-key", out var value);

            // Então
            result.ShouldBeTrue();
            value.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_DistributedCacheQueLancaExcecaoNoSet_Quando_Set_Entao_DeveCapturarErro()
        {
            // Dado
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();
            var cache = new EafRedisCache("test-cache", distributedCache, options);

            distributedCache.When(x => x.Set(
                    Arg.Any<string>(),
                    Arg.Any<byte[]>(),
                    Arg.Any<DistributedCacheEntryOptions>()))
                .Do(_ => throw new InvalidOperationException("Redis connection failed"));

            // Quando & Então
            Should.NotThrow(() => cache.Set("fail-key", "value"));
        }

        [Fact]
        public void Dado_DistributedCacheQueLancaExcecaoNoGet_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();
            var cache = new EafRedisCache("test-cache", distributedCache, options);

            distributedCache.Get(Arg.Any<string>()).Returns(_ => throw new TimeoutException("Redis timeout"));

            // Quando
            var result = cache.TryGetValue("timeout-key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }
    }
}
