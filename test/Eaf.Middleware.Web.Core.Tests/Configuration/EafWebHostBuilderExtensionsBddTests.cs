using Eaf.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Memory;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafWebHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafWebHostBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafWebHostBuilderExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_WebHostBuilder_Quando_UseEafConfiguration_Entao_DeveConfigurarAppConfigurationERetornarBuilder()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            var result = builder.UseEafConfiguration("EAF_");

            result.ShouldBeSameAs(builder);
            capturedAction.ShouldNotBeNull();

            var webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            webHostEnvironment.EnvironmentName.Returns("Development");

            var context = new WebHostBuilderContext
            {
                HostingEnvironment = webHostEnvironment,
                Configuration = Substitute.For<IConfiguration>()
            };

            var sources = new List<IConfigurationSource>();
            var properties = new Dictionary<string, object>();
            var configBuilder = Substitute.For<IConfigurationBuilder>();
            configBuilder.Sources.Returns(sources);
            configBuilder.Properties.Returns(properties);
            configBuilder.Add(Arg.Any<IConfigurationSource>())
                .Returns(ci =>
                {
                    sources.Add(ci.Arg<IConfigurationSource>());
                    return configBuilder;
                });

            capturedAction(context, configBuilder);

            sources.Count.ShouldBe(5);
            sources.Any(s => s is JsonConfigurationSource).ShouldBeTrue();
            sources.Any(s => s is EnvironmentVariablesConfigurationSource).ShouldBeTrue();
            sources.Any(s => s is MemoryConfigurationSource).ShouldBeTrue();
            properties.ContainsKey("FileProvider").ShouldBeTrue();
        }

        [Fact]
        public void Dado_WebHostBuilder_Quando_UseEafConfigurationSemPrefixo_Entao_DeveConfigurarSemPrefixo()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            builder.UseEafConfiguration();

            capturedAction.ShouldNotBeNull();

            var webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            webHostEnvironment.EnvironmentName.Returns("Production");

            var context = new WebHostBuilderContext
            {
                HostingEnvironment = webHostEnvironment,
                Configuration = Substitute.For<IConfiguration>()
            };

            var sources = new List<IConfigurationSource>();
            var properties = new Dictionary<string, object>();
            var configBuilder = Substitute.For<IConfigurationBuilder>();
            configBuilder.Sources.Returns(sources);
            configBuilder.Properties.Returns(properties);
            configBuilder.Add(Arg.Any<IConfigurationSource>())
                .Returns(ci =>
                {
                    sources.Add(ci.Arg<IConfigurationSource>());
                    return configBuilder;
                });

            capturedAction(context, configBuilder);

            sources.Count.ShouldBe(4);
        }

        [Fact]
        public void Dado_WebHostBuilder_Quando_UseEafConfigurationComAction_Entao_DeveDelegarConfiguracao()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            var customActionInvoked = false;
            builder.UseEafConfiguration((ctx, config) => { customActionInvoked = true; }, "EAF_");

            capturedAction.ShouldNotBeNull();
            customActionInvoked.ShouldBeFalse();

            var context = new WebHostBuilderContext
            {
                HostingEnvironment = Substitute.For<IWebHostEnvironment>(),
                Configuration = Substitute.For<IConfiguration>()
            };
            var configBuilder = Substitute.For<IConfigurationBuilder>();
            configBuilder.Sources.Returns(new List<IConfigurationSource>());
            configBuilder.Properties.Returns(new Dictionary<string, object>());

            capturedAction(context, configBuilder);

            customActionInvoked.ShouldBeTrue();
        }
    }
}
