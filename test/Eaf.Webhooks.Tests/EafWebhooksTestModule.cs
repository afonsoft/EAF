using System.Net.Http;
using Abp.AspNetCore.Webhook;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Webhooks;
using Castle.MicroKernel.Registration;
using Eaf.Webhooks.Configuration;
using Eaf.Webhooks.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Eaf.Webhooks.Tests
{
    /// <summary>
    /// Módulo de testes para Eaf.Webhooks.
    /// </summary>
    [DependsOn(typeof(EafWebhooksModule), typeof(AbpTestBaseModule))]
    public class EafWebhooksTestModule : AbpModule
    {
        /// <summary>
        /// PreInitialize: registra as opções e o IHttpClientFactory fake para os testes.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IOptions<EafWebhooksOptions>>()
                    .Instance(Options.Create(new EafWebhooksOptions()))
                    .LifestyleSingleton());

            IocManager.IocContainer.Register(
                Component.For<IWebhookSendAttemptStore>().Instance(Substitute.For<IWebhookSendAttemptStore>()).LifestyleSingleton(),
                Component.For<IWebhookSubscriptionsStore>().Instance(Substitute.For<IWebhookSubscriptionsStore>()).LifestyleSingleton(),
                Component.For<IWebhooksConfiguration>().Instance(Substitute.For<IWebhooksConfiguration>()).LifestyleSingleton());

            var handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(handler);
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName).Returns(httpClient);

            IocManager.IocContainer.Register(
                Component.For<IHttpClientFactory>()
                    .Instance(httpClientFactory)
                    .LifestyleSingleton());
        }

        /// <summary>
        /// Initialize: registra os tipos deste assembly por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafWebhooksTestModule).GetAssembly());
        }
    }
}
