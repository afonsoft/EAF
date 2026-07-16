using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Hangfire;
using Abp.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Threading.BackgroundWorkers;
using Abp.Timing;
using Abp.Zero.Configuration;
using Eaf.AspNetCore.SignalR.Chat;
using Eaf.Hangfire;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;
using Eaf.Middleware.Web.Auditing;
using Eaf.Middleware.Web.Configuration;
using Eaf.Middleware.Web.Features;
using Eaf.Middleware.Web.Notifications;
using Eaf.Middleware.Web.Session;
using Eaf.Middleware.Web.WebHooks;
using Eaf.Runtime.Caching.SqlServer;
using Hangfire;
using Hangfire.InMemory;
using Hangfire.Redis.StackExchange;
using Hangfire.SqlServer;
using Hangfire.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Eaf.Middleware.Web
{
    [DependsOn(
        typeof(MiddlewareApplicationModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpHangfireAspNetCoreModule),
        typeof(AbpRedisCacheModule),
        typeof(EafSqlServerCacheModule)
    )]
    /// <summary>
    /// Módulo ABP que configura e inicializa MiddlewareWebCore.
    /// </summary>
    public class MiddlewareWebCoreModule : AbpModule
    {
        private const string HangfireIsEnabledKey = "Hangfire:IsEnabled";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// MiddlewareWebCoreModule.
        /// </summary>
        /// <param name="env">Parâmetro env.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareWebCoreModule(IHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("Hosting:Environment") ?? Environment.GetEnvironmentVariable("ASPNET_ENV");
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", environmentName);
            Environment.SetEnvironmentVariable("ASPNET_ENV", environmentName);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareWebCoreModule).GetAssembly());
            IocManager.RegisterIfNot<IChatCommunicator, SignalRChatCommunicator>();
            if ((_appConfiguration["RedisCache:IsRedisEnabled"] != null && bool.Parse(_appConfiguration["RedisCache:IsRedisEnabled"]))
                || _appConfiguration["RedisCache:IsEnabled"] != null && bool.Parse(_appConfiguration["RedisCache:IsEnabled"]))
            {
                IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCorePerRequestRedisCacheModule).GetAssembly());
            }
        }

        /// <summary>
        /// PostInitialize.
        /// </summary>
        public override void PostInitialize()
        {
            SetAppFolders();
            ConfigureExternalAuthProviders();
            ConfigureBackgroundJobs();
        }

        private void ConfigureBackgroundJobs()
        {
            if (!Configuration.BackgroundJobs.IsJobExecutionEnabled)
                return;

            bool.TryParse(_appConfiguration[HangfireIsEnabledKey], out bool isEnabled);

            if (_appConfiguration[HangfireIsEnabledKey] == null || !isEnabled)
            {
                AddExpiredAuditLogDeleterWorker();
                return;
            }

            ConfigureHangfireStorage();
            RemoveOutdatedHangfireJobs();

            Configuration.Auditing.SetExpiredAuditWoker();
            Configuration.EntityHistory.SetExpiredHistoryEntityWoker();
        }

        private void AddExpiredAuditLogDeleterWorker()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            if (Configuration.Auditing.IsEnabled)
                workManager.Add(IocManager.Resolve<ExpiredAuditLogDeleterWorker>());
        }

        private void ConfigureHangfireStorage()
        {
            string connectionString = Configuration.DefaultNameOrConnectionString;
            var storageType = Middleware.Web.Startup.HangFireConfigurer.ResolveStorageType(_appConfiguration);

            switch (storageType)
            {
                case HangfireStorageType.SqlServer:
                    if (!string.IsNullOrEmpty(connectionString))
                        JobStorage.Current = new SqlServerStorage(connectionString, new SqlServerStorageOptions() { TransactionTimeout = TimeSpan.FromMinutes(30) });
                    else
                        JobStorage.Current = new InMemoryStorage();
                    break;

                case HangfireStorageType.Redis:
                    var redisConnectionString = _appConfiguration["RedisCache:ConnectionString"] ?? "localhost";
                    var redisDatabaseId = 0;
                    if (_appConfiguration["RedisCache:DatabaseId"] != null)
                        int.TryParse(_appConfiguration["RedisCache:DatabaseId"], out redisDatabaseId);

                    JobStorage.Current = new RedisStorage(redisConnectionString, new RedisStorageOptions
                    {
                        Db = redisDatabaseId,
                        Prefix = "hangfire:"
                    });
                    break;

                default:
                    JobStorage.Current = new InMemoryStorage();
                    break;
            }
        }

        private void RemoveOutdatedHangfireJobs()
        {
            Logger.Info("Removing outdated Job in HangFire");
            try
            {
                using (var connection = JobStorage.Current.GetConnection())
                {
                    RemoveOutdatedRecurringJobs(connection);
                    RemoveOutdatedFailedJobs();
                }
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "Error on removing outdated job : {0}", ex.Message);
            }
        }

        private void RemoveOutdatedRecurringJobs(IStorageConnection connection)
        {
            foreach (var recurringJob in connection.GetRecurringJobs())
            {
                if (recurringJob.Removed
                    || (recurringJob.LastExecution < Clock.Now.AddMonths(-1)
                        && recurringJob.LastJobState != "Succeeded"))
                {
                    Logger.DebugFormat("Removing outdated Job {0}", recurringJob.Id);
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }

        private void RemoveOutdatedFailedJobs()
        {
            var api = JobStorage.Current.GetMonitoringApi();
            foreach (var failedJob in api.FailedJobs(0, 1000))
            {
                if (failedJob.Value.FailedAt < Clock.Now.AddMonths(-1))
                {
                    BackgroundJob.Delete(failedJob.Key);
                    Logger.DebugFormat("Removing outdated Job {0}", failedJob.Key);
                }
            }
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            //https://learn.microsoft.com/pt-br/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(MiddlewareApplicationModule).GetAssembly()
                );

            Configuration.IocManager.RegisterIfNot<IPerRequestSessionCache, PerRequestSessionCache>();
            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();

            //Read all config of _appConfiguration
            Configuration.SetConfiguration(_appConfiguration.GetChildren());

            //App configurations
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"];

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled && _appConfiguration[HangfireIsEnabledKey] != null && bool.Parse(_appConfiguration[HangfireIsEnabledKey]))
            {
                //hangfire
                Configuration.BackgroundJobs.UseHangfire();
            }

            //Cache configuration (Redis, SQL Server)
            CacheConfigurer.Configure(Configuration, _appConfiguration, IocManager);

            //Auditing and Entity History
            AuditConfigurer.Configure(Configuration);

            //https://aspnetboilerplate.com/Pages/Documents/Timing
            Clock.Provider = ClockProviders.Utc;

            //Adding notification providers
            Configuration.Notifications.Providers.Add<MiddlewareNotificationProvider>();

            //Adding feature providers
            Configuration.Features.Providers.Add<MiddlewareFeatureProvider>();

            //Adding Webhook providers
            Configuration.Webhooks.Providers.Add<EafWebhookDefinitionProvider>();

            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //For�a a usar a formata��o do servi�o escolhido pelo usu�rio
            Configuration.Modules.AbpAspNetCore().UseMvcDateTimeFormatForAppServices = true;
        }

        private void ConfigureExternalAuthProviders()
        {
            ExternalAuthConfigurer.Configure(IocManager);
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            string contentRootPath = _env.ContentRootPath ?? Directory.GetCurrentDirectory();
            string webRootPath = "wwwroot";

            appFolders.ProfileImagesFolder = Path.Combine(contentRootPath, webRootPath, "ProfileImages");
            appFolders.WebLogsFolder = Path.Combine(contentRootPath, webRootPath, "Logs");
            appFolders.WebDownloadFolder = Path.Combine(contentRootPath, webRootPath, "Downloads");
            appFolders.WebTempFolder = Path.Combine(contentRootPath, webRootPath, "Temp");
            appFolders.WebDataFolder = Path.Combine(contentRootPath, webRootPath);

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.ProfileImagesFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WebLogsFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WebDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WebTempFolder);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "SetAppFolders {0}", ex.Message);
            }

            try
            {
                appFolders.WebRootFileProvider = new CompositeFileProvider(
                    appFolders.WebRootFileProvider
                );
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "WebRootFileProvider {0}", ex.Message);
            }
        }
    }
}
