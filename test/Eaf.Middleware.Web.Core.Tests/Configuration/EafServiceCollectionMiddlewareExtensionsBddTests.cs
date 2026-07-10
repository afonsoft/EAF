using Eaf.Middleware.Web.Startup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
