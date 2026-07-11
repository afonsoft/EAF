using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para AntiForgeryController seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AntiForgeryControllerBddTests
    {
        private readonly IAntiforgery _antiforgery;
        private readonly AntiForgeryController _sut;

        public AntiForgeryControllerBddTests()
        {
            _antiforgery = Substitute.For<IAntiforgery>();
            _sut = new AntiForgeryController(_antiforgery);
        }

        #region Instanciacao

        [Fact]
        public void Dado_Antiforgery_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Antiforgery_Quando_GetToken_Entao_DeveChamarSetCookieTokenAndHeader()
        {
            // Dado
            _sut.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Quando
            _sut.GetToken();

            // Então
            _antiforgery.Received(1).SetCookieTokenAndHeader(Arg.Any<HttpContext>());
        }

        #endregion
    }
}
