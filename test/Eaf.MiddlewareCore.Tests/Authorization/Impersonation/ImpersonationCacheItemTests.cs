using Eaf.Middleware.Authorization.Impersonation;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.Impersonation
{
    public class ImpersonationCacheItemTests
    {
        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveSerAppImpersonationCache()
        {
            ImpersonationCacheItem.CacheName.ShouldBe("AppImpersonationCache");
        }

        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveSerPadrao()
        {
            var item = new ImpersonationCacheItem();
            item.TargetTenantId.ShouldBeNull();
            item.TargetUserId.ShouldBe(0);
            item.IsBackToImpersonator.ShouldBeFalse();
            item.ImpersonatorTenantId.ShouldBeNull();
            item.ImpersonatorUserId.ShouldBe(0);
        }

        [Fact]
        public void Dado_ConstrutorComParametros_Quando_Criar_Entao_DeveDefinirValores()
        {
            var item = new ImpersonationCacheItem(2, 42, true);
            item.TargetTenantId.ShouldBe(2);
            item.TargetUserId.ShouldBe(42);
            item.IsBackToImpersonator.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ConstrutorComParametros_Quando_TenantIdNulo_Entao_DeveAceitarNull()
        {
            var item = new ImpersonationCacheItem(null, 10, false);
            item.TargetTenantId.ShouldBeNull();
            item.TargetUserId.ShouldBe(10);
            item.IsBackToImpersonator.ShouldBeFalse();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirImpersonatorProperties_Entao_DeveArmazenar()
        {
            var item = new ImpersonationCacheItem
            {
                ImpersonatorTenantId = 1,
                ImpersonatorUserId = 99
            };
            item.ImpersonatorTenantId.ShouldBe(1);
            item.ImpersonatorUserId.ShouldBe(99);
        }
    }
}
