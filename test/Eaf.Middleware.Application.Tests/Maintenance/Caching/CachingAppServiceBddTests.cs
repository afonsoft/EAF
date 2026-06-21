using Abp.Application.Services.Dto;
using Abp.Runtime.Caching;
using Eaf.Middleware.Maintenance.Caching;
using Eaf.Middleware.Maintenance.Caching.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Maintenance.Caching
{
    /// <summary>
    /// Testes BDD para CachingAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class CachingAppServiceBddTests
    {
        private readonly ICacheManager _cacheManager;
        private readonly CachingAppService _sut;

        public CachingAppServiceBddTests()
        {
            _cacheManager = Substitute.For<ICacheManager>();
            _sut = new CachingAppService(_cacheManager);
        }

        #region GetAllCaches

        [Fact]
        public void Dado_CachesExistentes_Quando_GetAllCaches_Entao_DeveRetornarListaDeCaches()
        {
            // Dado
            var cache1 = Substitute.For<ICache>();
            cache1.Name.Returns("Cache1");
            var cache2 = Substitute.For<ICache>();
            cache2.Name.Returns("Cache2");
            _cacheManager.GetAllCaches().Returns(new[] { cache1, cache2 });

            // Quando
            var result = _sut.GetAllCaches();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(2);
            result.Items[0].Name.ShouldBe("Cache1");
            result.Items[1].Name.ShouldBe("Cache2");
        }

        [Fact]
        public void Dado_NenhumCache_Quando_GetAllCaches_Entao_DeveRetornarListaVazia()
        {
            // Dado
            _cacheManager.GetAllCaches().Returns(new ICache[0]);

            // Quando
            var result = _sut.GetAllCaches();

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        #endregion

        #region ClearCache

        [Fact]
        public async Task Dado_CacheExistente_Quando_ClearCache_Entao_DeveLimparCacheEspecifico()
        {
            // Dado
            var cache = Substitute.For<ICache>();
            _cacheManager.GetCache("MeuCache").Returns(cache);

            // Quando
            await _sut.ClearCache(new EntityDto<string>("MeuCache"));

            // Então
            await cache.Received(1).ClearAsync();
        }

        #endregion

        #region ClearAllCaches

        [Fact]
        public async Task Dado_MultiploCaches_Quando_ClearAllCaches_Entao_DeveLimparTodos()
        {
            // Dado
            var cache1 = Substitute.For<ICache>();
            var cache2 = Substitute.For<ICache>();
            var cache3 = Substitute.For<ICache>();
            _cacheManager.GetAllCaches().Returns(new[] { cache1, cache2, cache3 });

            // Quando
            await _sut.ClearAllCaches();

            // Então
            await cache1.Received(1).ClearAsync();
            await cache2.Received(1).ClearAsync();
            await cache3.Received(1).ClearAsync();
        }

        [Fact]
        public async Task Dado_NenhumCache_Quando_ClearAllCaches_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            _cacheManager.GetAllCaches().Returns(new ICache[0]);

            // Quando / Então
            await Should.NotThrowAsync(() => _sut.ClearAllCaches());
        }

        #endregion

        #region Construtor

        [Fact]
        public void Dado_CacheManager_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            // Dado / Quando
            var sut = new CachingAppService(_cacheManager);

            // Então
            sut.ShouldNotBeNull();
        }

        #endregion
    }
}
