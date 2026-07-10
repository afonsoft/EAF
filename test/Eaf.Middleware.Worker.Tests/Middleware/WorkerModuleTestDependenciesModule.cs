using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.MailKit;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using Abp.Zero.Configuration;

namespace Eaf.Middleware.Worker.Tests.Middleware
{
    [DependsOn(
        typeof(AbpZeroCommonModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule))]
    public class WorkerModuleTestDependenciesModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.UnitOfWork.IsTransactional = false;

            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(WorkerTestTenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(WorkerTestRole);
            Configuration.Modules.Zero().EntityTypes.User = typeof(WorkerTestUser);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WorkerModuleTestDependenciesModule).GetAssembly());
        }
    }
}
