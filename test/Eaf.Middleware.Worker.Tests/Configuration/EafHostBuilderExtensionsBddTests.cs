using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    public class EafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_HostBuilder_Quando_UsarAbpConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, Microsoft.Extensions.Configuration.IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = Eaf.Middleware.Configuration.EafHostBuilderExtensions.UseAbpConfiguration(hostBuilder);

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, Microsoft.Extensions.Configuration.IConfigurationBuilder>>());
        }

        [Fact]
        public void Dado_HostBuilderComPrefixo_Quando_UsarAbpConfiguration_Entao_DeveRetornarMesmoBuilder()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();
            hostBuilder.ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, Microsoft.Extensions.Configuration.IConfigurationBuilder>>())
                .Returns(hostBuilder);

            var result = Eaf.Middleware.Configuration.EafHostBuilderExtensions.UseAbpConfiguration(hostBuilder, "EAF_");

            result.ShouldBeSameAs(hostBuilder);
            hostBuilder.Received(1).ConfigureAppConfiguration(Arg.Any<Action<HostBuilderContext, Microsoft.Extensions.Configuration.IConfigurationBuilder>>());
        }
    }
}
