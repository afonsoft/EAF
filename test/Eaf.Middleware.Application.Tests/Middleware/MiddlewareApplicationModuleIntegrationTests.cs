using Abp;
using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.RealTime;
using Castle.MicroKernel.Registration;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Middleware
{
    public class MiddlewareApplicationModuleIntegrationTests
    {
        [Fact]
        public void Dado_MiddlewareApplicationModule_Quando_ExecutarCicloDeVida_Entao_DeveCompletarSemErros()
        {
            using var bootstrapper = Abp.AbpBootstrapper.Create<MiddlewareCoreModuleIntegrationTestModule>();
            bootstrapper.Initialize();

            var iocManager = bootstrapper.IocManager;
            var configuration = iocManager.Resolve<IAbpStartupConfiguration>();

            iocManager.IocContainer.Register(
                Component.For<IUserFriendsCache>()
                    .Instance(Substitute.For<IUserFriendsCache>())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            var module = new MiddlewareApplicationModule();
            var moduleType = typeof(AbpModule);
            moduleType.GetProperty("IocManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, iocManager);
            moduleType.GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!.SetValue(module, configuration);

            Should.NotThrow(() => module.PreInitialize());
            Should.NotThrow(() => module.Initialize());
            Should.NotThrow(() => module.PostInitialize());

            iocManager.IsRegistered<IChatCommunicator>().ShouldBeTrue();
            iocManager.IsRegistered<IAuditingStore>().ShouldBeTrue();
        }
    }
}
