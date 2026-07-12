using Eaf.Middleware.Web.Startup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafServiceCollectionMiddlewareExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafServiceCollectionMiddlewareExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafServiceCollectionMiddlewareExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurer_Entao_DeveRegistrarServicosObrigatorios()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            services.AddEafConfigurer(configuration);

            services.Count.ShouldBeGreaterThan(0);
            services.Any(s => s.ServiceType == typeof(IAuthenticationService)).ShouldBeTrue();
            services.Any(s => s.ServiceType == typeof(IMemoryCache)).ShouldBeTrue();
            services.Any(s => s.ServiceType == typeof(IDistributedCache)).ShouldBeTrue();
            services.Any(s => s.ServiceType == typeof(IResponseCompressionProvider)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComJwtBearer_Entao_DeveRegistrarServicosDeAutenticacao()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:SecurityKey", "8CFB2EC534E14D56_EAF_8CFB2EC534E14D56"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:Issuer", "EAF"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:Audience", "EAF")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Any(s => s.ServiceType == typeof(IAuthenticationService)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComHangfire_Entao_DeveRegistrarServicosHangfire()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Hangfire:IsEnabled", "true")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComRedis_Entao_DeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("RedisCache:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("RedisCache:ConnectionString", "localhost:6379")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Any(s => s.ImplementationType != null && s.ImplementationType.Name.Contains("RedisCache")).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComRedisIsRedisEnabled_Entao_DeveRegistrarRedisCache()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("RedisCache:IsRedisEnabled", "true"),
                    new KeyValuePair<string, string?>("RedisCache:ConnectionString", "localhost:6379")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Any(s => s.ImplementationType != null && s.ImplementationType.Name.Contains("RedisCache")).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComSqlServerIsSqlEnabled_Entao_DeveRegistrarSqlServerCache()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("SqlServer:IsSqlEnabled", "true"),
                    new KeyValuePair<string, string?>("SqlServerCache:ConnectionString", "Server=.;Database=Cache;"),
                    new KeyValuePair<string, string?>("SqlServerCache:SchemaName", "dbo"),
                    new KeyValuePair<string, string?>("SqlServerCache:TableName", "EafCache")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Any(s => s.ImplementationType != null && s.ImplementationType.Name == "SqlServerCache").ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComSqlServerCache_Entao_DeveRegistrarSqlServerCache()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("SqlServerCache:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("SqlServerCache:ConnectionString", "Server=.;Database=Cache;"),
                    new KeyValuePair<string, string?>("SqlServerCache:SchemaName", "dbo"),
                    new KeyValuePair<string, string?>("SqlServerCache:TableName", "EafCache")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            services.Any(s => s.ImplementationType != null && s.ImplementationType.Name == "SqlServerCache").ShouldBeTrue();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafConfigurerComTudoHabilitado_Entao_DeveConfigurarOptions()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:SecurityKey", "8CFB2EC534E14D56_EAF_8CFB2EC534E14D56"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:Issuer", "EAF"),
                    new KeyValuePair<string, string?>("Authentication:JwtBearer:Audience", "EAF"),
                    new KeyValuePair<string, string?>("Hangfire:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("RedisCache:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("RedisCache:ConnectionString", "localhost:6379"),
                    new KeyValuePair<string, string?>("SqlServerCache:IsEnabled", "true"),
                    new KeyValuePair<string, string?>("SqlServerCache:ConnectionString", "Server=.;Database=Cache;"),
                    new KeyValuePair<string, string?>("SqlServerCache:SchemaName", "dbo"),
                    new KeyValuePair<string, string?>("SqlServerCache:TableName", "EafCache")
                })
                .Build();

            services.AddEafConfigurer(configuration);

            var provider = services.BuildServiceProvider();
            var hubOptions = provider.GetRequiredService<IOptions<HubOptions>>().Value;
            var responseCompression = provider.GetRequiredService<IOptions<ResponseCompressionOptions>>().Value;
            var cookiePolicy = provider.GetRequiredService<IOptions<CookiePolicyOptions>>().Value;
            var sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
            var brCompression = provider.GetRequiredService<IOptions<BrotliCompressionProviderOptions>>().Value;
            var gzipCompression = provider.GetRequiredService<IOptions<GzipCompressionProviderOptions>>().Value;
            var jwtOptions = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);

            hubOptions.ShouldNotBeNull();
            responseCompression.ShouldNotBeNull();
            cookiePolicy.ShouldNotBeNull();
            sessionOptions.ShouldNotBeNull();
            brCompression.ShouldNotBeNull();
            gzipCompression.ShouldNotBeNull();
            jwtOptions.ShouldNotBeNull();
        }
    }
}
