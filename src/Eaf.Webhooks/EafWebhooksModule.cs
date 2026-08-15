using System;
using Abp;
using Abp.Modules;
using Abp.Webhooks;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Eaf.Webhooks.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Módulo ABP que configura e inicializa o Eaf.Webhooks.
    /// </summary>
    [DependsOn(
        typeof(AbpKernelModule)
    )]
    public class EafWebhooksModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IWebhookSubscriptionSecretProtector>()
                    .UsingFactoryMethod((kernel, context) =>
                    {
                        var options = ResolveOptionsIfAvailable(kernel);

                        if (kernel.HasComponent(typeof(IDataProtectionProvider)))
                        {
                            var provider = kernel.Resolve<IDataProtectionProvider>();
                            return (IWebhookSubscriptionSecretProtector)new EafDataProtectionWebhookSecretProtector(provider, options);
                        }

                        return new EafPlainWebhookSecretProtector();
                    })
                    .LifestyleTransient()
                    .IsDefault());
        }

        public override void Initialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IWebhookManager>().ImplementedBy<EafWebhookManager>().LifestyleTransient().IsDefault().Named("EafWebhookManager"),
                Component.For<IWebhookSubscriptionManager>().ImplementedBy<EafWebhookSubscriptionManager>().LifestyleTransient().IsDefault().Named("EafWebhookSubscriptionManager"),
                Component.For<IWebhookSender>().ImplementedBy<EafWebhookSender>().LifestyleTransient().IsDefault().Named("EafWebhookSender")
            );
        }

        public override void PostInitialize()
        {
            var options = ResolveOptionsIfAvailable(IocManager.IocContainer.Kernel).Value;
            var webhooksConfiguration = Configuration.Webhooks;

            if (options.TimeoutSeconds > 0)
                webhooksConfiguration.TimeoutDuration = TimeSpan.FromSeconds(options.TimeoutSeconds);

            if (options.MaxSendAttemptCount > 0)
                webhooksConfiguration.MaxSendAttemptCount = options.MaxSendAttemptCount;

            webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled = options.IsAutomaticSubscriptionDeactivationEnabled;

            if (options.MaxConsecutiveFailCountBeforeDeactivateSubscription > 0)
                webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription = options.MaxConsecutiveFailCountBeforeDeactivateSubscription;

            webhooksConfiguration.JsonSerializerOptions = options.JsonSerializerOptions;
        }

        private static IOptions<EafWebhooksOptions> ResolveOptionsIfAvailable(IKernel kernel)
        {
            if (kernel.HasComponent(typeof(IOptions<EafWebhooksOptions>)))
                return kernel.Resolve<IOptions<EafWebhooksOptions>>();

            return Options.Create(new EafWebhooksOptions());
        }
    }
}
