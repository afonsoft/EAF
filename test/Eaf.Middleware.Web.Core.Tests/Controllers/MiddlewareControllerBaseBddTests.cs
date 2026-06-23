using Eaf.Middleware.Web.Controllers;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para MiddlewareControllerBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class MiddlewareControllerBaseBddTests
    {
        #region Instanciacao via subclasse concreta

        private sealed class TestableController : MiddlewareControllerBase
        {
            public string CallL(string name) => L(name);
        }

        [Fact]
        public void Dado_SubclasseConcreta_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new TestableController();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_SubclasseConcreta_Quando_CriarInstancia_Entao_DeveHerdarDeAbpController()
        {
            var sut = new TestableController();
            sut.ShouldBeAssignableTo<Abp.AspNetCore.Mvc.Controllers.AbpController>();
        }

        #endregion
    }
}
