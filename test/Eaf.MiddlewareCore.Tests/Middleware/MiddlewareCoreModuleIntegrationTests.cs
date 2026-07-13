using Abp;
using Abp.Dependency;
using Eaf.Middleware;
using Eaf.Middleware.Timing;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware
{
    public class MiddlewareCoreModuleIntegrationTests
    {
        private static Abp.AbpBootstrapper CriarBootstrapperIsolado()
        {
            return Abp.AbpBootstrapper.Create(typeof(MiddlewareCoreModuleIntegrationTestModule), options =>
            {
                options.IocManager = new IocManager();
            });
        }

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_InicializarAbpBootstrapper_Entao_DeveCompletarSemErros()
        {
            var bootstrapper = CriarBootstrapperIsolado();
            Should.NotThrow(() => bootstrapper.Initialize());
            bootstrapper.IocManager.IsRegistered<AppTimes>().ShouldBeTrue();
            bootstrapper.Dispose();
        }

        [Fact]
        public void Dado_MiddlewareCoreModule_Quando_VerificarCacheDeAmigos_Entao_DeveEstarConfiguradoComExpiracao()
        {
            var bootstrapper = CriarBootstrapperIsolado();
            bootstrapper.Initialize();

            var cacheManager = bootstrapper.IocManager.Resolve<Abp.Runtime.Caching.ICacheManager>();
            var cache = cacheManager.GetCache(Eaf.Middleware.Friendships.Cache.FriendCacheItem.CacheName);
            cache.ShouldNotBeNull();
            cache.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromMinutes(30));

            bootstrapper.Dispose();
        }
    }
}
