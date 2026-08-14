using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Eaf.Runtime.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.RedisCache.Tests
{
    /// <summary>
    /// Testes BDD em português para EafRedisCacheManager.
    /// </summary>
    public class EafRedisCacheManagerTests
    {
        private readonly ICachingConfiguration _mockConfiguration;

        public EafRedisCacheManagerTests()
        {
            _mockConfiguration = Substitute.For<ICachingConfiguration>();
        }

        [Fact]
        public void Dado_IocManagerNulo_Quando_CriarInstancia_Entao_DeveLancarNullReferenceException()
        {
            // Dado & Quando & Então
            Should.Throw<NullReferenceException>(() => new EafRedisCacheManager(null, _mockConfiguration));
        }

        [Fact]
        public void Dado_ConfiguracaoNula_Quando_CriarInstancia_Entao_DevePermitirCriacao()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();

            // Quando
            var manager = new EafRedisCacheManager(mockIocManager, null);

            // Então
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveRetornarInstanciaNaoNula()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();

            // Quando
            var manager = new EafRedisCacheManager(mockIocManager, _mockConfiguration);

            // Então
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_GetCacheChamado_Quando_CacheNaoExiste_Entao_DeveCriarNovaInstancia()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();

            mockIocManager.Resolve<EafRedisCache>(Arg.Any<object>()).Returns(new EafRedisCache("test-cache", distributedCache, options));
            var manager = new EafRedisCacheManager(mockIocManager, _mockConfiguration);

            // Quando
            var cache = manager.GetCache("test-cache");

            // Então
            cache.ShouldNotBeNull();
            cache.Name.ShouldBe("test-cache");
        }

        [Fact]
        public void Dado_GetCacheChamadoDuasVezes_Quando_MesmaChave_Entao_DeveRetornarMesmaInstancia()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();
            var cacheInstance = new EafRedisCache("test-cache", distributedCache, options);
            mockIocManager.Resolve<EafRedisCache>(Arg.Any<object>()).Returns(cacheInstance);
            var manager = new EafRedisCacheManager(mockIocManager, _mockConfiguration);

            // Quando
            var cache1 = manager.GetCache("test-cache");
            var cache2 = manager.GetCache("test-cache");

            // Então
            cache1.ShouldBe(cache2);
        }

        [Fact]
        public void Dado_GetAllCachesChamado_Quando_CachesExistem_Entao_DeveRetornarLista()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();
            var cache1 = new EafRedisCache("cache1", distributedCache, options);
            var cache2 = new EafRedisCache("cache2", distributedCache, options);
            mockIocManager.Resolve<EafRedisCache>(Arg.Any<object>()).Returns(cache1, cache2);
            var manager = new EafRedisCacheManager(mockIocManager, _mockConfiguration);

            // Quando
            manager.GetCache("cache1");
            manager.GetCache("cache2");
            var caches = manager.GetAllCaches();

            // Então
            caches.ShouldNotBeNull();
            caches.Count.ShouldBeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public void Dado_DisposeChamado_Quando_CachesAtivos_Entao_DeveLimparRecursos()
        {
            // Dado
            var mockIocManager = Substitute.For<IIocManager>();
            var distributedCache = Substitute.For<IDistributedCache>();
            var options = new EafRedisCacheOptions();
            mockIocManager.Resolve<EafRedisCache>(Arg.Any<object>()).Returns(new EafRedisCache("test-cache", distributedCache, options));
            var manager = new EafRedisCacheManager(mockIocManager, _mockConfiguration);
            manager.GetCache("test-cache");

            // Quando & Então
            Should.NotThrow(() => manager.Dispose());
        }
    }
}
