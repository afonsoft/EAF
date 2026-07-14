using Abp;
using Abp.Authorization;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Eaf.AspNetCore.Hangfire;
using Eaf.Middleware;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Eaf.Middleware.Tests.Hangfire
{
    /// <summary>
    /// Testes BDD para EafHangfireAuthorizationFilter seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafHangfireAuthorizationFilterBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_SemParametros_Quando_CriarInstancia_Entao_DeveInicializarComPermissoesPadrao()
        {
            var sut = new EafHangfireAuthorizationFilter();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_PermissoesCustom_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new EafHangfireAuthorizationFilter("Pages.Custom", "Pages.Admin");
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_SemParametros_Quando_CriarInstancia_Entao_DeveImplementarIDashboardAuthorizationFilter()
        {
            var sut = new EafHangfireAuthorizationFilter();
            sut.ShouldBeAssignableTo<global::Hangfire.Dashboard.IDashboardAuthorizationFilter>();
        }

        [Fact]
        public void Dado_ArrayVazio_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new EafHangfireAuthorizationFilter(System.Array.Empty<string>());
            sut.ShouldNotBeNull();
        }

        #endregion

        #region Authorize

        [Fact]
        public void Dado_RequisicaoLocalhost_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            var dashboardContext = CriarDashboardContext(httpContext);
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(dashboardContext);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SessaoComUsuarioEPermissao_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext, withUser: true));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SemUsuarioSemToken_Quando_Authorize_Entao_DeveRetornarFalso()
        {
            var httpContext = CriarHttpContext();
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeFalse();
        }

        [Fact]
        public void Dado_TokenJwtValidoNaQuery_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "token", token }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtInvalidoNaQuery_Quando_Authorize_Entao_DeveRetornarFalso()
        {
            var httpContext = CriarHttpContext();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "token", "not-a-jwt" }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeFalse();
        }

        [Fact]
        public void Dado_TokenJwtNaQueryComChaveAuth_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "auth", token }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtNaQueryComChaveAccessToken_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "accessToken", token }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtNoCookieEafAuthToken_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Headers["Cookie"] = $"Eaf.AuthToken={token}";
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtNoHeaderEafAuthToken_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Headers["Eaf.AuthToken"] = token;
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtNoCachePorIp_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            var token = CriarTokenJwtValido(2, 1);
            var dashboardContext = CriarDashboardContext(httpContext);
            var cache = dashboardContext.GetHttpContext().RequestServices.GetRequiredService<ICacheManager>().GetCache("HangFireCache");
            cache.GetOrDefault("127.0.0.1").Returns(token);

            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(dashboardContext);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TokenJwtComUserIdentifierClaim_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtComUserIdentifier(2, 1);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "token", token }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SemPermissoesComUsuarioNaSessao_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var sut = new EafHangfireAuthorizationFilter(System.Array.Empty<string>());

            var result = sut.Authorize(CriarDashboardContext(httpContext, withUser: true));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SemPermissoesComTokenJwt_Quando_Authorize_Entao_DeveRetornarVerdadeiro()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtValido(2, 1);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "token", token }
            });
            var sut = new EafHangfireAuthorizationFilter(System.Array.Empty<string>());

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ContextoNulo_Quando_Authorize_Entao_DeveRetornarFalso()
        {
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(null);

            result.ShouldBeFalse();
        }

        #endregion

        private static DashboardContext CriarDashboardContext(HttpContext httpContext, bool withUser = false)
        {
            var services = new ServiceCollection();
            var session = Substitute.For<IAbpSession>();
            if (withUser)
            {
                session.UserId.Returns(2L);
                session.TenantId.Returns(1);
            }
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGranted(Arg.Any<UserIdentifier>(), Arg.Any<string>()).Returns(true);
            var cacheManager = Substitute.For<ICacheManager>();
            var cache = Substitute.For<ICache>();
            cacheManager.GetCache(Arg.Any<string>()).Returns(cache);

            services.AddSingleton(session);
            services.AddSingleton(permissionChecker);
            services.AddSingleton(cacheManager);

            httpContext.RequestServices = services.BuildServiceProvider();

            var jobStorage = Substitute.For<JobStorage>();
            return new AspNetCoreDashboardContext(jobStorage, new DashboardOptions(), httpContext);
        }

        private static HttpContext CriarHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("example.com");
            httpContext.Request.Scheme = "http";
            return httpContext;
        }

        [Fact]
        public void Dado_TokenJwtComTenantSemSub_Quando_Authorize_Entao_DeveRetornarFalso()
        {
            var httpContext = CriarHttpContext();
            var token = CriarTokenJwtComTenantSemSub(2);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "token", token }
            });
            var sut = new EafHangfireAuthorizationFilter();

            var result = sut.Authorize(CriarDashboardContext(httpContext));

            result.ShouldBeFalse();
        }

        private static string CriarTokenJwtComTenantSemSub(int tenantId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(AbpClaimTypes.TenantId, tenantId.ToString())
                });
            return tokenHandler.WriteToken(token);
        }

        private static string CriarTokenJwtValido(long userId, int tenantId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(AbpClaimTypes.TenantId, tenantId.ToString())
                });
            return tokenHandler.WriteToken(token);
        }

        private static string CriarTokenJwtComUserIdentifier(long userId, int tenantId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(AbpClaimTypes.TenantId, tenantId.ToString()),
                    new Claim(MiddlewareCoreConsts.UserIdentifier, $"{userId}@{tenantId}")
                });
            return tokenHandler.WriteToken(token);
        }
    }
}
