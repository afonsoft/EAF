using System;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Sqlite;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    /// <summary>
    /// Testes BDD em português para EafSqliteCacheManager
    /// </summary>
    public class EafSqliteCacheManagerTests
    {
        private readonly ICachingConfiguration _mockConfiguration;

        public EafSqliteCacheManagerTests()
        {
            _mockConfiguration = Substitute.For<ICachingConfiguration>();
        }

        [Fact]
        public void Dado_IocManagerNulo_Quando_CriarInstancia_Entao_DeveLancarNullReferenceException()
        {
            // Dado & Quando & Então
            // O construtor atual não valida parâmetros nulos, então lança NullReferenceException
            Should.Throw<NullReferenceException>(() => new EafSqliteCacheManager(null, _mockConfiguration));
        }

        [Fact]
        public void Dado_ConfiguracaoNula_Quando_CriarInstancia_Entao_DevePermitirCriacao()
        {
            // Dado
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();

            // Quando
            var manager = new EafSqliteCacheManager(mockIocManager, null);

            // Então
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveRetornarInstanciaNaoNula()
        {
            // Dado
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();

            // Quando
            var manager = new EafSqliteCacheManager(mockIocManager, _mockConfiguration);

            // Então
            manager.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_GetCacheChamado_Quando_CacheNaoExiste_Entao_DeveCriarNovaInstancia()
        {
            // Dado
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            mockIocManager.Resolve<EafSqliteCache>(Arg.Any<object>()).Returns(new EafSqliteCache("test-cache", options));
            var manager = new EafSqliteCacheManager(mockIocManager, _mockConfiguration);

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
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            var cacheInstance = new EafSqliteCache("test-cache", options);
            mockIocManager.Resolve<EafSqliteCache>(Arg.Any<object>()).Returns(cacheInstance);
            var manager = new EafSqliteCacheManager(mockIocManager, _mockConfiguration);

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
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            var cache1 = new EafSqliteCache("cache1", options);
            var cache2 = new EafSqliteCache("cache2", options);
            mockIocManager.Resolve<EafSqliteCache>(Arg.Any<object>()).Returns(cache1, cache2);
            var manager = new EafSqliteCacheManager(mockIocManager, _mockConfiguration);

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
            var mockIocManager = Substitute.For<Abp.Dependency.IIocManager>();
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            mockIocManager.Resolve<EafSqliteCache>(Arg.Any<object>()).Returns(new EafSqliteCache("test-cache", options));
            var manager = new EafSqliteCacheManager(mockIocManager, _mockConfiguration);
            manager.GetCache("test-cache");

            // Quando & Então
            Should.NotThrow(() => manager.Dispose());
        }
    }
}