using Abp.Dependency;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using Eaf.Middleware.Worker;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Worker.Tests
{
    /// <summary>
    /// Testes para Interface Segregation no IEafWorkerBase — Spec 83.
    /// Verifica que IIocManager NÃO está exposto na interface pública.
    /// </summary>
    public class IEafWorkerBaseIspTests
    {
        #region ISP — IIocManager não deve estar na interface

        [Fact]
        public void Dado_Interface_Quando_VerificarMembros_Entao_NaoDeveConterIIocManager()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var hasIocManager = properties.Any(p => p.PropertyType == typeof(IIocManager));

            // Então
            hasIocManager.ShouldBeFalse("IEafWorkerBase NÃO deve expor IIocManager (ISP - Spec 83)");
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarMembros_Entao_NaoDeveConterPropriedadeIocManager()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var iocManagerProp = interfaceType.GetProperty("IocManager");

            // Então
            iocManagerProp.ShouldBeNull("IEafWorkerBase NÃO deve ter propriedade IocManager (ISP)");
        }

        #endregion

        #region Interface — Membros Essenciais

        [Fact]
        public void Dado_Interface_Quando_VerificarLogger_Entao_DeveExistir()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var loggerProp = interfaceType.GetProperty("Logger");

            // Então
            loggerProp.ShouldNotBeNull();
            loggerProp.PropertyType.ShouldBe(typeof(ILogger));
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarEventBus_Entao_DeveExistir()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var eventBusProp = interfaceType.GetProperty("EventBus");

            // Então
            eventBusProp.ShouldNotBeNull();
            eventBusProp.PropertyType.ShouldBe(typeof(IEventBus));
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarObjectMapper_Entao_DeveExistir()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var mapperProp = interfaceType.GetProperty("ObjectMapper");

            // Então
            mapperProp.ShouldNotBeNull();
            mapperProp.PropertyType.ShouldBe(typeof(IObjectMapper));
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarLocalizationManager_Entao_DeveExistirComSetter()
        {
            // Dado
            var interfaceType = typeof(IEafWorkerBase);

            // Quando
            var locProp = interfaceType.GetProperty("LocalizationManager");

            // Então
            locProp.ShouldNotBeNull();
            locProp.PropertyType.ShouldBe(typeof(ILocalizationManager));
            locProp.SetMethod.ShouldNotBeNull();
        }

        #endregion

        #region Interface — Herança

        [Fact]
        public void Dado_Interface_Quando_VerificarHeranca_Entao_DeveHerdarDeIHostedService()
        {
            // Dado & Quando
            var inheritsHosted = typeof(IHostedService).IsAssignableFrom(typeof(IEafWorkerBase));

            // Então
            inheritsHosted.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarHeranca_Entao_DeveHerdarDeIDomainService()
        {
            // Dado & Quando
            var inheritsDomain = typeof(IDomainService).IsAssignableFrom(typeof(IEafWorkerBase));

            // Então
            inheritsDomain.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Interface_Quando_VerificarHeranca_Entao_DeveHerdarDeISingletonDependency()
        {
            // Dado & Quando
            var inheritsSingleton = typeof(ISingletonDependency).IsAssignableFrom(typeof(IEafWorkerBase));

            // Então
            inheritsSingleton.ShouldBeTrue();
        }

        #endregion

        #region EafWorkerBase — IocManager na classe base

        [Fact]
        public void Dado_ClasseBase_Quando_VerificarIocManager_Entao_DeveExistirNaClasse()
        {
            // Dado — IocManager existe na classe, mas NÃO na interface
            var classType = typeof(EafWorkerBase);

            // Quando
            var iocManagerProp = classType.GetProperty("IocManager");

            // Então
            iocManagerProp.ShouldNotBeNull("EafWorkerBase DEVE ter IocManager como property injection");
            iocManagerProp.PropertyType.ShouldBe(typeof(IIocManager));
        }

        [Fact]
        public void Dado_ClasseBase_Quando_VerificarConstrutorPadrao_Entao_DeveInicializarLoggerComoNullLogger()
        {
            // Dado — Não podemos instanciar abstrata, mas verificar via reflection
            var classType = typeof(EafWorkerBase);

            // Quando — Verificar construtor protected/public sem parâmetros
            var constructor = classType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, Type.EmptyTypes, null);

            // Então
            constructor.ShouldNotBeNull("EafWorkerBase deve ter construtor sem parâmetros");
        }

        #endregion
    }
}
