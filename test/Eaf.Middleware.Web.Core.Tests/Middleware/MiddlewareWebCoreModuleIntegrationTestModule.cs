using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using NSubstitute;

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
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareWebCoreModuleIntegrationTestModule).GetAssembly());
        }
    }
}
