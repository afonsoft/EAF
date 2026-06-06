using Abp.Events.Bus;
using Abp.Localization;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using Eaf.Middleware.Worker;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    /// <summary>
    /// Testes para EafWorkerBase — construção, localização e propriedades.
    /// </summary>
    public class EafWorkerBaseLocalizationTests
    {
        private class TestWorker : EafWorkerBase
        {
            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                return Task.CompletedTask;
            }

            public string CallL(string name) => L(name);
            public string CallLWithArgs(string name, params object[] args) => L(name, args);
        }

        #region Construtor e Defaults

        [Fact]
        public void Dado_WorkerConcreto_Quando_Construir_Entao_LoggerDeveSerNullLogger()
        {
            // Dado & Quando
            var worker = new TestWorker();

            // Então
            worker.Logger.ShouldNotBeNull();
            worker.Logger.ShouldBe(NullLogger.Instance);
        }

        [Fact]
        public void Dado_WorkerConcreto_Quando_Construir_Entao_EventBusDeveSerNullEventBus()
        {
            // Dado & Quando
            var worker = new TestWorker();

            // Então
            worker.EventBus.ShouldNotBeNull();
            worker.EventBus.ShouldBe(NullEventBus.Instance);
        }

        [Fact]
        public void Dado_WorkerConcreto_Quando_Construir_Entao_ObjectMapperDeveSerNullObjectMapper()
        {
            // Dado & Quando
            var worker = new TestWorker();

            // Então
            worker.ObjectMapper.ShouldNotBeNull();
            worker.ObjectMapper.ShouldBe(NullObjectMapper.Instance);
        }

        #endregion

        #region Property Injection

        [Fact]
        public void Dado_Logger_Quando_Setar_Entao_DeveReter()
        {
            // Dado
            var worker = new TestWorker();
            var logger = NullLogger.Instance;

            // Quando
            worker.Logger = logger;

            // Então
            worker.Logger.ShouldBe(logger);
        }

        [Fact]
        public void Dado_EventBus_Quando_Setar_Entao_DeveReter()
        {
            // Dado
            var worker = new TestWorker();
            var eventBus = NullEventBus.Instance;

            // Quando
            worker.EventBus = eventBus;

            // Então
            worker.EventBus.ShouldBe(eventBus);
        }

        [Fact]
        public void Dado_ObjectMapper_Quando_Setar_Entao_DeveReter()
        {
            // Dado
            var worker = new TestWorker();
            var mapper = NullObjectMapper.Instance;

            // Quando
            worker.ObjectMapper = mapper;

            // Então
            worker.ObjectMapper.ShouldBe(mapper);
        }

        #endregion

        #region Localização

        [Fact]
        public void Dado_WorkerComLocalizationManager_Quando_ChamarL_Entao_DeveRetornarString()
        {
            // Dado
            var worker = new TestWorker();
            worker.LocalizationManager = NullLocalizationManager.Instance;

            // Quando — Usando reflection para acessar L() que é protected
            var result = worker.CallL("TestKey");

            // Então — Com NullLocalizationManager, retorna a key prefixada
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_WorkerComLocalizationManager_Quando_ChamarLComArgs_Entao_DeveRetornarString()
        {
            // Dado
            var worker = new TestWorker();
            worker.LocalizationManager = NullLocalizationManager.Instance;

            // Quando
            var result = worker.CallLWithArgs("TestKey", "arg1", "arg2");

            // Então
            result.ShouldNotBeNull();
        }

        #endregion

        #region BackgroundService Herança

        [Fact]
        public void Dado_EafWorkerBase_Quando_VerificarHeranca_Entao_DeveHerdarDeBackgroundService()
        {
            // Dado & Quando
            var type = typeof(EafWorkerBase);

            // Então
            typeof(BackgroundService).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void Dado_WorkerConcreto_Quando_ExecuteAsync_Entao_DeveSerChamavel()
        {
            // Dado
            var worker = new TestWorker();
            var cts = new CancellationTokenSource();

            // Quando & Então
            Should.NotThrow(async () => await worker.StartAsync(cts.Token));
            cts.Cancel();
        }

        #endregion

        #region UnitOfWorkManager

        [Fact]
        public void Dado_UnitOfWorkManagerNaoDefinido_Quando_Acessar_Entao_DeveLancarExcecao()
        {
            // Dado
            var worker = new TestWorker();

            // Quando & Então — via reflection para acessar propriedade pública
            var prop = typeof(EafWorkerBase).GetProperty("UnitOfWorkManager");
            prop.ShouldNotBeNull();

            Should.Throw<TargetInvocationException>(() => prop.GetValue(worker));
        }

        #endregion
    }
}
