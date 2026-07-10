using Abp;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Eaf.Middleware.Worker.Folders;
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
            using var bootstrapper = Abp.AbpBootstrapper.Create<WorkerModuleTestDependenciesModule>();
            bootstrapper.Initialize();

            var iocManager = bootstrapper.IocManager;
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
    }
}
