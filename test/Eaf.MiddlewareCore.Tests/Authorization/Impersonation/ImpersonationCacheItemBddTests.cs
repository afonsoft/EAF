using Eaf.Middleware.Authorization.Impersonation;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Impersonation
{
    /// <summary>
    /// Testes BDD para ImpersonationCacheItem seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ImpersonationCacheItemBddTests
    {
        #region CacheName

        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveSerAppImpersonationCache()
        {
            ImpersonationCacheItem.CacheName.ShouldBe("AppImpersonationCache");
        }

        #endregion

        #region Construtor Padrao

        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarInstancia_Entao_DeveInicializarComDefaults()
        {
            var sut = new ImpersonationCacheItem();
            sut.TargetTenantId.ShouldBeNull();
            sut.TargetUserId.ShouldBe(0);
            sut.IsBackToImpersonator.ShouldBeFalse();
            sut.ImpersonatorTenantId.ShouldBeNull();
            sut.ImpersonatorUserId.ShouldBe(0);
        }

        #endregion

        #region Construtor com Parametros

        [Fact]
        public void Dado_Parametros_Quando_CriarInstancia_Entao_DeveAtribuirTargetTenantId()
        {
            var sut = new ImpersonationCacheItem(1, 100, false);
            sut.TargetTenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_ParametrosComTenantNull_Quando_CriarInstancia_Entao_TargetTenantIdDeveSerNull()
        {
            var sut = new ImpersonationCacheItem(null, 200, true);
            sut.TargetTenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_Parametros_Quando_CriarInstancia_Entao_DeveAtribuirTargetUserId()
        {
            var sut = new ImpersonationCacheItem(1, 300, false);
            sut.TargetUserId.ShouldBe(300);
        }

        [Fact]
        public void Dado_IsBackTrue_Quando_CriarInstancia_Entao_IsBackToImpersonatorDeveSerTrue()
        {
            var sut = new ImpersonationCacheItem(1, 100, true);
            sut.IsBackToImpersonator.ShouldBeTrue();
        }

        [Fact]
        public void Dado_IsBackFalse_Quando_CriarInstancia_Entao_IsBackToImpersonatorDeveSerFalse()
        {
            var sut = new ImpersonationCacheItem(1, 100, false);
            sut.IsBackToImpersonator.ShouldBeFalse();
        }

        #endregion

        #region Propriedades

        [Fact]
        public void Dado_Instancia_Quando_DefinirImpersonatorTenantId_Entao_DeveArmazenar()
        {
            var sut = new ImpersonationCacheItem { ImpersonatorTenantId = 5 };
            sut.ImpersonatorTenantId.ShouldBe(5);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirImpersonatorUserId_Entao_DeveArmazenar()
        {
            var sut = new ImpersonationCacheItem { ImpersonatorUserId = 42 };
            sut.ImpersonatorUserId.ShouldBe(42);
        }

        #endregion
    }
}
