using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Eaf.Middleware;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;

namespace Eaf.MiddlewareCore.Tests.Middleware
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
            IocManager.IocContainer.Register(
                Component.For<IUserFriendsCache>()
                    .Instance(Substitute.For<IUserFriendsCache>())
                    .LifestyleSingleton()
                    .IsDefault()
            );
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareCoreModuleIntegrationTestModule).GetAssembly());
        }
    }
}
