using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Notifications;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Eaf.Middleware.Notifications;
using Eaf.MiddlewareCore.SampleApp;

namespace Eaf.Middleware
{
    [DependsOn(typeof(EafMiddlewareCoreSampleAppModule),
        typeof(AbpTestBaseModule))]
    public class EafMiddlewareTestModule : AbpModule
    {
        public EafMiddlewareTestModule(EafMiddlewareCoreSampleAppModule sampleAppModule)
        {
            sampleAppModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper is removed
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the UseStaticMapper is removed
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.UnitOfWork.IsTransactional = false;

            Configuration.Settings.Providers.Add<FakeAzureActiveDirectorySettingProvider>();

            Configuration.Notifications.Providers.Add<FakeNotificationProvider>();

            Configuration.ReplaceService<INotificationDistributer, FakeNotificationDistributer>();
        }

        public override void Initialize()
        {
            TestServiceCollectionRegistrar.Register(IocManager);
            IocManager.RegisterAssemblyByConvention(typeof(EafMiddlewareTestModule).GetAssembly());
            IocManager.IocContainer.Register(
                Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>()
            );
        }
    }
}