using Eaf.Middleware.Web.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
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
    }
}
