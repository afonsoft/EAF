using Abp;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using NSubstitute;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Worker
{
    public class EafWorkerBaseBddTests
    {
        private class TestWorker : EafWorkerBase
        {
            protected override System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public string PublicL(string name) => L(name);
            public string PublicL(string name, params object[] args) => L(name, args);
            public string PublicL(string name, System.Globalization.CultureInfo culture) => L(name, culture);
            public string PublicL(string name, System.Globalization.CultureInfo culture, params object[] args) => L(name, culture, args);
            public IUnitOfWorkManager PublicUnitOfWorkManager => UnitOfWorkManager;
            public ILocalizationSource PublicLocalizationSource => LocalizationSource;
            public IActiveUnitOfWork PublicCurrentUnitOfWork => CurrentUnitOfWork;
            public new string LocalizationSourceName { get => base.LocalizationSourceName; set => base.LocalizationSourceName = value; }
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveTerValoresPadrao()
        {
            var worker = new TestWorker();

            worker.Logger.ShouldNotBeNull();
            worker.EventBus.ShouldNotBeNull();
            worker.ObjectMapper.ShouldNotBeNull();
            worker.ObjectMapper.ShouldBeOfType<NullObjectMapper>();
        }

        [Fact]
        public void Dado_Worker_Quando_AcessarUnitOfWorkManagerSemSetar_Entao_DeveLancarAbpException()
        {
            var worker = new TestWorker();

            Should.Throw<AbpException>(() => worker.PublicUnitOfWorkManager)
                .Message.ShouldContain("Must set UnitOfWorkManager");
        }

        [Fact]
        public void Dado_UnitOfWorkManager_Quando_Setar_Entao_DeveRetornarMesmaInstancia()
        {
            var worker = new TestWorker();
            var uowManager = Substitute.For<IUnitOfWorkManager>();

            worker.UnitOfWorkManager = uowManager;

            worker.PublicUnitOfWorkManager.ShouldBeSameAs(uowManager);
        }

        [Fact]
        public void Dado_LocalizationManagerNulo_Quando_L_Entao_DeveRetornarChave()
        {
            var worker = new TestWorker();

            var result = worker.PublicL("TestKey");

            result.ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_LocalizationSourceComValor_Quando_L_Entao_DeveRetornarValorLocalizado()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafCore");
            source.GetStringOrNull("TestKey", Arg.Any<System.Globalization.CultureInfo>()).Returns("LocalizedValue");
            localizationManager.GetSource("EafCore").Returns(source);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("TestKey");

            result.ShouldBe("LocalizedValue");
        }

        [Fact]
        public void Dado_LocalizationSourceComFormato_Quando_LComArgs_Entao_DeveFormatarValor()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafCore");
            source.GetStringOrNull("Hello", Arg.Any<System.Globalization.CultureInfo>()).Returns("Hello {0}");
            localizationManager.GetSource("EafCore").Returns(source);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("Hello", "World");

            result.ShouldBe("Hello World");
        }

        [Fact]
        public void Dado_SourceNaoEncontrado_Quando_L_Entao_DeveRetornarChave()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            localizationManager.GetSource(Arg.Any<string>()).Returns(x => { throw new Exception("Not found"); });
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("MissingKey");

            result.ShouldBe("MissingKey");
        }

        [Fact]
        public void Dado_PrimeiroSourceSemValor_Quando_L_Entao_DeveUsarFallback()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var eafSource = Substitute.For<ILocalizationSource>();
            eafSource.Name.Returns("EafCore");
            eafSource.GetStringOrNull("FallbackKey", Arg.Any<System.Globalization.CultureInfo>()).Returns((string)null);
            var abpSource = Substitute.For<ILocalizationSource>();
            abpSource.Name.Returns("Abp");
            abpSource.GetStringOrNull("FallbackKey", Arg.Any<System.Globalization.CultureInfo>()).Returns("FallbackValue");
            localizationManager.GetSource("EafCore").Returns(eafSource);
            localizationManager.GetSource("Abp").Returns(abpSource);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("FallbackKey");

            result.ShouldBe("FallbackValue");
        }

        [Fact]
        public void Dado_LocalizationSourceName_Quando_AcessarLocalizationSource_Entao_DeveRetornarSource()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafCore");
            localizationManager.GetSource("EafCore").Returns(source);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicLocalizationSource;

            result.ShouldBeSameAs(source);
        }

        [Fact]
        public void Dado_ChaveVazia_Quando_L_Entao_DeveRetornarChave()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL(string.Empty);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_LocalizationSourceNameNulo_Quando_AcessarLocalizationSource_Entao_DeveLancarAbpException()
        {
            var worker = new TestWorker();
            worker.LocalizationSourceName = null;

            Should.Throw<AbpException>(() => worker.PublicLocalizationSource)
                .Message.ShouldContain("Must set LocalizationSourceName");
        }

        [Fact]
        public void Dado_LocalizationSourceNameMudado_Quando_AcessarLocalizationSource_Entao_DeveAtualizarSource()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var eafSource = Substitute.For<ILocalizationSource>();
            eafSource.Name.Returns("EafCore");
            var abpSource = Substitute.For<ILocalizationSource>();
            abpSource.Name.Returns("Abp");
            localizationManager.GetSource("EafCore").Returns(eafSource);
            localizationManager.GetSource("Abp").Returns(abpSource);
            worker.LocalizationManager = localizationManager;

            _ = worker.PublicLocalizationSource;
            worker.LocalizationSourceName = "Abp";

            worker.PublicLocalizationSource.ShouldBeSameAs(abpSource);
        }

        [Fact]
        public void Dado_Worker_Quando_LComCultura_Entao_DeveLocalizar()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafCore");
            source.GetStringOrNull("Key", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")).Returns("Valor");
            localizationManager.GetSource("EafCore").Returns(source);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("Key", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

            result.ShouldBe("Valor");
        }

        [Fact]
        public void Dado_Worker_Quando_LComCulturaEArgs_Entao_DeveFormatar()
        {
            var worker = new TestWorker();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafCore");
            source.GetStringOrNull("Hello", System.Globalization.CultureInfo.GetCultureInfo("en-US")).Returns("Hello {0}");
            localizationManager.GetSource("EafCore").Returns(source);
            worker.LocalizationManager = localizationManager;

            var result = worker.PublicL("Hello", System.Globalization.CultureInfo.GetCultureInfo("en-US"), "World");

            result.ShouldBe("Hello World");
        }

        [Fact]
        public void Dado_Worker_Quando_AcessarCurrentUnitOfWork_Entao_DeveRetornarCurrent()
        {
            var worker = new TestWorker();
            var uowManager = Substitute.For<IUnitOfWorkManager>();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            uowManager.Current.Returns(activeUow);
            worker.UnitOfWorkManager = uowManager;

            worker.PublicCurrentUnitOfWork.ShouldBeSameAs(activeUow);
        }

        [Fact]
        public void Dado_EafWorkerBase_Quando_Instanciar_Entao_DeveImplementarIEafWorkerBase()
        {
            var worker = new TestWorker();

            worker.ShouldBeAssignableTo<IEafWorkerBase>();
        }
    }
}
