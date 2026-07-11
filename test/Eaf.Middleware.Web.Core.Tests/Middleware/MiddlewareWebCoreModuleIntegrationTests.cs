using Abp;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Middleware
{
    public class MiddlewareWebCoreModuleIntegrationTests
    {
        [Fact]
        public void Dado_MiddlewareWebCoreModule_Quando_ExecutarCicloDeVida_Entao_DeveCompletarSemErros()
        {
            var iocManager = new IocManager();
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            iocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component.For<IHostEnvironment>()
                    .Instance(hostEnvironment)
                    .LifestyleSingleton()
            );

            using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareWebCoreModuleIntegrationTestModule>(options =>
            {
                options.IocManager = iocManager;
            });

            Should.NotThrow(() => bootstrapper.Initialize());

            bootstrapper.IocManager.IsRegistered<IUserFriendsCache>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<ISettingManager>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<IChatCommunicator>().ShouldBeTrue();
            bootstrapper.IocManager.IsRegistered<AppFolders>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_RedisCacheHabilitado_Quando_ExecutarCicloDeVida_Entao_DeveCompletarSemErros()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                File.WriteAllText(
                    Path.Combine(tempDir, "appsettings.json"),
                    JsonSerializer.Serialize(new
                    {
                        RedisCache = new
                        {
                            IsEnabled = "true",
                            ConnectionString = "localhost:6379"
                        }
                    }));

                var iocManager = new IocManager();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Development");

                iocManager.IocContainer.Register(
                    Castle.MicroKernel.Registration.Component.For<IHostEnvironment>()
                        .Instance(hostEnvironment)
                        .LifestyleSingleton()
                );

                using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareWebCoreModuleIntegrationTestModule>(options =>
                {
                    options.IocManager = iocManager;
                });

                Should.NotThrow(() => bootstrapper.Initialize());
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        [Fact]
        public void Dado_BackgroundJobsHabilitado_Quando_ExecutarCicloDeVida_Entao_DeveAdicionarExpiredAuditLogDeleterWorker()
        {
            var iocManager = new IocManager();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(Path.GetTempPath());
            hostEnvironment.EnvironmentName.Returns("Development");

            iocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component.For<IHostEnvironment>()
                    .Instance(hostEnvironment)
                    .LifestyleSingleton()
            );

            using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareWebCoreModuleJobExecutionTestModule>(options =>
            {
                options.IocManager = iocManager;
            });

            Should.NotThrow(() => bootstrapper.Initialize());

            bootstrapper.IocManager.IsRegistered<Eaf.Middleware.Web.Auditing.ExpiredAuditLogDeleterWorker>().ShouldBeTrue();
        }

        [Fact]
        public void Dado_HangfireHabilitado_Quando_ExecutarCicloDeVida_Entao_DeveConfigurarInMemoryStorage()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                File.WriteAllText(
                    Path.Combine(tempDir, "appsettings.json"),
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Hangfire = new { IsEnabled = "true" },
                        Database = new { Provider = "PostgreSQL" }
                    }));

                var iocManager = new IocManager();
                var hostEnvironment = Substitute.For<IHostEnvironment>();
                hostEnvironment.ContentRootPath.Returns(tempDir);
                hostEnvironment.EnvironmentName.Returns("Development");

                iocManager.IocContainer.Register(
                    Castle.MicroKernel.Registration.Component.For<IHostEnvironment>()
                        .Instance(hostEnvironment)
                        .LifestyleSingleton()
                );

                using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareWebCoreModuleJobExecutionTestModule>(options =>
                {
                    options.IocManager = iocManager;
                });

                Should.NotThrow(() => bootstrapper.Initialize());

                JobStorage.Current.ShouldBeOfType<InMemoryStorage>();
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Abp.Modules.DependsOn(typeof(MiddlewareWebCoreModuleIntegrationTestModule))]
    public class MiddlewareWebCoreModuleJobExecutionTestModule : Abp.Modules.AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();
            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
        }
    }
}
