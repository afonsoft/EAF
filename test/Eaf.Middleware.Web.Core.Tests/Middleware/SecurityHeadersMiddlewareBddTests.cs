using Eaf.Middleware.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Tests.Middleware
{
    /// <summary>
    /// Testes BDD para SecurityHeadersMiddleware seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class SecurityHeadersMiddlewareBddTests
    {
        [Fact]
        public async Task Dado_RequisicaoHttp_Quando_Invocar_Entao_DeveAdicionarHeadersDeSegurancaSemHsts()
        {
            // Dado
            var proximoChamado = false;
            var middleware = new SecurityHeadersMiddleware(_ =>
            {
                proximoChamado = true;
                return Task.CompletedTask;
            });
            var context = new DefaultHttpContext();

            // Quando
            await middleware.Invoke(context);

            // Então
            proximoChamado.ShouldBeTrue();
            context.Response.Headers["X-Content-Type-Options"].ToString().ShouldBe("nosniff");
            context.Response.Headers["X-Frame-Options"].ToString().ShouldBe("DENY");
            context.Response.Headers["X-XSS-Protection"].ToString().ShouldBe("1; mode=block");
            context.Response.Headers["Referrer-Policy"].ToString().ShouldBe("strict-origin-when-cross-origin");
            context.Response.Headers["Permissions-Policy"].ToString().ShouldContain("camera=()");
            context.Response.Headers.ContainsKey("Strict-Transport-Security").ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_RequisicaoHttps_Quando_Invocar_Entao_DeveAdicionarHsts()
        {
            // Dado
            var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";

            // Quando
            await middleware.Invoke(context);

            // Então
            context.Response.Headers["Strict-Transport-Security"].ToString()
                .ShouldBe("max-age=31536000; includeSubDomains");
        }

        [Fact]
        public async Task Dado_HeaderJaExistente_Quando_Invocar_Entao_NaoDeveSobrescrever()
        {
            // Dado
            var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";

            // Quando
            await middleware.Invoke(context);

            // Então
            context.Response.Headers["X-Frame-Options"].ToString().ShouldBe("SAMEORIGIN");
        }
    }
}
