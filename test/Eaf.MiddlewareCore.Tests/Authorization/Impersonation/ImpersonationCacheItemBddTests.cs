using Eaf.Middleware.Authorization.Impersonation;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Impersonation
{
    /// <summary>
    /// Testes BDD para ImpersonationCacheItem seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ImpersonationCacheItemBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveInicializarComValoresPadrao()
        {
            var item = new ImpersonationCacheItem();
            item.TargetTenantId.ShouldBeNull();
            item.TargetUserId.ShouldBe(0);
            item.IsBackToImpersonator.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ConstrutorComParametros_Quando_Criar_Entao_DeveDefinirValores()
        {
            var item = new ImpersonationCacheItem(1, 100, true);
            item.TargetTenantId.ShouldBe(1);
            item.TargetUserId.ShouldBe(100);
            item.IsBackToImpersonator.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ConstrutorComTenantNull_Quando_Criar_Entao_TenantDeveSerNull()
        {
            var item = new ImpersonationCacheItem(null, 50, false);
            item.TargetTenantId.ShouldBeNull();
            item.TargetUserId.ShouldBe(50);
            item.IsBackToImpersonator.ShouldBeFalse();
        }

        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            ImpersonationCacheItem.CacheName.ShouldBe("AppImpersonationCache");
        }

        [Fact]
        public void Dado_Item_Quando_DefinirImpersonatorProperties_Entao_DeveArmazenar()
        {
            var item = new ImpersonationCacheItem
            {
                ImpersonatorTenantId = 5,
                ImpersonatorUserId = 200
            };

            item.ImpersonatorTenantId.ShouldBe(5);
            item.ImpersonatorUserId.ShouldBe(200);
        }
    }
}
