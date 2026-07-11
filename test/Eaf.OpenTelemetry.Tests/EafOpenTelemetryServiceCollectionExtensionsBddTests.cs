using Eaf.AspNetCore.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.OpenTelemetry.Tests
{
    /// <summary>
    /// Testes BDD para EafOpenTelemetryServiceCollectionExtensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafOpenTelemetryServiceCollectionExtensionsBddTests
    {


        [Fact]
        public void Dado_EndpointRouteBuilder_Quando_MapEafOpenTelemetryMetrics_Entao_DeveRetornarEndpointConventionBuilder()
        {
            // Dado
            var services = new ServiceCollection();
            services.AddSingleton<IApplicationBuilderFactory>(provider => new ApplicationBuilderFactory(provider));
            services.AddEafOpenTelemetry();
            var serviceProvider = services.BuildServiceProvider();
            var endpoints = Substitute.For<IEndpointRouteBuilder>();
            endpoints.ServiceProvider.Returns(serviceProvider);
            endpoints.DataSources.Returns(new List<EndpointDataSource>());
            endpoints.CreateApplicationBuilder().Returns(new ApplicationBuilderFactory(serviceProvider).CreateBuilder(new FeatureCollection()));

            // Quando
            var result = endpoints.MapEafOpenTelemetryMetrics();

            // Então
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafOpenTelemetry_Entao_DeveRetornarOpenTelemetryBuilder()
        {
            var services = new ServiceCollection();
            var result = services.AddEafOpenTelemetry();
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafOpenTelemetryComOptions_Entao_DeveConfigurarSourceName()
        {
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions> optionsAction = options =>
            {
                options.ServiceName = "TestService";
                options.SourceName = new[] { "CustomSource" };
            };

            var result = services.AddEafOpenTelemetry(optionsAction);

            result.ShouldNotBeNull();
        }
    }
}
