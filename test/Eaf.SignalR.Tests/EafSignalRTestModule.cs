using System.Reflection;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Eaf.SignalR;
using Eaf.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;

namespace Eaf.SignalR.Tests
{
    /// <summary>
    /// Módulo de teste para Eaf.SignalR.
    /// </summary>
    [DependsOn(typeof(EafSignalRModule), typeof(AbpTestBaseModule))]
    public class EafSignalRTestModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafSignalRTestModule).Assembly);

            IocManager.IocContainer.Register(
                Component.For<IHubContext<EafCommonHub>>()
                    .Instance(Substitute.For<IHubContext<EafCommonHub>>())
                    .LifestyleSingleton()
            );
        }
    }
}
