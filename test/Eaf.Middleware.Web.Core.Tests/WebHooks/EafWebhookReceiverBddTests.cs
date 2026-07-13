using Abp;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Localization;
using Eaf.WebHooks;
using NSubstitute;
using Shouldly;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.WebHooks
{
    public class EafWebhookReceiverBddTests
    {
        private static readonly PropertyInfo InstanceProperty = typeof(IocManager)
            .GetProperty(nameof(IocManager.Instance), BindingFlags.Public | BindingFlags.Static)!;

        private static void SetIocManagerInstance(IocManager instance)
        {
            InstanceProperty.SetValue(null, instance, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, null);
        }

        private sealed class TestWebhookReceiver : EafWebHookReceiver
        {
            public string? PublicL(string name) => L(name);
            public string? PublicL(string name, params object[] args) => L(name, args);
            public string? PublicL(string name, System.Globalization.CultureInfo culture) => L(name, culture);
            public string? PublicL(string name, System.Globalization.CultureInfo culture, params object[] args) => L(name, culture, args);
            public ILocalizationManager? ExposedLocalizationManager => LocalizationManager;

            public new IUnitOfWorkManager? UnitOfWorkManagerProperty
            {
                get => base.UnitOfWorkManager;
                set => base.UnitOfWorkManager = value!;
            }

            public new ILocalizationSource? LocalizationSourceProperty
            {
                get => base.LocalizationSource;
            }

            public string? LocalizationSourceNameOverride
            {
                get => base.LocalizationSourceName;
                set => base.LocalizationSourceName = value;
            }

            public new IActiveUnitOfWork? CurrentUnitOfWorkProperty => base.CurrentUnitOfWork;

            public override Task ProcessRequest(string requestBody)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarAbstrata_Entao_DeveSerAbstrata()
        {
            typeof(EafWebHookReceiver).IsAbstract.ShouldBeTrue();
        }

        [Fact]
        public void Dado_IocManagerComDependencias_Quando_CriarReceiver_Entao_DeveResolverDependencias()
        {
            var original = IocManager.Instance;
            var iocManager = new IocManager();
            SetIocManagerInstance(iocManager);

            try
            {
                iocManager.IocContainer.Register(
                    Component.For<ILoggerFactory>().Instance(Substitute.For<ILoggerFactory>()).LifestyleSingleton(),
                    Component.For<IEventBus>().Instance(Substitute.For<IEventBus>()).LifestyleSingleton(),
                    Component.For<ILocalizationManager>().Instance(Substitute.For<ILocalizationManager>()).LifestyleSingleton(),
                    Component.For<IObjectMapper>().Instance(Substitute.For<IObjectMapper>()).LifestyleSingleton()
                );

                var receiver = new TestWebhookReceiver();

                receiver.IocManager.ShouldBeSameAs(iocManager);
                receiver.EventBus.ShouldNotBeNull();
                receiver.ExposedLocalizationManager.ShouldNotBeNull();
                receiver.ObjectMapper.ShouldNotBeNull();
                receiver.Logger.ShouldNotBeNull();
            }
            finally
            {
                SetIocManagerInstance(original);
                iocManager.Dispose();
            }
        }

        [Fact]
        public void Dado_UnitOfWorkManagerNaoDefinido_Quando_Acessar_Entao_DeveLancarExcecao()
        {
            var receiver = new TestWebhookReceiver();

            var exception = Should.Throw<AbpException>(() => { _ = receiver.UnitOfWorkManagerProperty; });
            exception.Message.ShouldContain("UnitOfWorkManager");
        }

        [Fact]
        public void Dado_UnitOfWorkManagerDefinido_Quando_Acessar_Entao_DeveRetornarValor()
        {
            var receiver = new TestWebhookReceiver();
            var uowManager = Substitute.For<IUnitOfWorkManager>();

            receiver.UnitOfWorkManagerProperty = uowManager;

            receiver.UnitOfWorkManagerProperty.ShouldBeSameAs(uowManager);
        }

        [Fact]
        public void Dado_Receiver_Quando_UsarLocalizacao_Entao_DeveRetornarChaveComoFallback()
        {
            var receiver = new TestWebhookReceiver();

            receiver.PublicL("TestKey").ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_LocalizationSourceNaoDefinido_Quando_Acessar_Entao_DeveLancarExcecao()
        {
            var receiver = new TestWebhookReceiver();
            receiver.LocalizationSourceNameOverride = null;

            var exception = Should.Throw<AbpException>(() => { _ = receiver.LocalizationSourceProperty; });
            exception.Message.ShouldContain("LocalizationSourceName");
        }

        [Fact]
        public void Dado_LocalizationManagerComSource_Quando_AcessarLocalizationSource_Entao_DeveRetornarSource()
        {
            // Dado
            var receiver = new TestWebhookReceiver();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var localizationSource = Substitute.For<ILocalizationSource>();
            localizationSource.Name.Returns(MiddlewareLocalizationHelper.DefaultSourceName);
            localizationManager.GetSource(MiddlewareLocalizationHelper.DefaultSourceName).Returns(localizationSource);

            receiver.LocalizationManager = localizationManager;

            // Quando
            var source = receiver.LocalizationSourceProperty;

            // Então
            source.ShouldNotBeNull();
            source.Name.ShouldBe(MiddlewareLocalizationHelper.DefaultSourceName);
        }

        [Fact]
        public async Task Dado_Receiver_Quando_ProcessarRequest_Entao_DeveCompletarSemErros()
        {
            var receiver = new TestWebhookReceiver();

            await Should.NotThrowAsync(async () => await receiver.ProcessRequest("{}"));
        }

        [Fact]
        public void Dado_Receiver_Quando_UsarLocalizacaoComCultura_Entao_DeveRetornarChaveComoFallback()
        {
            var receiver = new TestWebhookReceiver();
            receiver.PublicL("TestKey", System.Globalization.CultureInfo.InvariantCulture).ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_Receiver_Quando_UsarLocalizacaoComCulturaEArgs_Entao_DeveRetornarChaveComoFallback()
        {
            var receiver = new TestWebhookReceiver();
            receiver.PublicL("TestKey", System.Globalization.CultureInfo.InvariantCulture, "arg1").ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_Receiver_Quando_UsarLocalizacaoComArgs_Entao_DeveRetornarChaveComoFallback()
        {
            var receiver = new TestWebhookReceiver();
            receiver.PublicL("TestKey", "arg1").ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_Receiver_Quando_ConfigurarPropriedades_Entao_DeveRetornarValores()
        {
            var receiver = new TestWebhookReceiver
            {
                ReceiverName = "TestReceiver",
                context = null
            };

            receiver.ReceiverName.ShouldBe("TestReceiver");
            receiver.context.ShouldBeNull();
        }

        [Fact]
        public void Dado_UnitOfWorkManagerDefinido_Quando_AcessarCurrentUnitOfWork_Entao_DeveRetornarValor()
        {
            var receiver = new TestWebhookReceiver();
            var uowManager = Substitute.For<IUnitOfWorkManager>();
            var uow = Substitute.For<IActiveUnitOfWork>();
            uowManager.Current.Returns(uow);

            receiver.UnitOfWorkManagerProperty = uowManager;

            receiver.CurrentUnitOfWorkProperty.ShouldBeSameAs(uow);
        }
    }
}
