#nullable disable
using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    public class MiddlewareControllerBaseBddTests
    {
        private sealed class TestableController : MiddlewareControllerBase
        {
            public string CallL(string name) => L(name);
            public string CallLWithArgs(string name, params object[] args) => L(name, args);
            public string CallLWithCulture(string name, System.Globalization.CultureInfo culture) => L(name, culture);
            public void CallSetTenantIdCookie(int? tenantId) => SetTenantIdCookie(tenantId);
            public void CallCheckErrors(IdentityResult identityResult) => CheckErrors(identityResult);
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

        [Fact]
        public void Dado_ChaveLocalizacao_Quando_L_Entao_DeveRetornarChaveComFallback()
        {
            // Dado
            var sut = new TestableController();

            // Quando
            var result = sut.CallL("ChaveInexistente_P25");

            // Então
            result.ShouldBe("ChaveInexistente_P25");
        }

        [Fact]
        public void Dado_ChaveEArgs_Quando_L_Entao_DeveRetornarChaveComArgumentos()
        {
            // Dado
            var sut = new TestableController();

            // Quando
            var result = sut.CallLWithArgs("Chave_{0}", "P25");

            // Então
            result.ShouldContain("P25");
        }

        [Fact]
        public void Dado_TenantId_Quando_SetTenantIdCookie_Entao_DeveAdicionarCookie()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            var sut = new TestableController
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Quando
            sut.CallSetTenantIdCookie(42);

            // Então
            httpContext.Response.Headers.ShouldContainKey("Set-Cookie");
        }

        [Fact]
        public void Dado_TenantIdNulo_Quando_SetTenantIdCookie_Entao_DeveLancarArgumentNullException()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            var sut = new TestableController
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Quando & Então
            Should.Throw<System.ArgumentNullException>(() => sut.CallSetTenantIdCookie(null));
        }

        [Fact]
        public void Dado_IdentityResultSucesso_Quando_CheckErrors_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            var sut = new TestableController();

            // Quando & Então
            Should.NotThrow(() => sut.CallCheckErrors(IdentityResult.Success));
        }

        [Fact]
        public void Dado_IdentityResultFalha_Quando_CheckErrors_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = new TestableController();
            var failedResult = IdentityResult.Failed(new IdentityError { Description = "Erro P25" });

            // Quando & Então
            Should.Throw<Abp.UI.UserFriendlyException>(() => sut.CallCheckErrors(failedResult));
        }

        [Fact]
        public void Dado_ChaveECultura_Quando_L_Entao_DeveRetornarChaveLocalizada()
        {
            // Dado
            var sut = new TestableController();

            // Quando
            var result = sut.CallLWithCulture("ChaveInexistente_P52", System.Globalization.CultureInfo.InvariantCulture);

            // Então
            result.ShouldBe("ChaveInexistente_P52");
        }
    }
}
