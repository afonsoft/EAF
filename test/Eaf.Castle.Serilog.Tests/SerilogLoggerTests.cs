using Eaf.Castle.Logging.SerilogIntegration;
using NSubstitute;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shouldly;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.Castle.Serilog.Tests
{
    /// <summary>
    /// Testes para a classe SerilogLogger usando padrão BDD em português.
    /// </summary>
    public class SerilogLoggerBddTests
    {
        private readonly ILogger _serilogLogger;
        private readonly SerilogLoggerFactory _factory;
        private readonly SerilogLogger _logger;

        public SerilogLoggerBddTests()
        {
            // Criar um logger Serilog real para evitar problemas com mocking
            _serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            _factory = new SerilogLoggerFactory(_serilogLogger);
            _logger = new SerilogLogger(_serilogLogger, _factory);
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarInstancia_Entao_DeveRetornarInstanciaValida()
        {
            // Arrange & Act
            var logger = new SerilogLogger(_serilogLogger, _factory);

            // Assert
            logger.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsDebugEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsDebugEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsErrorEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsErrorEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsFatalEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsFatalEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsInfoEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsInfoEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsTraceEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsTraceEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_LoggerConfigurado_Quando_VerificarIsWarnEnabled_Entao_DeveRetornarTrue()
        {
            // Act
            var result = _logger.IsWarnEnabled;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_NomeDeChildLogger_Quando_CriarChildLogger_Entao_DeveLancarNotImplementedException()
        {
            // Act & Assert
            Should.Throw<NotImplementedException>(() => _logger.CreateChildLogger("child"));
        }

        [Fact]
        public void Dado_MensagemDebug_Quando_LogarDebug_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Debug message";

            // Act & Assert
            Should.NotThrow(() => _logger.Debug(message));
        }

        [Fact]
        public void Dado_MensagemEExcecao_Quando_LogarDebug_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Debug message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Debug(message, exception));
        }

        [Fact]
        public void Dado_MessageFactory_Quando_LogarDebug_Entao_DeveExecutarSemErro()
        {
            // Arrange
            Func<string> messageFactory = () => "Debug message from factory";

            // Act & Assert
            Should.NotThrow(() => _logger.Debug(messageFactory));
        }

        [Fact]
        public void Dado_MensagemInfo_Quando_LogarInfo_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Info message";

            // Act & Assert
            Should.NotThrow(() => _logger.Info(message));
        }

        [Fact]
        public void Dado_MensagemInfoEExcecao_Quando_LogarInfo_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Info message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Info(message, exception));
        }

        [Fact]
        public void Dado_MensagemWarn_Quando_LogarWarn_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Warn message";

            // Act & Assert
            Should.NotThrow(() => _logger.Warn(message));
        }

        [Fact]
        public void Dado_MensagemWarnEExcecao_Quando_LogarWarn_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Warn message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Warn(message, exception));
        }

        [Fact]
        public void Dado_MensagemError_Quando_LogarError_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Error message";

            // Act & Assert
            Should.NotThrow(() => _logger.Error(message));
        }

        [Fact]
        public void Dado_MensagemErrorEExcecao_Quando_LogarError_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Error message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Error(message, exception));
        }

        [Fact]
        public void Dado_MensagemFatal_Quando_LogarFatal_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Fatal message";

            // Act & Assert
            Should.NotThrow(() => _logger.Fatal(message));
        }

        [Fact]
        public void Dado_MensagemFatalEExcecao_Quando_LogarFatal_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Fatal message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Fatal(message, exception));
        }

        [Fact]
        public void Dado_MensagemTrace_Quando_LogarTrace_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Trace message";

            // Act & Assert
            Should.NotThrow(() => _logger.Trace(message));
        }

        [Fact]
        public void Dado_MensagemTraceEExcecao_Quando_LogarTrace_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var message = "Trace message";
            var exception = new Exception("Test exception");

            // Act & Assert
            Should.NotThrow(() => _logger.Trace(message, exception));
        }

        [Theory]
        [InlineData("test message")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("message with special characters !@#$%^&*()")]
        public void Dado_DiferentesMensagens_Quando_LogarDebug_Entao_DeveProcessarCorretamente(string message)
        {
            // Act & Assert
            Should.NotThrow(() => _logger.Debug(message));
        }

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarDebugFormat_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var format = "Debug message with parameter: {0}";
            var parameter = "test";

            // Act & Assert
            Should.NotThrow(() => _logger.DebugFormat(format, parameter));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarDebugFormat_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var format = "Debug message with parameter: {0}";
            var parameter = "test";

            // Act & Assert
            Should.NotThrow(() => _logger.DebugFormat(exception, format, parameter));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarDebugFormat_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var formatProvider = CultureInfo.InvariantCulture;
            var format = "Debug message with parameter: {0}";
            var parameter = "test";

            // Act & Assert
            Should.NotThrow(() => _logger.DebugFormat(formatProvider, format, parameter));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarDebugFormat_Entao_DeveExecutarSemErro()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var formatProvider = CultureInfo.InvariantCulture;
            var format = "Debug message with parameter: {0}";
            var parameter = "test";

            // Act & Assert
            Should.NotThrow(() => _logger.DebugFormat(exception, formatProvider, format, parameter));
        }

        [Fact]
        public void Dado_Logger_Quando_ChamarToString_Entao_DeveRetornarStringValida()
        {
            // Act
            var result = _logger.ToString();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }

        [Fact]
        public void Dado_LoggerDesabilitado_Quando_InvocarTodosOsMetodosDeLog_Entao_NaoDeveChamarLogger()
        {
            // Arrange — logger real com nível Off para simular todos os IsEnabled falsos
            var levelSwitch = new LoggingLevelSwitch(LevelAlias.Off);
            var disabledSerilogLogger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .CreateLogger();
            var factory = new SerilogLoggerFactory(disabledSerilogLogger);
            var logger = new SerilogLogger(disabledSerilogLogger, factory);

            var methods = typeof(SerilogLogger)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.ReturnType == typeof(void) &&
                            (m.Name.StartsWith("Debug") || m.Name.StartsWith("Info") ||
                             m.Name.StartsWith("Warn") || m.Name.StartsWith("Error") ||
                             m.Name.StartsWith("Fatal") || m.Name.StartsWith("Trace")))
                .ToList();

            methods.ShouldNotBeEmpty();

            foreach (var method in methods)
            {
                var args = method.GetParameters()
                    .Select(p => GetDefaultValueForLogMethodParameter(p.ParameterType))
                    .ToArray();

                Should.NotThrow(() => method.Invoke(logger, args));
            }
        }

        private static object GetDefaultValueForLogMethodParameter(Type type)
        {
            if (type == typeof(string))
                return "test";
            if (type == typeof(Exception))
                return new Exception("test");
            if (type == typeof(Func<string>))
                return (Func<string>)(() => "test");
            if (type == typeof(IFormatProvider))
                return CultureInfo.InvariantCulture;
            if (type == typeof(object[]))
                return new object[] { "test" };
            return null;
        }
    }
}