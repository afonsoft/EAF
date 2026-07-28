using Eaf.Middleware.Web.Startup;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Tests.Configuration
{
    public class EafCorsConfigurationBddTests
    {
        [Fact]
        public void Dado_OrigensComWildcard_Quando_RegistrarPolitica_Entao_DevePermitirSubdominio()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "App:CorsOrigins", "https://*.example.com;http://localhost:4200" }
                })
                .Build();

            var services = new ServiceCollection();

            // Quando
            services.AddEafCors(configuration, false, "TestPolicy");

            // Então
            var sp = services.BuildServiceProvider();
            var options = sp.GetRequiredService<IOptions<CorsOptions>>().Value;
            var policy = options.GetPolicy("TestPolicy");

            policy.ShouldNotBeNull();
            policy.SupportsCredentials.ShouldBeTrue();
            policy.IsOriginAllowed("https://sub.example.com").ShouldBeTrue();
            policy.IsOriginAllowed("https://example.com").ShouldBeFalse();
            policy.IsOriginAllowed("http://localhost:4200").ShouldBeTrue();
        }

        [Fact]
        public void Dado_HeadersDoEafHttpInterceptor_Quando_RegistrarPolitica_Entao_DeveEstarPermitidos()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "App:CorsOrigins", "https://admin.example.com" }
                })
                .Build();

            var services = new ServiceCollection();

            // Quando
            services.AddEafCors(configuration, false, "TestPolicy");

            // Então
            var sp = services.BuildServiceProvider();
            var options = sp.GetRequiredService<IOptions<CorsOptions>>().Value;
            var policy = options.GetPolicy("TestPolicy");

            policy.ShouldNotBeNull();
            policy.Headers.ShouldContain("Authorization");
            policy.Headers.ShouldContain("Abp-TenantId");
            policy.Headers.ShouldContain("Pragma");
            policy.Headers.ShouldContain("Cache-Control");
            policy.Headers.ShouldContain("Expires");
            policy.Headers.ShouldContain("X-Correlation-ID");
        }

        [Fact]
        public void Dado_AmbienteDeProducaoSemOrigens_Quando_RegistrarPolitica_Entao_DeveLancarExcecao()
        {
            // Dado
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            var services = new ServiceCollection();

            // Quando / Então
            Should.Throw<System.InvalidOperationException>(() =>
                services.AddEafCors(configuration, false, "TestPolicy"));
        }
    }
}
