using Castle.Core.Logging;
using Serilog;
using System;
using ILogger = Castle.Core.Logging.ILogger;

namespace Eaf.Castle.Logging.SerilogIntegration
{
    /// <summary>
    /// Fábrica de loggers que integra Serilog com Castle Windsor.
    /// Responsável por criar instâncias de loggers configurados com Serilog.
    /// </summary>
    public class SerilogLoggerFactory : AbstractLoggerFactory
    {
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Inicializa uma nova instância da classe SerilogLoggerFactory usando o logger global do Serilog.
        /// </summary>
        public SerilogLoggerFactory()
        {
            logger = Log.Logger;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe SerilogLoggerFactory com um logger específico.
        /// </summary>
        /// <param name="logger">Instância do logger Serilog a ser utilizada</param>
        public SerilogLoggerFactory(Serilog.ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Cria um novo logger com o nome especificado.
        /// </summary>
        /// <param name="name">Nome do logger a ser criado</param>
        /// <returns>Nova instância de ILogger configurada com Serilog</returns>
        /// <exception cref="ArgumentNullException">Lançada quando o parâmetro name é nulo</exception>
        public override ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return new SerilogLogger(logger.ForContext(Serilog.Core.Constants.SourceContextPropertyName, name, false), this);
        }

        /// <summary>
        /// Cria um novo logger com nome e nível especificados.
        /// Nota: Níveis de log não podem ser definidos em tempo de execução no Serilog.
        /// </summary>
        /// <param name="name">Nome do logger</param>
        /// <param name="level">Nível do logger (não suportado)</param>
        /// <returns>Não retorna - sempre lança exceção</returns>
        /// <exception cref="NotSupportedException">Sempre lançada pois níveis não são suportados em tempo de execução</exception>
        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please see Serilog's LoggerConfiguration.MinimumLevel.");
        }
    }
}