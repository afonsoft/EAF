using Eaf.Middleware.Web.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WebHost = Microsoft.AspNetCore.WebHost;

namespace Eaf.Middleware.Web.Core.Tests.Configuration
{
    public class EafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_IHostBuilder_Quando_UseAbpConfigurationSemParametros_Entao_DeveConfigurarAppConfiguration()
        {
            var builder = Substitute.For<IHostBuilder>();
            builder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>()).Returns(builder);

            var result = builder.UseAbpConfiguration();

            result.ShouldNotBeNull();
            builder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixo_Entao_DeveConfigurarAppConfiguration()
        {
            var builder = Substitute.For<IHostBuilder>();
            builder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>()).Returns(builder);

            var result = builder.UseAbpConfiguration("EAF_");

            result.ShouldNotBeNull();
            builder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_IWebHostBuilder_Quando_UseAbpConfigurationSemParametros_Entao_DeveConfigurarAppConfiguration()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            builder.ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>()).Returns(builder);

            var result = builder.UseAbpConfiguration();

            result.ShouldNotBeNull();
            builder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixo_Entao_DeveConfigurarAppConfiguration()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            builder.ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>()).Returns(builder);

            var result = builder.UseAbpConfiguration("EAF_");

            result.ShouldNotBeNull();
            builder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<WebHostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarAbpConfigurationComPrefixo_Entao_DeveCriarHost()
        {
            var builder = new HostBuilder()
                .UseAbpConfiguration("EAF_");

            using var host = builder.Build();

            host.ShouldNotBeNull();
            host.Services.GetService(typeof(IConfiguration)).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarAbpConfigurationSemParametros_Entao_DeveCriarHost()
        {
            var builder = new HostBuilder()
                .UseAbpConfiguration();

            using var host = builder.Build();

            host.ShouldNotBeNull();
            host.Services.GetService(typeof(IConfiguration)).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_WebHostBuilderReal_Quando_UsarAbpConfigurationComPrefixo_Entao_DeveCriarWebHost()
        {
#pragma warning disable ASPDEPR008 // IWebHost is obsolete
            var builder = WebHost.CreateDefaultBuilder()
                .Configure(app => { })
                .UseAbpConfiguration("EAF_");

            using var host = builder.Build();
#pragma warning restore ASPDEPR008

            host.ShouldNotBeNull();
            host.Services.GetService(typeof(IConfiguration)).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_WebHostBuilderReal_Quando_UsarAbpConfigurationSemParametros_Entao_DeveCriarWebHost()
        {
#pragma warning disable ASPDEPR008 // IWebHost is obsolete
            var builder = WebHost.CreateDefaultBuilder()
                .Configure(app => { })
                .UseAbpConfiguration();

            using var host = builder.Build();
#pragma warning restore ASPDEPR008

            host.ShouldNotBeNull();
            host.Services.GetService(typeof(IConfiguration)).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixoENulo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo()
        {
            var builder = Substitute.For<IHostBuilder>();
            Action<HostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<HostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            builder.UseAbpConfiguration(null, "EAF_");

            capturedAction.ShouldNotBeNull();

            var context = new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = Substitute.For<IHostEnvironment>(),
                Configuration = Substitute.For<IConfiguration>()
            };
            context.HostingEnvironment.EnvironmentName.Returns("Development");

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
            sources.Any(s => s is EnvironmentVariablesConfigurationSource).ShouldBeTrue();
        }

        [Fact]
        public void Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixoVazio_Entao_DeveConfigurarSemPrefixo()
        {
            var builder = Substitute.For<IHostBuilder>();
            Action<HostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<HostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            builder.UseAbpConfiguration("");

            capturedAction.ShouldNotBeNull();

            var context = new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = Substitute.For<IHostEnvironment>(),
                Configuration = Substitute.For<IConfiguration>()
            };
            context.HostingEnvironment.EnvironmentName.Returns("Development");

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
        public void Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixoENulo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            builder.UseAbpConfiguration(null, "EAF_");

            capturedAction.ShouldNotBeNull();

            var context = new WebHostBuilderContext
            {
                HostingEnvironment = Substitute.For<IWebHostEnvironment>(),
                Configuration = Substitute.For<IConfiguration>()
            };
            context.HostingEnvironment.EnvironmentName.Returns("Development");

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
            sources.Any(s => s is EnvironmentVariablesConfigurationSource).ShouldBeTrue();
        }

        [Fact]
        public void Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixoVazio_Entao_DeveConfigurarSemPrefixo()
        {
            var builder = Substitute.For<IWebHostBuilder>();
            Action<WebHostBuilderContext, IConfigurationBuilder> capturedAction = null!;
            builder.ConfigureAppConfiguration(Arg.Do<Action<WebHostBuilderContext, IConfigurationBuilder>>(a => capturedAction = a))
                .Returns(builder);

            builder.UseAbpConfiguration("");

            capturedAction.ShouldNotBeNull();

            var context = new WebHostBuilderContext
            {
                HostingEnvironment = Substitute.For<IWebHostEnvironment>(),
                Configuration = Substitute.For<IConfiguration>()
            };
            context.HostingEnvironment.EnvironmentName.Returns("Development");

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
    }
}
