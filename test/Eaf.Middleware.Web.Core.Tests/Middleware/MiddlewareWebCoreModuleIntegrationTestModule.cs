using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.MicroKernel.Registration;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Core.Tests.Middleware
{
    [DependsOn(typeof(MiddlewareWebCoreModule))]
    public class MiddlewareWebCoreModuleIntegrationTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.UnitOfWork.IsTransactional = false;

            IocManager.IocContainer.Register(
                Component.For<IUserFriendsCache>()
                    .Instance(Substitute.For<IUserFriendsCache>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<ISettingManager>()
                    .Instance(Substitute.For<ISettingManager>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<IAbpSession>()
                    .Instance(Substitute.For<IAbpSession>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<ICacheManager>()
                    .Instance(Substitute.For<ICacheManager>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<IApplicationLanguageManager>()
                    .Instance(Substitute.For<IApplicationLanguageManager>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<IChatCommunicator>()
                    .Instance(Substitute.For<IChatCommunicator>())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            IocManager.IocContainer.Register(
                Component.For<ApplicationPartManager>()
                    .Instance(new ApplicationPartManager())
                    .LifestyleSingleton()
            );
            IocManager.IocContainer.Register(
                Component.For<IOptions<AntiforgeryOptions>>()
                    .Instance(Options.Create(new AntiforgeryOptions()))
                    .LifestyleSingleton()
            );
            IocManager.IocContainer.Register(
                Component.For<IBackgroundWorkerManager>()
                    .Instance(Substitute.For<IBackgroundWorkerManager>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<AbpTimer>()
                    .Instance(Substitute.For<AbpTimer>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<IRepository<AuditLog, long>>()
                    .Instance(Substitute.For<IRepository<AuditLog, long>>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.IocContainer.Register(
                Component.For<IRepository<Tenant>>()
                    .Instance(Substitute.For<IRepository<Tenant>>())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            var backgroundJobStore = Substitute.For<IBackgroundJobStore>();
            backgroundJobStore.GetWaitingJobsAsync(Arg.Any<int>()).Returns(Task.FromResult(new List<BackgroundJobInfo>()));
            backgroundJobStore.GetWaitingJobs(Arg.Any<int>()).Returns(new List<BackgroundJobInfo>());
            IocManager.IocContainer.Register(
                Component.For<IBackgroundJobStore>()
                    .Instance(backgroundJobStore)
                    .LifestyleSingleton()
                    .IsDefault()
            );

            IocManager.IocContainer.Register(
                Component.For<IRepository<UserToken, long>>()
                    .Instance(Substitute.For<IRepository<UserToken, long>>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareWebCoreModuleIntegrationTestModule).GetAssembly());
        }
    }
}
