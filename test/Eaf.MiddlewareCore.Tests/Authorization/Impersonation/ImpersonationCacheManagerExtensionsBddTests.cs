using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.Impersonation;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Impersonation
{
    /// <summary>
    /// Testes BDD para ImpersonationCacheManagerExtensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ImpersonationCacheManagerExtensionsBddTests
    {
        #region GetImpersonationCache

        [Fact]
        public void Dado_CacheManager_Quando_GetImpersonationCache_Entao_DeveRetornarCache()
        {
            // Dado
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache(ImpersonationCacheItem.CacheName).Returns(cache);

            // Quando
            var result = cacheManager.GetImpersonationCache();

            // Entao
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CacheManager_Quando_GetImpersonationCache_Entao_DeveChamarGetCacheComNomeCorreto()
        {
            // Dado
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache(Arg.Any<string>()).Returns(cache);

            // Quando
            cacheManager.GetImpersonationCache();

            // Entao
            cacheManager.Received(1).GetCache(ImpersonationCacheItem.CacheName);
        }

        #endregion
    }
}
