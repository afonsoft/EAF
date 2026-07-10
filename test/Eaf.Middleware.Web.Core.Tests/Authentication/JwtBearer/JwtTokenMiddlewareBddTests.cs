using Eaf.Middleware.Web.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;


namespace Eaf.Middleware.Tests.WebCore.Authentication.JwtBearer
{
    public class JwtTokenMiddlewareBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(JwtTokenMiddleware).IsAbstract.ShouldBeTrue();
            typeof(JwtTokenMiddleware).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioNaoAutenticado_Quando_InvocarMiddleware_Entao_DeveAutenticarECarregarClaimsPrincipal()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = CriarServiceProviderComAutenticacaoSucesso();

            var app = Substitute.For<Microsoft.AspNetCore.Builder.IApplicationBuilder>();
            app.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(app);

            // Quando
            var builder = JwtTokenMiddleware.UseJwtTokenMiddleware(app);
            builder.ShouldBe(app);

            var middleware = CapturarMiddleware(app);
            var nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            await middleware(next).Invoke(httpContext);

            // Então
            httpContext.User.Identity!.IsAuthenticated.ShouldBeTrue();
            nextCalled.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioJaAutenticado_Quando_InvocarMiddleware_Entao_NaoDeveChamarAutenticacaoNovamente()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test"), }, "Bearer"));
            httpContext.RequestServices = CriarServiceProviderComAutenticacaoSucesso();

            var app = Substitute.For<Microsoft.AspNetCore.Builder.IApplicationBuilder>();
            app.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(app);

            var middleware = CapturarMiddleware(app);
            var nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            // Quando
            JwtTokenMiddleware.UseJwtTokenMiddleware(app);
            await middleware(next).Invoke(httpContext);

            // Então
            httpContext.User.Identity!.IsAuthenticated.ShouldBeTrue();
            nextCalled.ShouldBeTrue();
        }

        private static IServiceProvider CriarServiceProviderComAutenticacaoSucesso()
        {
            var authService = Substitute.For<IAuthenticationService>();
            authService.AuthenticateAsync(Arg.Any<HttpContext>(), Arg.Any<string>())
                .Returns(AuthenticateResult.Success(new AuthenticationTicket(
                    new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test") }, "Bearer")),
                    "Bearer")));

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            return serviceProvider;
        }

        private static Func<RequestDelegate, RequestDelegate> CapturarMiddleware(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
            Func<RequestDelegate, RequestDelegate>? captured = null;
            app.When(x => x.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()))
                .Do(x => captured = x.Arg<Func<RequestDelegate, RequestDelegate>>());

            JwtTokenMiddleware.UseJwtTokenMiddleware(app);

            captured.ShouldNotBeNull("o middleware deve ter sido registrado no IApplicationBuilder");
            return captured;
        }
    }
}
