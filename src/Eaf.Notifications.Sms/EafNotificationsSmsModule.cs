using System.Linq;
using Abp;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Eaf.Notifications.Sms.Configuration;
using Eaf.Notifications.Sms.Providers;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Módulo ABP para envio de SMS através de providers configuráveis.
    /// </summary>
    [DependsOn(
        typeof(AbpKernelModule)
    )]
    public class EafNotificationsSmsModule : AbpModule
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.IocContainer.Register(
                Component.For<ISmsProvider>()
                    .ImplementedBy<GenericHttpSmsProvider>()
                    .LifestyleTransient()
                    .Named("EafSmsGenericHttp"),
                Component.For<ISmsProvider>()
                    .ImplementedBy<TwilioSmsProvider>()
                    .LifestyleTransient()
                    .Named("EafSmsTwilio"),
                Component.For<ISmsSender>()
                    .UsingFactoryMethod((kernel, context) =>
                    {
                        var options = kernel.Resolve<IOptions<SmsOptions>>();
                        var providers = kernel.ResolveAll<ISmsProvider>();
                        return new EafSmsSender(options, providers);
                    })
                    .LifestyleTransient()
                    .IsDefault()
            );
        }
    }
}
