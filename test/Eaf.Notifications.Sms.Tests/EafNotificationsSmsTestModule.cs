using System.Net.Http;
using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Eaf.Notifications.Sms.Configuration;
using Eaf.Notifications.Sms.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Eaf.Notifications.Sms.Tests
{
    /// <summary>
    /// Módulo de testes para Eaf.Notifications.Sms.
    /// </summary>
    [DependsOn(typeof(EafNotificationsSmsModule), typeof(AbpTestBaseModule))]
    public class EafNotificationsSmsTestModule : AbpModule
    {
        /// <summary>
        /// PreInitialize: registra as opções e o IHttpClientFactory fake.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IOptions<SmsOptions>>()
                    .Instance(Options.Create(new SmsOptions()))
                    .LifestyleSingleton());

            var handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(handler);
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient("EafSms").Returns(httpClient);

            IocManager.IocContainer.Register(
                Component.For<IHttpClientFactory>()
                    .Instance(httpClientFactory)
                    .LifestyleSingleton());
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafNotificationsSmsTestModule).GetAssembly());
        }
    }
}
