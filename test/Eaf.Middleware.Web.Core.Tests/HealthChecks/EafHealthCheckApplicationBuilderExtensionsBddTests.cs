using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.HealthChecks
{
    public class EafHealthCheckApplicationBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ApplicationBuilderComHealthChecks_Quando_UsarEafHealthChecks_Entao_DeveAdicionarMiddlewareEPipeline()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddHealthChecks();
            var serviceProvider = services.BuildServiceProvider();

            var app = new ApplicationBuilder(serviceProvider);
            var result = app.UseEafHealthChecks();

            result.ShouldBeSameAs(app);

            var pipeline = app.Build();
            pipeline.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ApplicationBuilderComHealthChecks_Quando_UsarEafHealthChecksComOptionsCustomizados_Entao_DeveAdicionarMiddleware()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddHealthChecks();
            var serviceProvider = services.BuildServiceProvider();

            var app = new ApplicationBuilder(serviceProvider);
            var options = new HealthCheckOptions
            {
                ResponseWriter = (context, report) => System.Threading.Tasks.Task.CompletedTask
            };

            var result = app.UseEafHealthChecks(options);

            result.ShouldBeSameAs(app);

            var pipeline = app.Build();
            pipeline.ShouldNotBeNull();
        }
    }
}
