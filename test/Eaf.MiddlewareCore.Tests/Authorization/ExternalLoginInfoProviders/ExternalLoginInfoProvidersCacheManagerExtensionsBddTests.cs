using Abp.Runtime.Caching;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.ExternalLoginInfoProviders
{
    /// <summary>
    /// Testes BDD para ExternalLoginInfoProvidersCacheManagerExtensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ExternalLoginInfoProvidersCacheManagerExtensionsBddTests
    {
        #region GetExternalLoginInfoProviderCache

        [Fact]
        public void Dado_CacheManager_Quando_GetExternalLoginInfoProviderCache_Entao_DeveRetornarCache()
        {
            // Dado
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache("AppExternalLoginInfoProvidersCache").Returns(cache);

            // Quando
            var result = cacheManager.GetExternalLoginInfoProviderCache();

            // Entao
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CacheManager_Quando_GetExternalLoginInfoProviderCache_Entao_DeveChamarGetCacheComNomeCorreto()
        {
            // Dado
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache(Arg.Any<string>()).Returns(cache);

            // Quando
            cacheManager.GetExternalLoginInfoProviderCache();

            // Entao
            cacheManager.Received(1).GetCache("AppExternalLoginInfoProvidersCache");
        }

        #endregion
    }
}
