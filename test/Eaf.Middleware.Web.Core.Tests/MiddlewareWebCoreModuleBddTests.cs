using Abp.Application.Features;
using Abp.AspNetCore.Configuration;
using Abp.Auditing;
using Abp.BackgroundJobs;
using Abp.Collections;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Session;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Web.Configuration;
using Abp.Web.MultiTenancy;
using Abp.Webhooks;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using Eaf.Middleware.Web;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Web.Auditing;
using Eaf.Middleware.Web.Features;
using Eaf.Middleware.Web.Notifications;
using Eaf.Middleware.Web.WebHooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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

        [Fact]
        public void Dado_HangfireSqlServerSemConnectionString_Quando_PostInitialize_Entao_DeveUsarInMemoryStorage()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

        [Fact]
        public void Dado_HangfireRedisHabilitado_Quando_PostInitialize_Entao_DeveConfigurarRedisStorage()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true},\"Database\":{\"Provider\":\"MySql\"},\"RedisCache\":{\"IsEnabled\":true,\"ConnectionString\":\"localhost:6379,abortConnect=false,connectTimeout=1\"}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

                global::Hangfire.JobStorage.Current.ShouldBeOfType<global::Hangfire.Redis.StackExchange.RedisStorage>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireRedisComDatabaseId_Quando_PostInitialize_Entao_DeveConfigurarRedisStorageComDatabaseId()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true},\"Database\":{\"Provider\":\"MySql\"},\"RedisCache\":{\"IsEnabled\":true,\"ConnectionString\":\"localhost:6379,abortConnect=false,connectTimeout=1\",\"DatabaseId\":\"5\"}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

                global::Hangfire.JobStorage.Current.ShouldBeOfType<global::Hangfire.Redis.StackExchange.RedisStorage>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireSqlServerComConnectionString_Quando_PostInitialize_Entao_DeveConfigurarSqlServerStorage()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

                configType.GetProperty("DefaultNameOrConnectionString")?.SetValue(config, "Server=.;Database=hangfire;Connect Timeout=1;");

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.PostInitialize());

                global::Hangfire.JobStorage.Current.ShouldBeOfType<global::Hangfire.SqlServer.SqlServerStorage>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostEnvironmentComContentRootInvalido_Quando_PostInitialize_Entao_DeveCapturarExcecoesSemLancar()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns("/proc");
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
                backgroundJobs.IsJobExecutionEnabled.Returns(false);
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
        public void Dado_HostEnvironmentComContentRootInvalido_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalEaf = Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            var originalHosting = Environment.GetEnvironmentVariable("Hosting:Environment");
            var originalAspNet = Environment.GetEnvironmentVariable("ASPNET_ENV");
            var originalDotNet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", null);
                Environment.SetEnvironmentVariable("ASPNET_ENV", null);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Staging");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Staging");

                var module = new MiddlewareWebCoreModule(env);

                module.ShouldNotBeNull();
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Staging");
                Environment.GetEnvironmentVariable("EAF_ENVIRONMENT").ShouldBe("Staging");
                Environment.GetEnvironmentVariable("ASPNET_ENV").ShouldBe("Staging");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", originalEaf);
                Environment.SetEnvironmentVariable("Hosting:Environment", originalHosting);
                Environment.SetEnvironmentVariable("ASPNET_ENV", originalAspNet);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotNet);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireDesabilitadoComAuditingHabilitado_Quando_PostInitialize_Entao_DeveRegistrarExpiredAuditLogDeleterWorker()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var workManager = Substitute.For<IBackgroundWorkerManager>();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
                    Component.For<IBackgroundWorkerManager>().Instance(workManager),
                    Component.For<ExternalAuthConfiguration>().Instance(new ExternalAuthConfiguration()),
                    Component.For<AppFolders>().Instance(new AppFolders()),
                    Component.For<IRepository<AuditLog, long>>().Instance(Substitute.For<IRepository<AuditLog, long>>()),
                    Component.For<IRepository<Tenant>>().Instance(Substitute.For<IRepository<Tenant>>())
                );

                iocManager.Register<AbpTimer>(Abp.Dependency.DependencyLifeStyle.Transient);
                iocManager.Register<ExpiredAuditLogDeleterWorker>(Abp.Dependency.DependencyLifeStyle.Singleton);

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
                auditing.IsEnabled.Returns(true);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var entityHistory = Substitute.For<Abp.EntityHistory.IEntityHistoryConfiguration>();
                entityHistory.IsEnabled.Returns(false);
                configType.GetProperty("EntityHistory")?.SetValue(config, entityHistory);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.PostInitialize());

                workManager.Received().Add(Arg.Any<ExpiredAuditLogDeleterWorker>());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_ConfiguracaoInicializada_Quando_PreInitialize_Entao_DeveConfigurarModulos()
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
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, CriarConfiguracao(iocManager));

                Should.NotThrow(() => module.PreInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireHabilitado_Quando_PreInitialize_Entao_DeveUsarHangfire()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configuration = CriarConfiguracao(iocManager);
                configuration.BackgroundJobs.IsJobExecutionEnabled.Returns(true);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, configuration);

                Should.NotThrow(() => module.PreInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_RedisDesabilitado_Quando_Initialize_Entao_NaoDeveRegistrarRedisAssembly()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"RedisCache\":{\"IsRedisEnabled\":false,\"IsEnabled\":false}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_RedisEnabledHabilitadoIsRedisEnabledDesabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"RedisCache\":{\"IsRedisEnabled\":false,\"IsEnabled\":true}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);
                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_BackgroundJobsHabilitadoHangfireDesabilitado_Quando_PreInitialize_Entao_NaoDeveUsarHangfire()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":false}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configuration = CriarConfiguracao(iocManager);
                configuration.BackgroundJobs.IsJobExecutionEnabled.Returns(true);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, configuration);

                Should.NotThrow(() => module.PreInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireInMemoryHabilitadoAuditingEnabled_Quando_PostInitialize_Entao_DeveRegistrarExpiredWorkers()
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

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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
                auditing.IsEnabled.Returns(true);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var entityHistory = Substitute.For<Abp.EntityHistory.IEntityHistoryConfiguration>();
                entityHistory.IsEnabled.Returns(true);
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

        [Fact]
        public void Dado_HangfireInMemoryDefaultComMySqlERedisDesabilitado_Quando_PostInitialize_Entao_DeveConfigurarInMemoryStorage()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true},\"Database\":{\"Provider\":\"MySql\"},\"RedisCache\":{\"IsEnabled\":false}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

                global::Hangfire.JobStorage.Current.ShouldBeOfType<global::Hangfire.InMemory.InMemoryStorage>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_VariaveisAmbienteNaoDefinidas_Quando_CriarModulo_Entao_DeveResolverPelaVariavelDotnetEnvironment()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            var originalAspNetCore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var originalEaf = Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            var originalHosting = Environment.GetEnvironmentVariable("Hosting:Environment");
            var originalAspNet = Environment.GetEnvironmentVariable("ASPNET_ENV");
            var originalDotnet = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
                Environment.SetEnvironmentVariable("Hosting:Environment", null);
                Environment.SetEnvironmentVariable("ASPNET_ENV", null);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns((string)null!);

                var module = new MiddlewareWebCoreModule(env);
                module.ShouldNotBeNull();
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", originalAspNetCore);
                Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", originalEaf);
                Environment.SetEnvironmentVariable("Hosting:Environment", originalHosting);
                Environment.SetEnvironmentVariable("ASPNET_ENV", originalAspNet);
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", originalDotnet);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_ContentRootPathNulo_Quando_PostInitialize_Entao_DeveUsarDiretorioAtualEConfigurarPastas()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalCurrentDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
                    Component.For<ExternalAuthConfiguration>().Instance(new ExternalAuthConfiguration()),
                    Component.For<AppFolders>().Instance(new AppFolders { WebRootFileProvider = null! })
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
                backgroundJobs.IsJobExecutionEnabled.Returns(false);
                configType.GetProperty("BackgroundJobs")?.SetValue(config, backgroundJobs);

                var auditing = Substitute.For<Abp.Auditing.IAuditingConfiguration>();
                auditing.IsEnabled.Returns(false);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                env.ContentRootPath.Returns((string)null!);

                Should.NotThrow(() => module.PostInitialize());

                var appFolders = iocManager.Resolve<AppFolders>();
                appFolders.WebRootFileProvider.ShouldNotBeNull();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCurrentDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireRedisComDatabaseIdInvalido_Quando_PostInitialize_Entao_DeveConfigurarRedisStorageComDatabaseIdZero()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), "{\"Hangfire\":{\"IsEnabled\":true},\"Database\":{\"Provider\":\"MySql\"},\"RedisCache\":{\"IsEnabled\":true,\"ConnectionString\":\"localhost:6379,abortConnect=false,connectTimeout=1\",\"DatabaseId\":\"abc\"}}");

                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
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

                global::Hangfire.JobStorage.Current.ShouldBeOfType<global::Hangfire.Redis.StackExchange.RedisStorage>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HangfireNaoConfiguradoComAuditingHabilitado_Quando_PostInitialize_Entao_DeveRegistrarExpiredAuditLogDeleterWorker()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var workManager = Substitute.For<IBackgroundWorkerManager>();

                iocManager.IocContainer.Register(
                    Component.For<ISettingManager>().Instance(Substitute.For<ISettingManager>()),
                    Component.For<IAbpSession>().Instance(Substitute.For<IAbpSession>()),
                    Component.For<ICacheManager>().Instance(Substitute.For<ICacheManager>()),
                    Component.For<IBackgroundWorkerManager>().Instance(workManager),
                    Component.For<ExternalAuthConfiguration>().Instance(new ExternalAuthConfiguration()),
                    Component.For<AppFolders>().Instance(new AppFolders()),
                    Component.For<IRepository<AuditLog, long>>().Instance(Substitute.For<IRepository<AuditLog, long>>()),
                    Component.For<IRepository<Tenant>>().Instance(Substitute.For<IRepository<Tenant>>())
                );

                iocManager.Register<AbpTimer>(Abp.Dependency.DependencyLifeStyle.Transient);
                iocManager.Register<ExpiredAuditLogDeleterWorker>(Abp.Dependency.DependencyLifeStyle.Singleton);

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
                auditing.IsEnabled.Returns(true);
                configType.GetProperty("Auditing")?.SetValue(config, auditing);

                var entityHistory = Substitute.For<Abp.EntityHistory.IEntityHistoryConfiguration>();
                entityHistory.IsEnabled.Returns(false);
                configType.GetProperty("EntityHistory")?.SetValue(config, entityHistory);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.PostInitialize());

                workManager.Received().Add(Arg.Any<ExpiredAuditLogDeleterWorker>());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_BackgroundJobsHabilitadoHangfireNaoConfigurado_Quando_PreInitialize_Entao_NaoDeveUsarHangfire()
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
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configuration = CriarConfiguracao(iocManager);
                configuration.BackgroundJobs.IsJobExecutionEnabled.Returns(true);

                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance);
                configProperty?.SetValue(module, configuration);

                Should.NotThrow(() => module.PreInitialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        private static IAbpStartupConfiguration CriarConfiguracao(IIocManager iocManager)
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            configuration.IocManager.Returns(iocManager);
            configuration.DefaultNameOrConnectionString.Returns(string.Empty);

            var modules = Substitute.For<IModuleConfigurations>();
            modules.AbpConfiguration.Returns(configuration);
            configuration.Modules.Returns(modules);

            var aspNetCoreConfiguration = Substitute.For<IAbpAspNetCoreConfiguration>();
            configuration.Get<IAbpAspNetCoreConfiguration>().Returns(aspNetCoreConfiguration);

            var webCommonConfiguration = Substitute.For<IAbpWebCommonModuleConfiguration>();
            var webMultiTenancyConfiguration = Substitute.For<IWebMultiTenancyConfiguration>();
            webCommonConfiguration.MultiTenancy.Returns(webMultiTenancyConfiguration);
            configuration.Get<IAbpWebCommonModuleConfiguration>().Returns(webCommonConfiguration);

            var zeroConfig = Substitute.For<IAbpZeroConfig>();
            var languageManagementConfig = Substitute.For<ILanguageManagementConfig>();
            zeroConfig.LanguageManagement.Returns(languageManagementConfig);
            configuration.Get<IAbpZeroConfig>().Returns(zeroConfig);

            configuration.Notifications.Returns(Substitute.For<INotificationConfiguration>());
            configuration.Notifications.Providers.Returns(new TypeList<NotificationProvider>());
            configuration.Features.Returns(Substitute.For<IFeatureConfiguration>());
            configuration.Features.Providers.Returns(new TypeList<FeatureProvider>());
            configuration.Webhooks.Returns(Substitute.For<IWebhooksConfiguration>());
            configuration.Webhooks.Providers.Returns(new TypeList<WebhookDefinitionProvider>());
            configuration.Caching.Returns(Substitute.For<ICachingConfiguration>());
            configuration.Auditing.Returns(Substitute.For<IAuditingConfiguration>());
            configuration.EntityHistory.Returns(Substitute.For<IEntityHistoryConfiguration>());
            configuration.BackgroundJobs.Returns(Substitute.For<IBackgroundJobConfiguration>());

            return configuration;
        }
    }
}
