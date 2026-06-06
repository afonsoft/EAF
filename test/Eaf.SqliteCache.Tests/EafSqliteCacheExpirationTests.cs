using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    /// <summary>
    /// Testes de expiração para EafSqliteCache.
    /// Verifica que slidingExpireTime e absoluteExpireTime são corretamente
    /// repassados ao CreateForSet (fix do bug identificado no code review).
    /// </summary>
    public class EafSqliteCacheExpirationTests
    {
        private static int _cacheCounter = 2000;

        private string GetUniqueCacheName()
        {
            return $"expiration-test-{Interlocked.Increment(ref _cacheCounter)}";
        }

        private EafSqliteCache CreateInMemoryCache()
        {
            return new EafSqliteCache(GetUniqueCacheName(), new EafSqliteCacheOptions { MemoryOnly = true });
        }

        #region Absolute Expiration

        [Fact]
        public void Dado_AbsoluteExpireTimeNoPassadoComSlidingCurto_Quando_SetEGet_Entao_NaoDeveRetornarItem()
        {
            // Dado — expiração absoluta no passado COM sliding curto
            // (a lógica soma sliding ao absolute: past + 1ms = past)
            using var cache = CreateInMemoryCache();
            var key = "absolute-past-key";
            var pastExpiry = DateTimeOffset.UtcNow.AddSeconds(-10);
            var shortSliding = TimeSpan.FromMilliseconds(1);

            // Quando
            cache.Set(key, "valor-expirado", slidingExpireTime: shortSliding, absoluteExpireTime: pastExpiry);
            var found = cache.TryGetValue(key, out var result);

            // Então — absolute(-10s) + sliding(1ms) = passado, item não encontrado
            found.ShouldBeFalse();
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_AbsoluteExpireTimeNoFuturo_Quando_SetEGet_Entao_DeveRetornarItem()
        {
            // Dado — expiração absoluta no futuro
            using var cache = CreateInMemoryCache();
            var key = "absolute-future-key";
            var futureExpiry = DateTimeOffset.UtcNow.AddHours(1);

            // Quando
            cache.Set(key, "valor-valido", absoluteExpireTime: futureExpiry);
            var found = cache.TryGetValue(key, out var result);

            // Então — item ainda válido
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_AbsoluteExpireTimeCurto_Quando_Esperar_Entao_ItemDeveExpirar()
        {
            // Dado — absolute em 1s + sliding de 1ms = expiry em ~1s
            using var cache = CreateInMemoryCache();
            var key = "absolute-short-key";
            var shortExpiry = DateTimeOffset.UtcNow.AddSeconds(1);
            var tinySliding = TimeSpan.FromMilliseconds(1);

            // Quando
            cache.Set(key, "valor-temporario", slidingExpireTime: tinySliding, absoluteExpireTime: shortExpiry);

            // Verificar que existe agora
            cache.TryGetValue(key, out _).ShouldBeTrue();

            // Esperar expirar
            await Task.Delay(1500);

            // Então — item deve ter expirado
            var found = cache.TryGetValue(key, out var result);
            found.ShouldBeFalse();
            result.ShouldBeNull();
        }

        #endregion

        #region Sliding Expiration

        [Fact]
        public void Dado_SlidingExpireTimePequeno_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado — sliding expiration de 100ms
            using var cache = CreateInMemoryCache();
            var key = "sliding-small-key";
            var sliding = TimeSpan.FromMilliseconds(100);

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, "valor", slidingExpireTime: sliding));
        }

        [Fact]
        public async Task Dado_SlidingExpireTimeCurto_Quando_Esperar_Entao_ItemDeveExpirar()
        {
            // Dado — sliding expiration de 1 segundo
            using var cache = CreateInMemoryCache();
            var key = "sliding-expire-key";
            var sliding = TimeSpan.FromSeconds(1);

            // Quando
            cache.Set(key, "valor-sliding", slidingExpireTime: sliding);

            // Verificar que existe agora
            cache.TryGetValue(key, out _).ShouldBeTrue();

            // Esperar expirar
            await Task.Delay(1500);

            // Então — item deve ter expirado
            var found = cache.TryGetValue(key, out var result);
            found.ShouldBeFalse();
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_SlidingExpireTimeGrande_Quando_SetEGet_Entao_DeveRetornarItem()
        {
            // Dado — sliding expiration longa (1 hora)
            using var cache = CreateInMemoryCache();
            var key = "sliding-large-key";
            var sliding = TimeSpan.FromHours(1);

            // Quando
            cache.Set(key, "valor-longo", slidingExpireTime: sliding);
            var found = cache.TryGetValue(key, out var result);

            // Então — item deve estar acessível
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        #endregion

        #region Combinação Sliding + Absolute

        [Fact]
        public void Dado_SlidingEAbsolute_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado — ambos os parâmetros
            using var cache = CreateInMemoryCache();
            var key = "combined-key";
            var sliding = TimeSpan.FromMinutes(5);
            var absolute = DateTimeOffset.UtcNow.AddHours(1);

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, "valor-combinado",
                slidingExpireTime: sliding, absoluteExpireTime: absolute));
        }

        [Fact]
        public void Dado_SlidingEAbsoluteNoFuturo_Quando_SetEGet_Entao_DeveRetornarItem()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "combined-future-key";
            var sliding = TimeSpan.FromMinutes(30);
            var absolute = DateTimeOffset.UtcNow.AddHours(2);

            // Quando
            cache.Set(key, "valor-combo", slidingExpireTime: sliding, absoluteExpireTime: absolute);
            var found = cache.TryGetValue(key, out var result);

            // Então
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_AbsoluteNoPassadoComSlidingGrande_Quando_SetEGet_Entao_DeveRetornarItem()
        {
            // Dado — lógica: expiry = absolute + sliding
            // absolute(-5s) + sliding(1h) = futuro
            using var cache = CreateInMemoryCache();
            var key = "past-absolute-combo-key";
            var sliding = TimeSpan.FromHours(1);
            var absolute = DateTimeOffset.UtcNow.AddSeconds(-5);

            // Quando
            cache.Set(key, "valor-combo",
                slidingExpireTime: sliding, absoluteExpireTime: absolute);
            var found = cache.TryGetValue(key, out var result);

            // Então — absolute(-5s) + sliding(1h) = futuro, item encontrado
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_AbsoluteNoPassadoComSlidingCurto_Quando_SetEGet_Entao_NaoDeveRetornarItem()
        {
            // Dado — absolute(-5s) + sliding(1ms) = passado
            using var cache = CreateInMemoryCache();
            var key = "past-absolute-short-sliding-key";
            var sliding = TimeSpan.FromMilliseconds(1);
            var absolute = DateTimeOffset.UtcNow.AddSeconds(-5);

            // Quando
            cache.Set(key, "valor-expirado",
                slidingExpireTime: sliding, absoluteExpireTime: absolute);
            var found = cache.TryGetValue(key, out var result);

            // Então — já expirou
            found.ShouldBeFalse();
        }

        #endregion

        #region Default Expiration (sem parâmetros)

        [Fact]
        public void Dado_SemParametrosDeExpiracao_Quando_Set_Entao_DeveUsarDefaultSlidingExpireTime()
        {
            // Dado — DefaultSlidingExpireTime é 60 minutos por padrão no ABP CacheBase
            using var cache = CreateInMemoryCache();
            var key = "default-expire-key";

            // Quando
            cache.Set(key, "valor-default");
            var found = cache.TryGetValue(key, out var result);

            // Então — com default de 60min, deve existir imediatamente
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        #endregion

        #region RemoveExpired

        [Fact]
        public async Task Dado_ItemExpirado_Quando_RemoveExpired_Entao_DeveRemoverDoCache()
        {
            // Dado — absolute(1s) + sliding(1ms) = expiry em ~1s
            using var cache = CreateInMemoryCache();
            var key = "removable-key";
            cache.Set(key, "valor-removivel",
                slidingExpireTime: TimeSpan.FromMilliseconds(1),
                absoluteExpireTime: DateTimeOffset.UtcNow.AddSeconds(1));

            // Verificar que existe
            cache.TryGetValue(key, out _).ShouldBeTrue();

            // Esperar expirar
            await Task.Delay(1500);

            // Quando
            cache.RemoveExpired();

            // Então
            cache.TryGetValue(key, out var result).ShouldBeFalse();
            result.ShouldBeNull();
        }

        #endregion
    }
}
