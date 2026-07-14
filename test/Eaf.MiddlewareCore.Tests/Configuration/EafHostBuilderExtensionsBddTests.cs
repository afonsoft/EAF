using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Configuration
{
    public class EafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_HostBuilder_Quando_UsarEafConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = hostBuilder.UseEafConfiguration();

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_HostBuilderComPrefixo_Quando_UsarEafConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = hostBuilder.UseEafConfiguration("EAF_");

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_HostBuilderComActionCustomizado_Quando_UsarEafConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = hostBuilder.UseEafConfiguration((ctx, config) => config.AddInMemoryCollection(), "EAF_");

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarEafConfigurationEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string>()))
                    .UseEafConfiguration();

                using var host = builder.Build();

                host.ShouldNotBeNull();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarEafConfigurationComPrefixoEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string>()))
                    .UseEafConfiguration("EAF_");

                using var host = builder.Build();

                host.ShouldNotBeNull();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarEafConfigurationComActionEPrefixoEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string>()))
                    .UseEafConfiguration((ctx, config) => config.AddInMemoryCollection(), "EAF_");

                using var host = builder.Build();

                host.ShouldNotBeNull();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostBuilderComPrefixoEActionNula_Quando_UsarEafConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = hostBuilder.UseEafConfiguration(null, "EAF_");

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, IConfigurationBuilder>>());
        }
    }
}
