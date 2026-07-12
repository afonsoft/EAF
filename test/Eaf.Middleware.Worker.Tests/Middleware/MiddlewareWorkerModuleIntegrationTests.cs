using Abp;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Worker.Folders;
using Eaf.Middleware.Worker.VirtualFileSystem;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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

        [Fact]
        public void Dado_AspNetCoreEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var originalEaf = Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            var originalHosting = Environment.GetEnvironmentVariable("Hosting:Environment");
            var originalAspNet = Environment.GetEnvironmentVariable("ASPNET_ENV");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", null);
                Environment.SetEnvironmentVariable("ASPNET_ENV", null);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

                var tempDir = Path.GetTempPath();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Staging");

                var module = new MiddlewareWorkerModule(hostEnvironment);

                module.ShouldNotBeNull();
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").ShouldBe("Staging");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", originalEaf);
                Environment.SetEnvironmentVariable("Hosting:Environment", originalHosting);
                Environment.SetEnvironmentVariable("ASPNET_ENV", originalAspNet);
            }
        }

        [Fact]
        public void Dado_EafEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var originalEaf = Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", "Production");
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

                var tempDir = Path.GetTempPath();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Production");

                var module = new MiddlewareWorkerModule(hostEnvironment);

                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Production");
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").ShouldBe("Production");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", originalEaf);
            }
        }

        [Fact]
        public void Dado_HostingEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var originalHosting = Environment.GetEnvironmentVariable("Hosting:Environment");
            var originalAspNet = Environment.GetEnvironmentVariable("ASPNET_ENV");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", "Test");
                Environment.SetEnvironmentVariable("ASPNET_ENV", null);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

                var tempDir = Path.GetTempPath();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Test");

                var module = new MiddlewareWorkerModule(hostEnvironment);

                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Test");
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").ShouldBe("Test");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
                Environment.SetEnvironmentVariable("Hosting:Environment", originalHosting);
                Environment.SetEnvironmentVariable("ASPNET_ENV", originalAspNet);
            }
        }

        [Fact]
        public void Dado_AspNetEnv_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var originalHosting = Environment.GetEnvironmentVariable("Hosting:Environment");
            var originalAspNet = Environment.GetEnvironmentVariable("ASPNET_ENV");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", null);
                Environment.SetEnvironmentVariable("ASPNET_ENV", "Custom");
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

                var tempDir = Path.GetTempPath();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Custom");

                var module = new MiddlewareWorkerModule(hostEnvironment);

                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Custom");
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").ShouldBe("Custom");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
                Environment.SetEnvironmentVariable("Hosting:Environment", originalHosting);
                Environment.SetEnvironmentVariable("ASPNET_ENV", originalAspNet);
            }
        }

        [Fact]
        public void Dado_DotnetEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", null);
                Environment.SetEnvironmentVariable("ASPNET_ENV", null);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "QA");

                var tempDir = Path.GetTempPath();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("QA");

                var module = new MiddlewareWorkerModule(hostEnvironment);

                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("QA");
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT").ShouldBe("QA");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
            }
        }

        [Fact]
        public void Dado_CacheConfigurado_Quando_PreInitialize_Entao_DeveAplicarCacheConfigurator()
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

            module.PreInitialize();

            var cacheManager = iocManager.Resolve<ICacheManager>();
            var cache = cacheManager.GetCache("test");
            cache.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromMinutes(10));
        }

    }
}
