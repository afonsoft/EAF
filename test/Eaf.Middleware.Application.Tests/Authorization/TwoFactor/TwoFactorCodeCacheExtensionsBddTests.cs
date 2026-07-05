using Abp.Runtime.Caching;
using Eaf.Middleware.Authorization.TwoFactor;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    /// <summary>
    /// Testes BDD para TwoFactorCodeCacheExtensions seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class TwoFactorCodeCacheExtensionsBddTests
    {
        [Fact]
        public void Dado_ClasseDeExtensao_Quando_VerificarTipo_Entao_DeveSerEstatica()
        {
            var tipo = typeof(TwoFactorCodeCacheExtensions);
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
        }

        [Fact]
        public void Dado_CacheManager_Quando_GetTwoFactorCodeCache_Entao_DeveRetornarCacheTipado()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache(TwoFactorCodeCacheItem.CacheName).Returns(cache);

            var result = cacheManager.GetTwoFactorCodeCache();

            result.ShouldNotBeNull();
            cacheManager.Received(1).GetCache(TwoFactorCodeCacheItem.CacheName);
        }
    }
}
