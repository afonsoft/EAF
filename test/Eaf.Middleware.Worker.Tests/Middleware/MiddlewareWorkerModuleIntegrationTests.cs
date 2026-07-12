using Abp;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Worker.Folders;
using Eaf.Middleware.Worker.VirtualFileSystem;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Middleware
{
    public class MiddlewareWorkerModuleIntegrationTests
    {
        [Fact]
        public void Dado_MiddlewareWorkerModule_Quando_ExecutarCicloDeVida_Entao_DeveCompletarSemErros()
        {
            var iocManager = new IocManager();
            using var bootstrapper = AbpBootstrapper.Create<WorkerModuleTestDependenciesModule>(options => options.IocManager = iocManager);
            bootstrapper.Initialize();

            var configuration = iocManager.Resolve<IAbpStartupConfiguration>();

            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            var module = new MiddlewareWorkerModule(hostEnvironment);
            var moduleType = typeof(AbpModule);
            moduleType.GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, iocManager);
            moduleType.GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, configuration);

            Should.NotThrow(() => module.PreInitialize());
            Should.NotThrow(() => module.Initialize());
            Should.NotThrow(() => module.PostInitialize());

            iocManager.IsRegistered<IEafWorkerBase>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_ProviderDeConteudoRegistrado_Quando_PostInitialize_Entao_DeveConfigurarRootFileProviderSemErro()
        {
            var iocManager = new IocManager();
            using var bootstrapper = AbpBootstrapper.Create<WorkerModuleTestDependenciesModule>(options => options.IocManager = iocManager);
            bootstrapper.Initialize();

            var configuration = iocManager.Resolve<IAbpStartupConfiguration>();

            iocManager.IocContainer.Register(Component.For<IWorkerContentFileProvider>().Instance(Substitute.For<IWorkerContentFileProvider>()));

            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            var module = new MiddlewareWorkerModule(hostEnvironment);
            typeof(AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, iocManager);
            typeof(AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, configuration);

            Should.NotThrow(() => module.Initialize());
            Should.NotThrow(() => module.PostInitialize());
        }

        [Fact]
        public void Dado_ErroAoCriarPastas_Quando_PostInitialize_Entao_DeveLogarErroSemLancarExcecao()
        {
            var iocManager = new IocManager();
            using var bootstrapper = AbpBootstrapper.Create<WorkerModuleTestDependenciesModule>(options => options.IocManager = iocManager);
            bootstrapper.Initialize();

            var configuration = iocManager.Resolve<IAbpStartupConfiguration>();

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                File.Create(Path.Combine(tempDir, "ProfileImages")).Dispose();

                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Development");

                var module = new MiddlewareWorkerModule(hostEnvironment);
                typeof(AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, iocManager);
                typeof(AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, configuration);

                Should.NotThrow(() => module.Initialize());
                Should.NotThrow(() => module.PostInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
            }
        }
    }
}
