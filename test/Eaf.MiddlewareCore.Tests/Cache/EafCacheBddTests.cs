using Eaf.Middleware.Core.Cache;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Cache
{
    /// <summary>
    /// Testes BDD para EafCache seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EafCacheBddTests
    {
        [Fact]
        public void Dado_EafCache_Quando_CriarComPropriedades_Entao_DeveArmazenar()
        {
            var cache = new EafCache
            {
                Id = "cache-key-1",
                Value = new byte[] { 1, 2, 3 },
                ExpiresAtTime = DateTimeOffset.UtcNow.AddHours(1),
                SlidingExpirationInSeconds = 300,
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };

            cache.Id.ShouldBe("cache-key-1");
            cache.Value.Length.ShouldBe(3);
            cache.SlidingExpirationInSeconds.ShouldBe(300);
            cache.AbsoluteExpiration.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EafCache_Quando_IsTransient_Entao_DeveRetornarTrue()
        {
            var cache = new EafCache();
            cache.IsTransient().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EafCache_Quando_SemSlidingExpiration_Entao_DeveSerNull()
        {
            var cache = new EafCache { Id = "key" };
            cache.SlidingExpirationInSeconds.ShouldBeNull();
        }

        [Fact]
        public void Dado_EafCache_Quando_SemAbsoluteExpiration_Entao_DeveSerNull()
        {
            var cache = new EafCache { Id = "key" };
            cache.AbsoluteExpiration.ShouldBeNull();
        }

        [Fact]
        public void Dado_EafCache_Quando_DefinirValue_Entao_DeveSerByteArray()
        {
            var data = new byte[] { 0xFF, 0x00, 0xAB };
            var cache = new EafCache { Id = "binary-key", Value = data };
            cache.Value.ShouldBe(data);
        }
    }
}
