using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Threading.BackgroundWorkers;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using Eaf.Middleware.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore
{
    public class MiddlewareWebCoreModuleBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarModulo_Entao_DeveTerNomeCorreto()
        {
            typeof(MiddlewareWebCoreModule).Name.ShouldBe("MiddlewareWebCoreModule");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveSerAbpModule()
        {
            typeof(Abp.Modules.AbpModule).IsAssignableFrom(typeof(MiddlewareWebCoreModule)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var original = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var module = new MiddlewareWebCoreModule(env);

                module.ShouldNotBeNull();
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Development");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", original);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_IocManagerConfigurado_Quando_Initialize_Entao_DeveRegistrarConventions()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);
                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostEnvironmentComRedisHabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"RedisCache\":{\"IsRedisEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);
                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostEnvironmentComRedisEnabledHabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"RedisCache\":{\"IsEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);
                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_IocManagerConfigurado_Quando_PostInitialize_Entao_DeveConfigurarPastasEProvedoresExternos()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var settingManager = Substitute.For<ISettingManager>();
                var abpSession = Substitute.For<IAbpSession>();
                var cacheManager = Substitute.For<ICacheManager>();
                var backgroundWorkerManager = Substitute.For<IBackgroundWorkerManager>();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(settingManager),
                    Component.For<IAbpSession>().Instance(abpSession),
                    Component.For<ICacheManager>().Instance(cacheManager),
                    Component.For<IBackgroundWorkerManager>().Instance(backgroundWorkerManager),
                    Component.For<ExternalAuthConfiguration>().Instance(new ExternalAuthConfiguration()),
                    Component.For<AppFolders>().Instance(new AppFolders())
                );

                iocManager.Register<TenantBasedOpenIdConnectExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedGoogleExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedMicrosoftExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedAuthZeroExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);

                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);

                var backgroundJobs = Substitute.For<Abp.BackgroundJobs.IBackgroundJobConfiguration>();
                backgroundJobs.IsJobExecutionEnabled.Returns(true);
                configType.GetProperty("BackgroundJobs")?.SetValue(config, backgroundJobs);

                var auditing = Substitute.For<Abp.Auditing.IAuditingConfiguration>();
                auditing.IsEnabled.Returns(false);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.PostInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireHabilitado_Quando_PostInitialize_Entao_DeveConfigurarStorageELimparJobs()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true,\"IsInMemoryDatabase\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var settingManager = Substitute.For<ISettingManager>();
                var abpSession = Substitute.For<IAbpSession>();
                var cacheManager = Substitute.For<ICacheManager>();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(settingManager),
                    Component.For<IAbpSession>().Instance(abpSession),
                    Component.For<ICacheManager>().Instance(cacheManager),
                    Component.For<ExternalAuthConfiguration>().Instance(new ExternalAuthConfiguration()),
                    Component.For<AppFolders>().Instance(new AppFolders())
                );

                iocManager.Register<TenantBasedOpenIdConnectExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedGoogleExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedMicrosoftExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);
                iocManager.Register<TenantBasedAuthZeroExternalLoginInfoProvider>(Abp.Dependency.DependencyLifeStyle.Singleton);

                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);

                var backgroundJobs = Substitute.For<Abp.BackgroundJobs.IBackgroundJobConfiguration>();
                backgroundJobs.IsJobExecutionEnabled.Returns(true);
                configType.GetProperty("BackgroundJobs")?.SetValue(config, backgroundJobs);

                var auditing = Substitute.For<Abp.Auditing.IAuditingConfiguration>();
                auditing.IsEnabled.Returns(false);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var entityHistory = Substitute.For<Abp.EntityHistory.IEntityHistoryConfiguration>();
                entityHistory.IsEnabled.Returns(false);
                configType.GetProperty("EntityHistory")?.SetValue(config, entityHistory);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.PostInitialize());

                global::Hangfire.JobStorage.Current.ShouldNotBeNull();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }
    }
}
