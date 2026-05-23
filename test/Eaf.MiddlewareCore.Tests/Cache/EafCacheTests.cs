using Eaf.Middleware.Core.Cache;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Cache
{
    public class EafCacheTests
    {
        [Fact]
        public void Dado_EafCache_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var now = DateTimeOffset.UtcNow;
            var cache = new EafCache
            {
                Id = "cache-key-1",
                Value = new byte[] { 1, 2, 3 },
                ExpiresAtTime = now,
                SlidingExpirationInSeconds = 300,
                AbsoluteExpiration = now.AddHours(1)
            };

            cache.Id.ShouldBe("cache-key-1");
            cache.Value.ShouldBe(new byte[] { 1, 2, 3 });
            cache.ExpiresAtTime.ShouldBe(now);
            cache.SlidingExpirationInSeconds.ShouldBe(300);
            cache.AbsoluteExpiration.ShouldBe(now.AddHours(1));
        }

        [Fact]
        public void Dado_EafCache_Quando_IsTransient_Entao_DeveRetornarTrue()
        {
            var cache = new EafCache();
            cache.IsTransient().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EafCache_Quando_PropriedadesNull_Entao_DeveRetornarNull()
        {
            var cache = new EafCache();
            cache.SlidingExpirationInSeconds.ShouldBeNull();
            cache.AbsoluteExpiration.ShouldBeNull();
        }
    }
}
