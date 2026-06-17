using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.IO;
using Abp.MailKit;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Worker.Folders;
using Eaf.Middleware.Worker.VirtualFileSystem;
using Eaf.Middleware.Worker.Emailing;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Transactions;

namespace Eaf.Middleware.Worker
{
    [DependsOn(
        typeof(AbpZeroCommonModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule))]
    /// <summary>
    /// Módulo ABP que configura e inicializa MiddlewareWorker.
    /// </summary>
    public class MiddlewareWorkerModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// MiddlewareWorkerModule.
        /// </summary>
        /// <param name="env">Parâmetro env.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareWorkerModule(IHostEnvironment env)
        {
            _env = env;

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
                environmentName = Environment.GetEnvironmentVariable("Hosting:Environment") ?? Environment.GetEnvironmentVariable("ASPNET_ENV");
            if (environmentName.IsNullOrWhiteSpace())
                environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environmentName);

            _appConfiguration = env.GetAppConfiguration();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareWorkerModule).GetAssembly());
        }

        /// <summary>
        /// PostInitialize.
        /// </summary>
        public override void PostInitialize()
        {

            SetAppFolders();

            IocManager.RegisterIfNot<IEafWorkerBase, EafWorkerBase>(DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();

            //Read all config of _appConfiguration
            Configuration.SetConfiguration(_appConfiguration.GetChildren());

            //Configuration for all caches
            Configuration.Caching.ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            });

            //Auditing
            Configuration.Auditing.IsEnabledForAnonymousUsers = false;
            Configuration.Auditing.IsEnabled = false;
            Configuration.EntityHistory.IsEnabled = false;
            Configuration.EntityHistory.IsEnabledForAnonymousUsers = false;

            //https://aspnetboilerplate.com/Pages/Documents/Timing
            Clock.Provider = ClockProviders.Utc;

            // MailKit configuration
            Configuration.Modules.AbpMailKit().SecureSocketOption = SecureSocketOptions.Auto;
            Configuration.ReplaceService<IMailKitSmtpBuilder, MiddlewareMailKitSmtpBuilder>(DependencyLifeStyle.Transient);

            Configuration.ReplaceService(typeof(IEmailSenderConfiguration), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IEmailSenderConfiguration, ISmtpEmailSenderConfiguration>()
                             .ImplementedBy<MiddlewareSmtpEmailSenderConfiguration>()
                             .LifestyleTransient()
                );
            });
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            string contentRootPath = _env.ContentRootPath ?? Directory.GetCurrentDirectory();

            appFolders.ProfileImagesFolder = Path.Combine(contentRootPath, "ProfileImages");
            appFolders.LogsFolder = Path.Combine(contentRootPath, "Logs");
            appFolders.DownloadFolder = Path.Combine(contentRootPath, "Downloads");
            appFolders.TempFolder = Path.GetTempPath();
            appFolders.DataFolder = Path.Combine(contentRootPath, "data");

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.ProfileImagesFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.LogsFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.DownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.TempFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.DataFolder);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "SetAppFolders {0}", ex.Message);
            }

            try
            {
                appFolders.RootFileProvider = new CompositeFileProvider(
                    appFolders.RootFileProvider,
                    IocManager.Resolve<IWorkerContentFileProvider>()
                );
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "RootFileProvider {0}", ex.Message);
            }

        }
    }
}