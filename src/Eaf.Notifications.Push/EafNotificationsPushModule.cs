using System.Linq;
using Abp;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Eaf.Notifications.Push.Configuration;
using Eaf.Notifications.Push.Providers;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Módulo ABP para envio de notificações push através de providers configuráveis.
    /// </summary>
    [DependsOn(
        typeof(AbpKernelModule)
    )]
    public class EafNotificationsPushModule : AbpModule
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IPushNotificationProvider>()
                    .ImplementedBy<WebPushNotificationProvider>()
                    .LifestyleTransient()
                    .Named("EafPushWebPush"),
                Component.For<IPushNotificationProvider>()
                    .ImplementedBy<GenericHttpPushProvider>()
                    .LifestyleTransient()
                    .Named("EafPushGenericHttp"),
                Component.For<IPushNotificationSender>()
                    .UsingFactoryMethod((kernel, context) =>
                    {
                        var options = kernel.Resolve<IOptions<PushOptions>>();
                        var providers = kernel.ResolveAll<IPushNotificationProvider>();
                        return new EafPushNotificationSender(options, providers);
                    })
                    .LifestyleTransient()
                    .IsDefault()
            );
        }
    }
}
