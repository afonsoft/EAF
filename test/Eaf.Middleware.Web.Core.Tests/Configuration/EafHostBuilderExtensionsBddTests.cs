using Eaf.Middleware.Web.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafHostBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafHostBuilderExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostBuilder_Quando_UsarAbpConfigurationSemPrefixo_Entao_DeveConfigurarAppConfigurationERetornarBuilder()
        {
            var builder = Substitute.For<IHostBuilder>();
            Action<HostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<HostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            var result = builder.UseAbpConfiguration();

            result.ShouldBeSameAs(builder);
            capturedAction.ShouldNotBeNull();

            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.Returns("Development");

            var context = new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = hostEnvironment,
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
            sources.Select(s => s.GetType().Name).ShouldContain("JsonConfigurationSource");
            sources.Select(s => s.GetType().Name).ShouldContain("EnvironmentVariablesConfigurationSource");
            sources.Select(s => s.GetType().Name).ShouldContain("MemoryConfigurationSource");
            properties.ContainsKey("FileProvider").ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostBuilder_Quando_UsarAbpConfigurationComPrefixo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo()
        {
            var builder = Substitute.For<IHostBuilder>();
            Action<HostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<HostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            var result = builder.UseAbpConfiguration("EAF_");

            result.ShouldBeSameAs(builder);
            capturedAction.ShouldNotBeNull();

            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.Returns("Production");

            var context = new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = hostEnvironment,
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
            sources.Select(s => s.GetType().Name).ShouldContain("EnvironmentVariablesConfigurationSource");
        }

        [Fact]
        public void Dado_WebHostBuilder_Quando_UsarAbpConfiguration_Entao_DeveConfigurarAppConfigurationERetornarBuilder()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            var result = builder.UseAbpConfiguration("EAF_");

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
            sources.Select(s => s.GetType().Name).ShouldContain("JsonConfigurationSource");
            sources.Select(s => s.GetType().Name).ShouldContain("EnvironmentVariablesConfigurationSource");
            sources.Select(s => s.GetType().Name).ShouldContain("MemoryConfigurationSource");
            properties.ContainsKey("FileProvider").ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarAbpConfigurationEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>()))
                    .UseAbpConfiguration();

                using var host = builder.Build();

                host.ShouldNotBeNull();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }
    }
}
