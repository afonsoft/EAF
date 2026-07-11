using Abp.Authorization;
using Abp.Dependency;
using Abp.Runtime.Security;
using Eaf.Middleware;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Startup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class AuthConfigurerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(AuthConfigurer).IsAbstract.ShouldBeTrue();
            typeof(AuthConfigurer).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_JwtBearerAtivado_Quando_Configure_Entao_DeveRegistrarAutenticacao()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string?>
                {
                    ["Authentication:JwtBearer:IsEnabled"] = "true",
                    ["Authentication:JwtBearer:SecurityKey"] = "EAF_P25_TestKey_1234567890123456"
                })
                .Build();

            // Quando
            Should.NotThrow(() => AuthConfigurer.Configure(services, configuration));

            // Então
            var tokenAuthConfig = Abp.Dependency.IocManager.Instance.Resolve<TokenAuthConfiguration>();
            tokenAuthConfig.ShouldNotBeNull();
            tokenAuthConfig.SecurityKey.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_JwtBearerDesativado_Quando_Configure_Entao_DeveRegistrarAutenticacaoSemJwtBearer()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string?>
                {
                    ["Authentication:JwtBearer:IsEnabled"] = "false"
                })
                .Build();

            // Quando
            Should.NotThrow(() => AuthConfigurer.Configure(services, configuration));

            // Então
            var provider = services.BuildServiceProvider();
            var authOptions = provider.GetService<Microsoft.Extensions.Options.IOptions<AuthenticationOptions>>()?.Value;
            authOptions.ShouldNotBeNull();
            authOptions.DefaultScheme.ShouldBe(JwtBearerDefaults.AuthenticationScheme);
            provider.GetService<IAuthenticationService>().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_PathVazio_Quando_QueryStringTokenResolver_Entao_DeveRetornarCompleted()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "");

            // Quando
            var task = AuthConfigurer.QueryStringTokenResolver(context);

            // Então
            task.ShouldBe(Task.CompletedTask);
        }

        [Fact]
        public void Dado_PathAllowAnonymous_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/signalr", hasToken: false);

            // Quando
            var task = AuthConfigurer.QueryStringTokenResolver(context);

            // Então
            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_PathExigeTokenSemHeader_Quando_TokenAusente_Entao_DeveLancarAbpAuthorizationException()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/Chat/GetUploadedObject?", hasToken: false);

            // Quando & Então
            await Should.ThrowAsync<AbpAuthorizationException>(async () => await AuthConfigurer.QueryStringTokenResolver(context));
        }

        [Fact]
        public async Task Dado_PathExigeToken_Quando_TokenCriptografado_Entao_DeveDefinirToken()
        {
            // Dado
            var token = "eaf-p25-token";
            var encryptedToken = SimpleStringCipher.Instance.Encrypt(token, MiddlewareCoreConsts.DefaultPassPhrase);
            var context = CriarMessageReceivedContext(path: "/Chat/GetUploadedObject?", token: encryptedToken);

            // Quando
            await AuthConfigurer.QueryStringTokenResolver(context);

            // Então
            context.Token.ShouldBe(token);
        }

        [Fact]
        public async Task Dado_PathExigeToken_Quando_TokenNulo_Quando_AnonimoNaoPermitido_Entao_DeveLancarExcecao()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/Profile/GetProfilePictureByUser?", hasToken: false);

            // Quando & Então
            await Should.ThrowAsync<AbpAuthorizationException>(async () => await AuthConfigurer.QueryStringTokenResolver(context));
        }

        [Fact]
        public async Task Dado_PathExigeToken_Quando_HeaderAuthorizationPresente_Entao_DeveRetornarCompleted()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/Chat/GetUploadedObject?", hasToken: false);
            context.Request.Headers["authorization"] = "Bearer token";

            // Quando
            await AuthConfigurer.QueryStringTokenResolver(context);

            // Então
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_PathExigeToken_Quando_ProfileHeaderAuthorizationPresente_Entao_DeveRetornarCompleted()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/Profile/GetProfilePictureByUser?", hasToken: false);
            context.Request.Headers["authorization"] = "Bearer token";

            // Quando
            await AuthConfigurer.QueryStringTokenResolver(context);

            // Então
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_PathExigeToken_Quando_TokenNullLiteral_Entao_DeveLancarAbpAuthorizationException()
        {
            // Dado
            var context = CriarMessageReceivedContext(path: "/Chat/GetUploadedObject?", token: "null");

            // Quando & Então
            await Should.ThrowAsync<AbpAuthorizationException>(async () => await AuthConfigurer.QueryStringTokenResolver(context));
        }

        [Fact]
        public async Task Dado_CaminhoHangfire_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/hangfire", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoJob_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/job", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoHeartbeat_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/heartbeat", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoHealthChecksUi_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/healthchecks-ui", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoHealth_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/health", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoSignalrComSubPath_Quando_QueryStringTokenResolver_Entao_DevePermitirSemToken()
        {
            var context = CriarMessageReceivedContext(path: "/signalr/test", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
            context.Token.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_CaminhoNaoMapeado_Quando_QueryStringTokenResolver_Entao_DeveRetornarCompleted()
        {
            var context = CriarMessageReceivedContext(path: "/unmapped", hasToken: false);

            var task = AuthConfigurer.QueryStringTokenResolver(context);

            task.ShouldBe(Task.CompletedTask);
        }

        [Fact]
        public void Dado_JwtBearerAtivadoSemSecurityKey_Quando_Configure_Entao_DeveUsarChavePadrao()
        {
            // Dado
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string?>
                {
                    ["Authentication:JwtBearer:IsEnabled"] = "true"
                })
                .Build();

            // Quando
            Should.NotThrow(() => AuthConfigurer.Configure(services, configuration));

            // Então
            var tokenAuthConfig = IocManager.Instance.Resolve<TokenAuthConfiguration>();
            tokenAuthConfig.ShouldNotBeNull();
            tokenAuthConfig.SecurityKey.ShouldNotBeNull();
        }

        private static MessageReceivedContext CriarMessageReceivedContext(
            string path,
            string? token = null,
            bool hasToken = true)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = new PathString(path);
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";

            if (!string.IsNullOrEmpty(token))
            {
                httpContext.Request.Query = new QueryCollection(new System.Collections.Generic.Dictionary<string, StringValues>
                {
                    ["enc_auth_token"] = token
                });
            }
            else if (hasToken)
            {
                httpContext.Request.Query = new QueryCollection(new System.Collections.Generic.Dictionary<string, StringValues>
                {
                    ["enc_auth_token"] = "not-empty-token"
                });
            }

            var scheme = new AuthenticationScheme("Bearer", "Bearer", typeof(JwtBearerHandler));
            var options = new JwtBearerOptions();
            return new MessageReceivedContext(httpContext, scheme, options);
        }
    }
}
