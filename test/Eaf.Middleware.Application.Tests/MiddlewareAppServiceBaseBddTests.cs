using Abp.Application.Services;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    /// <summary>
    /// Testes BDD para MiddlewareAppServiceBase seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class MiddlewareAppServiceBaseBddTests
    {
        private sealed class TestableAppService : MiddlewareAppServiceBase
        {
        }

        [Fact]
        public void Dado_TipoMiddlewareAppServiceBase_Quando_Verificar_Entao_DeveSerAbstrato()
        {
            typeof(MiddlewareAppServiceBase).IsAbstract.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Subclasse_Quando_Criar_Entao_DeveSerApplicationService()
        {
            var sut = new TestableAppService();

            sut.ShouldBeAssignableTo<ApplicationService>();
        }

        [Fact]
        public void Dado_Subclasse_Quando_DefinirManagers_Entao_DeveArmazenar()
        {
            var sut = new TestableAppService
            {
                TenantManager = null,
                UserManager = null
            };

            sut.TenantManager.ShouldBeNull();
            sut.UserManager.ShouldBeNull();
        }
    }
}
