using Eaf.Castle.Logging.SerilogIntegration;
using Serilog;
using Shouldly;
using System;
using System.Globalization;
using Xunit;

namespace Eaf.Castle.Serilog.Tests.Castle.Logging.SerilogIntegration
{
    /// <summary>
    /// Testes para os métodos Format do SerilogLogger (Error, Info, Warn, Fatal, Trace).
    /// Complementa os testes existentes que só cobriam DebugFormat.
    /// </summary>
    public class SerilogLoggerFormatTests
    {
        private readonly SerilogLogger _logger;

        public SerilogLoggerFormatTests()
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var factory = new SerilogLoggerFactory(serilogLogger);
            _logger = new SerilogLogger(serilogLogger, factory);
        }

        #region ErrorFormat

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarErrorFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.ErrorFormat("Error: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarErrorFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.ErrorFormat(exception, "Error: {0}", "test"));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarErrorFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.ErrorFormat(CultureInfo.InvariantCulture, "Error: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarErrorFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.ErrorFormat(exception, CultureInfo.InvariantCulture, "Error: {0}", "test"));
        }

        #endregion

        #region InfoFormat

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarInfoFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.InfoFormat("Info: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarInfoFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.InfoFormat(exception, "Info: {0}", "test"));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarInfoFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.InfoFormat(CultureInfo.InvariantCulture, "Info: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarInfoFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.InfoFormat(exception, CultureInfo.InvariantCulture, "Info: {0}", "test"));
        }

        #endregion

        #region WarnFormat

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarWarnFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.WarnFormat("Warn: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarWarnFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.WarnFormat(exception, "Warn: {0}", "test"));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarWarnFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.WarnFormat(CultureInfo.InvariantCulture, "Warn: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarWarnFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.WarnFormat(exception, CultureInfo.InvariantCulture, "Warn: {0}", "test"));
        }

        #endregion

        #region FatalFormat

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarFatalFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.FatalFormat("Fatal: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarFatalFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.FatalFormat(exception, "Fatal: {0}", "test"));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarFatalFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.FatalFormat(CultureInfo.InvariantCulture, "Fatal: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarFatalFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.FatalFormat(exception, CultureInfo.InvariantCulture, "Fatal: {0}", "test"));
        }

        #endregion

        #region TraceFormat

        [Fact]
        public void Dado_FormatoComParametros_Quando_LogarTraceFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.TraceFormat("Trace: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoEFormato_Quando_LogarTraceFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.TraceFormat(exception, "Trace: {0}", "test"));
        }

        [Fact]
        public void Dado_FormatProviderEFormato_Quando_LogarTraceFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.TraceFormat(CultureInfo.InvariantCulture, "Trace: {0}", "test"));
        }

        [Fact]
        public void Dado_ExcecaoFormatProviderEFormato_Quando_LogarTraceFormat_Entao_DeveExecutarSemErro()
        {
            var exception = new Exception("Test");
            Should.NotThrow(() => _logger.TraceFormat(exception, CultureInfo.InvariantCulture, "Trace: {0}", "test"));
        }

        #endregion

        #region MessageFactory overloads

        [Fact]
        public void Dado_MessageFactory_Quando_LogarInfo_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.Info(() => "Info from factory"));
        }

        [Fact]
        public void Dado_MessageFactory_Quando_LogarWarn_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.Warn(() => "Warn from factory"));
        }

        [Fact]
        public void Dado_MessageFactory_Quando_LogarError_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.Error(() => "Error from factory"));
        }

        [Fact]
        public void Dado_MessageFactory_Quando_LogarFatal_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.Fatal(() => "Fatal from factory"));
        }

        [Fact]
        public void Dado_MessageFactory_Quando_LogarTrace_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.Trace(() => "Trace from factory"));
        }

        #endregion

        #region Disabled Logger Tests

        [Fact]
        public void Dado_LoggerComNivelMinimoPorcimaDoDebug_Quando_LogarDebug_Entao_NaoDeveLogar()
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .CreateLogger();

            var factory = new SerilogLoggerFactory(serilogLogger);
            var logger = new SerilogLogger(serilogLogger, factory);

            logger.IsDebugEnabled.ShouldBeFalse();
            logger.IsInfoEnabled.ShouldBeFalse();
            logger.IsWarnEnabled.ShouldBeFalse();
            logger.IsErrorEnabled.ShouldBeFalse();
            logger.IsTraceEnabled.ShouldBeFalse();
            logger.IsFatalEnabled.ShouldBeTrue();

            Should.NotThrow(() => logger.Debug("should not log"));
            Should.NotThrow(() => logger.Debug(() => "should not log"));
            Should.NotThrow(() => logger.DebugFormat("should not log: {0}", "arg"));
            Should.NotThrow(() => logger.Info("should not log"));
            Should.NotThrow(() => logger.Info(() => "should not log"));
            Should.NotThrow(() => logger.InfoFormat("should not log: {0}", "arg"));
            Should.NotThrow(() => logger.Warn("should not log"));
            Should.NotThrow(() => logger.Warn(() => "should not log"));
            Should.NotThrow(() => logger.WarnFormat("should not log: {0}", "arg"));
            Should.NotThrow(() => logger.Error("should not log"));
            Should.NotThrow(() => logger.Error(() => "should not log"));
            Should.NotThrow(() => logger.ErrorFormat("should not log: {0}", "arg"));
            Should.NotThrow(() => logger.Trace("should not log"));
            Should.NotThrow(() => logger.Trace(() => "should not log"));
            Should.NotThrow(() => logger.TraceFormat("should not log: {0}", "arg"));
        }

        #endregion

        #region Multiple Args

        [Fact]
        public void Dado_MultiplosParametros_Quando_LogarInfoFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.InfoFormat("User {0} logged in from {1}", "admin", "192.168.1.1"));
        }

        [Fact]
        public void Dado_MultiplosParametros_Quando_LogarErrorFormat_Entao_DeveExecutarSemErro()
        {
            Should.NotThrow(() => _logger.ErrorFormat("Error {0} at {1}: {2}", 500, "API", "timeout"));
        }

        #endregion
    }
}
