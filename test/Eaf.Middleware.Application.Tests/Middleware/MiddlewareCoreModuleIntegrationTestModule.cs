using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Eaf.Middleware;

namespace Eaf.Middleware.Application.Tests.Middleware
{
    [DependsOn(typeof(MiddlewareCoreModule))]
    public class MiddlewareCoreModuleIntegrationTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.UnitOfWork.IsTransactional = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareCoreModuleIntegrationTestModule).GetAssembly());
        }
    }
}
