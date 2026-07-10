using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
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
    }
}
