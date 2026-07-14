using Abp.Dependency;
using Abp.Modules;
using Eaf.Middleware.Worker.Tests.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.ServiceProviders
{
    public class EafServiceCollectionExtensionsBddTests
    {
        private static readonly Action<Abp.AbpBootstrapperOptions> BootstrapperOptions = options =>
        {
            options.IocManager = new IocManager();
        };

        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafServiceCollectionExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafServiceCollectionExtensions).IsSealed.ShouldBeTrue();
        }

        private static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHostApplicationLifetime>(Substitute.For<IHostApplicationLifetime>());
            return services;
        }

        [Fact]
        public void Dado_ColecoesDeServicos_Quando_AdicionarEaf_Entao_DeveRetornarServiceProviderConfigurado()
        {
            var services = CreateServices();

            var serviceProvider = services.AddEaf<WorkerModuleTestDependenciesModule>(BootstrapperOptions);

            serviceProvider.ShouldNotBeNull();
            serviceProvider.GetService<ILoggerFactory>().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ColecoesDeServicos_Quando_AdicionarEafSemRetornarServiceProvider_Entao_DeveInicializarSemErros()
        {
            var services = CreateServices();

            Should.NotThrow(() => services.AddEafWithoutCreatingServiceProvider<WorkerModuleTestDependenciesModule>(BootstrapperOptions));
        }

        [Fact]
        public void Dado_ColecoesDeServicos_Quando_AdicionarEafComOptions_Entao_DeveAplicarConfiguration()
        {
            var services = CreateServices();
            var optionsInvoked = false;

            var serviceProvider = services.AddEaf<WorkerModuleTestDependenciesModule>(options =>
            {
                optionsInvoked = true;
                options.IocManager = new IocManager();
            });

            optionsInvoked.ShouldBeTrue();
            serviceProvider.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ColecoesDeServicos_Quando_AdicionarEafSemRemoveConventionalInterceptors_Entao_DeveManterInterceptors()
        {
            var services = CreateServices();

            var serviceProvider = services.AddEaf<WorkerModuleTestDependenciesModule>(BootstrapperOptions, removeConventionalInterceptors: false);

            serviceProvider.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ColecoesDeServicos_Quando_AdicionarEafSemRetornarServiceProviderSemRemoveConventionalInterceptors_Entao_DeveManterInterceptors()
        {
            var services = CreateServices();

            Should.NotThrow(() => services.AddEafWithoutCreatingServiceProvider<WorkerModuleTestDependenciesModule>(BootstrapperOptions, removeConventionalInterceptors: false));
        }

        [Fact]
        public void Dado_CastleLoggerFactoryRegistrado_Quando_AdicionarEaf_Entao_DeveRetornarServiceProviderConfigurado()
        {
            var services = CreateServices();
            services.AddSingleton(Substitute.For<Castle.Core.Logging.ILoggerFactory>());

            var serviceProvider = services.AddEaf<WorkerModuleTestDependenciesModule>(BootstrapperOptions);

            serviceProvider.ShouldNotBeNull();
            serviceProvider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>().ShouldNotBeNull();
        }
    }
}
