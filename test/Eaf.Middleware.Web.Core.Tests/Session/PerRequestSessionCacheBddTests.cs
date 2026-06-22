using Eaf.Middleware.Sessions;
using Eaf.Middleware.Sessions.Dto;
using Eaf.Middleware.Web.Session;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Session
{
    /// <summary>
    /// Testes BDD para PerRequestSessionCache seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class PerRequestSessionCacheBddTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAppService _sessionAppService;
        private readonly PerRequestSessionCache _sut;

        public PerRequestSessionCacheBddTests()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _sessionAppService = Substitute.For<ISessionAppService>();
            _sut = new PerRequestSessionCache(_httpContextAccessor, _sessionAppService);
        }

        #region GetCurrentLoginInformationsAsync

        [Fact]
        public async Task Dado_HttpContextNulo_Quando_GetCurrentLoginInformations_Entao_DeveChamarSessionAppService()
        {
            // Dado
            _httpContextAccessor.HttpContext.Returns((HttpContext)null);
            var expected = new GetCurrentLoginInformationsOutput();
            _sessionAppService.GetCurrentLoginInformations().Returns(expected);

            // Quando
            var result = await _sut.GetCurrentLoginInformationsAsync();

            // Entao
            result.ShouldBe(expected);
            await _sessionAppService.Received(1).GetCurrentLoginInformations();
        }

        [Fact]
        public async Task Dado_HttpContextComCacheVazio_Quando_GetCurrentLoginInformations_Entao_DeveBuscarEArmazenarCache()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext.Returns(httpContext);

            var expected = new GetCurrentLoginInformationsOutput
            {
                User = new UserLoginInfoDto { Name = "Admin" }
            };
            _sessionAppService.GetCurrentLoginInformations().Returns(expected);

            // Quando
            var result = await _sut.GetCurrentLoginInformationsAsync();

            // Entao
            result.ShouldBe(expected);
            result.User.Name.ShouldBe("Admin");
            httpContext.Items["__PerRequestSessionCache"].ShouldBe(expected);
        }

        [Fact]
        public async Task Dado_HttpContextComCachePreenchido_Quando_GetCurrentLoginInformations_Entao_DeveRetornarDoCacheSemChamarServico()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            var cached = new GetCurrentLoginInformationsOutput
            {
                User = new UserLoginInfoDto { Name = "Cached" }
            };
            httpContext.Items["__PerRequestSessionCache"] = cached;
            _httpContextAccessor.HttpContext.Returns(httpContext);

            // Quando
            var result = await _sut.GetCurrentLoginInformationsAsync();

            // Entao
            result.ShouldBe(cached);
            result.User.Name.ShouldBe("Cached");
            await _sessionAppService.DidNotReceive().GetCurrentLoginInformations();
        }

        [Fact]
        public async Task Dado_HttpContextComUsuarioNulo_Quando_GetCurrentLoginInformations_Entao_NaoDeveArmazenarCache()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext.Returns(httpContext);

            var expected = new GetCurrentLoginInformationsOutput { User = null };
            _sessionAppService.GetCurrentLoginInformations().Returns(expected);

            // Quando
            var result = await _sut.GetCurrentLoginInformationsAsync();

            // Entao
            result.ShouldBe(expected);
            httpContext.Items.ContainsKey("__PerRequestSessionCache").ShouldBeFalse();
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion
    }
}
