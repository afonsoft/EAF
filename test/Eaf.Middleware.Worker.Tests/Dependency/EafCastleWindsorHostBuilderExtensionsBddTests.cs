using Abp.Dependency;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Dependency
{
    public class EafCastleWindsorHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_HostBuilderEContainer_Quando_UsarCastleWindsor_Entao_DeveRegistrarContainerEServiceProviderFactory()
        {
            using var container = new WindsorContainer();
            var hostBuilder = Host.CreateDefaultBuilder();

            var result = hostBuilder.UseCastleWindsor(container);

            result.ShouldBeSameAs(hostBuilder);
            var services = new ServiceCollection();
            var providerFactory = new Castle.Windsor.MsDependencyInjection.WindsorServiceProviderFactory();
            var serviceProvider = providerFactory.CreateServiceProvider(providerFactory.CreateBuilder(services));
            serviceProvider.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ContainerNulo_Quando_UsarCastleWindsor_Entao_DeveLancarArgumentNullException()
        {
            var hostBuilder = Substitute.For<IHostBuilder>();

            Should.Throw<ArgumentNullException>(() => hostBuilder.UseCastleWindsor(null));
        }
    }
}
