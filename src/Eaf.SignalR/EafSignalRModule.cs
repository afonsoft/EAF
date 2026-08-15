using System;
using Abp.AspNetCore.SignalR;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetCore.SignalR.Notifications;
using Abp.Modules;
using Abp.Notifications;
using Abp.RealTime;
using Castle.MicroKernel.Registration;
using Eaf.SignalR.Hubs;
using Eaf.SignalR.Notifications;
using Eaf.SignalR.RealTime;

namespace Eaf.SignalR
{
    /// <summary>
    /// Módulo ABP que configura e inicializa o Eaf.SignalR.
    /// </summary>
    [DependsOn(typeof(AbpAspNetCoreSignalRModule))]
    public class EafSignalRModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            RegisterOnlineClientServices();
            RegisterCommonHub();
            ReplaceRealTimeNotifier();
        }

        private void RegisterOnlineClientServices()
        {
            IocManager.IocContainer.Register(
                Component.For(typeof(IOnlineClientStore<>))
                    .ImplementedBy(typeof(EafInMemoryOnlineClientStore<>))
                    .LifestyleSingleton()
                    .IsDefault(),
                Component.For(typeof(IOnlineClientManager<>))
                    .ImplementedBy(typeof(EafOnlineClientManager<>))
                    .LifestyleSingleton()
                    .IsDefault(),
                Component.For<IOnlineClientStore>()
                    .ImplementedBy<EafInMemoryOnlineClientStore>()
                    .LifestyleSingleton()
                    .IsDefault(),
                Component.For<IOnlineClientManager>()
                    .ImplementedBy<EafOnlineClientManager>()
                    .LifestyleSingleton()
                    .IsDefault()
            );
        }

        private void RegisterCommonHub()
        {
            IocManager.IocContainer.Register(Component.For<EafCommonHub>().LifestyleTransient());
        }

        private void ReplaceRealTimeNotifier()
        {
            IocManager.IocContainer.Register(
                Component.For<IRealTimeNotifier>()
                    .ImplementedBy<EafSignalRRealTimeNotifier>()
                    .LifestyleTransient()
                    .IsDefault()
            );

            Configuration.Notifications.Notifiers.Remove<SignalRRealTimeNotifier>();
            Configuration.Notifications.Notifiers.Add<EafSignalRRealTimeNotifier>();
        }
    }
}
