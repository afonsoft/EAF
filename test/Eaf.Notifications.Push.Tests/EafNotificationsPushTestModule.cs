using System.Net.Http;
using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Eaf.Notifications.Push.Configuration;
using Eaf.Notifications.Push.Tests.Fakes;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Eaf.Notifications.Push.Tests
{
    /// <summary>
    /// Módulo de testes para Eaf.Notifications.Push.
    /// </summary>
    [DependsOn(typeof(EafNotificationsPushModule), typeof(AbpTestBaseModule))]
    public class EafNotificationsPushTestModule : AbpModule
    {
        /// <summary>
        /// PreInitialize: registra as opções e o IHttpClientFactory fake.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IOptions<PushOptions>>()
                    .Instance(Options.Create(new PushOptions()))
                    .LifestyleSingleton());

            var handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(handler);
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient("EafPush").Returns(httpClient);

            IocManager.IocContainer.Register(
                Component.For<IHttpClientFactory>()
                    .Instance(httpClientFactory)
                    .LifestyleSingleton());
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafNotificationsPushTestModule).GetAssembly());
        }
    }
}
