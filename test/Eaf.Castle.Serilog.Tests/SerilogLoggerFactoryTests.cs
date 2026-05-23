using Castle.Core.Logging;
using Eaf.Castle.Logging.SerilogIntegration;
using Serilog;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Castle.Serilog.Tests
{
    public class SerilogLoggerFactoryTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarFactory_Entao_DeveCriarComSucesso()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory();
            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_LoggerCustomizado_Quando_CriarFactory_Entao_DeveCriarComSucesso()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory(logger);
            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Factory_Quando_CreateComNomeValido_Entao_DeveRetornarLogger()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory(logger);

            var result = factory.Create("TestLogger");
            result.ShouldNotBeNull();
            result.ShouldBeOfType<SerilogLogger>();
        }

        [Fact]
        public void Dado_Factory_Quando_CreateComNomeNull_Entao_DeveLancarArgumentNullException()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory(logger);

            Should.Throw<ArgumentNullException>(() => factory.Create((string)null));
        }

        [Fact]
        public void Dado_Factory_Quando_CreateComNomeELevel_Entao_DeveLancarNotSupportedException()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory(logger);

            Should.Throw<NotSupportedException>(() => factory.Create("test", LoggerLevel.Debug));
        }

        [Fact]
        public void Dado_Factory_Quando_CriarMultiplosLoggers_Entao_DevemSerIndependentes()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            var factory = new SerilogLoggerFactory(logger);

            var logger1 = factory.Create("Logger1");
            var logger2 = factory.Create("Logger2");

            logger1.ShouldNotBeSameAs(logger2);
        }
    }
}
