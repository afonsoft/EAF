using Serilog;
using Serilog.Events;
using System;
using CastleILogger = Castle.Core.Logging.ILogger;

namespace Eaf.Castle.Logging.SerilogIntegration
{
    /// <summary>
    /// Implementação de logger que integra Serilog com a interface ILogger do Castle Windsor.
    /// Fornece funcionalidades completas de logging com suporte a diferentes níveis e formatação.
    /// </summary>
    [Serializable]
    public class SerilogLogger :
        MarshalByRefObject,
        CastleILogger
    {
        /// <summary>
        /// Inicializa uma nova instância da classe SerilogLogger.
        /// </summary>
        /// <param name="logger">Instância do logger Serilog</param>
        /// <param name="factory">Fábrica de loggers responsável pela criação</param>
        public SerilogLogger(ILogger logger, SerilogLoggerFactory factory)
        {
            Logger = logger;
            Factory = factory;
        }

        /// <summary>
        /// Construtor interno para serialização.
        /// </summary>
        internal SerilogLogger()
        {
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Debug está habilitado.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Debug); }
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Error está habilitado.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Error); }
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Fatal está habilitado.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Fatal); }
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Info está habilitado.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Information); }
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Trace está habilitado.
        /// </summary>
        public bool IsTraceEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Verbose); }
        }

        /// <summary>
        /// Obtém um valor indicando se o nível de log Warning está habilitado.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return Logger.IsEnabled(LogEventLevel.Warning); }
        }

        /// <summary>
        /// Obtém ou define a fábrica de loggers responsável pela criação desta instância.
        /// </summary>
        protected internal SerilogLoggerFactory Factory { get; set; }

        /// <summary>
        /// Obtém ou define a instância do logger Serilog subjacente.
        /// </summary>
        protected internal ILogger Logger { get; set; }

        /// <summary>
        /// Cria um logger filho com o nome especificado.
        /// Nota: Criação de loggers filhos não é suportada no Serilog.
        /// </summary>
        /// <param name="loggerName">Nome do logger filho</param>
        /// <returns>Não retorna - sempre lança exceção</returns>
        /// <exception cref="NotImplementedException">Sempre lançada pois loggers filhos não são suportados</exception>
        public CastleILogger CreateChildLogger(string loggerName)
        {
            // Serilog calls these sub loggers. We might be able to do something here but for now
            // I'm going leave it like this.
            throw new NotImplementedException("Creating child loggers for Serilog is not supported");
        }

        /// <summary>
        /// Registra uma mensagem de debug com exceção associada.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="exception">Exceção associada à mensagem</param>
        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, message);
            }
        }

        /// <summary>
        /// Registra uma mensagem de debug usando uma factory de mensagem.
        /// </summary>
        /// <param name="messageFactory">Factory que produz a mensagem</param>
        public void Debug(Func<string> messageFactory)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Registra uma mensagem de debug simples.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        /// <summary>
        /// DebugFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// DebugFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// DebugFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, format, args);
            }
        }

        /// <summary>
        /// DebugFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void DebugFormat(string format, params object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(format, args);
            }
        }

        /// <summary>
        /// Registra uma mensagem de erro com exceção associada.
        /// </summary>
        /// <param name="message">Mensagem de erro a ser registrada</param>
        /// <param name="exception">Exceção associada ao erro</param>
        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, message);
            }
        }

        /// <summary>
        /// Registra uma mensagem de erro usando uma factory de mensagem.
        /// </summary>
        /// <param name="messageFactory">Factory que produz a mensagem de erro</param>
        public void Error(Func<string> messageFactory)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Registra uma mensagem de erro simples.
        /// </summary>
        /// <param name="message">Mensagem de erro a ser registrada</param>
        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(message);
            }
        }

        /// <summary>
        /// ErrorFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// ErrorFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// ErrorFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, format, args);
            }
        }

        /// <summary>
        /// ErrorFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(format, args);
            }
        }

        /// <summary>
        /// Fatal.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="exception">Parâmetro exception.</param>
        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, message);
            }
        }

        /// <summary>
        /// Fatal.
        /// </summary>
        /// <param name="messageFactory">Parâmetro messageFactory.</param>
        public void Fatal(Func<string> messageFactory)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Fatal.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(message);
            }
        }

        /// <summary>
        /// FatalFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// FatalFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// FatalFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, format, args);
            }
        }

        /// <summary>
        /// FatalFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void FatalFormat(string format, params object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(format, args);
            }
        }

        /// <summary>
        /// Info.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="exception">Parâmetro exception.</param>
        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(exception, message);
            }
        }

        /// <summary>
        /// Info.
        /// </summary>
        /// <param name="messageFactory">Parâmetro messageFactory.</param>
        public void Info(Func<string> messageFactory)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Info.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(message);
            }
        }

        /// <summary>
        /// InfoFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// InfoFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// InfoFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(exception, format, args);
            }
        }

        /// <summary>
        /// InfoFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void InfoFormat(string format, params object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Information(format, args);
            }
        }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public override string ToString()
        {
            return Logger.ToString();
        }

        /// <summary>
        /// Trace.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="exception">Parâmetro exception.</param>
        public void Trace(string message, Exception exception)
        {
            Logger.Verbose(exception, message);
        }

        /// <summary>
        /// Trace.
        /// </summary>
        /// <param name="messageFactory">Parâmetro messageFactory.</param>
        public void Trace(Func<string> messageFactory)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Trace.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public void Trace(string message)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(message);
            }
        }

        /// <summary>
        /// TraceFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// TraceFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// TraceFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void TraceFormat(Exception exception, string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(exception, format, args);
            }
        }

        /// <summary>
        /// TraceFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void TraceFormat(string format, params object[] args)
        {
            if (IsTraceEnabled)
            {
                Logger.Verbose(format, args);
            }
        }

        /// <summary>
        /// Warn.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        /// <param name="exception">Parâmetro exception.</param>
        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(exception, message);
            }
        }

        /// <summary>
        /// Warn.
        /// </summary>
        /// <param name="messageFactory">Parâmetro messageFactory.</param>
        public void Warn(Func<string> messageFactory)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(messageFactory.Invoke());
            }
        }

        /// <summary>
        /// Warn.
        /// </summary>
        /// <param name="message">Parâmetro message.</param>
        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(message);
            }
        }

        /// <summary>
        /// WarnFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(exception, string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// WarnFormat.
        /// </summary>
        /// <param name="formatProvider">Parâmetro formatProvider.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(string.Format(formatProvider, format, args));
            }
        }

        /// <summary>
        /// WarnFormat.
        /// </summary>
        /// <param name="exception">Parâmetro exception.</param>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(exception, format, args);
            }
        }

        /// <summary>
        /// WarnFormat.
        /// </summary>
        /// <param name="format">Parâmetro format.</param>
        /// <param name="args">Parâmetro args.</param>
        public void WarnFormat(string format, params object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warning(format, args);
            }
        }
    }
}