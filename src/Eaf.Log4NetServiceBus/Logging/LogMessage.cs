using System;

namespace Eaf.Log4NetServiceBus.Logging
{
    /// <summary>
    /// Representa uma mensagem de log estruturada para envio via Service Bus.
    /// Contém todas as informações necessárias para rastreamento e auditoria de eventos de aplicação.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Nome da aplicação que gerou o log.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Identificador de correlação para rastreamento de requisições distribuídas.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Nome do evento ou ação que gerou o log.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Data e hora do evento em UTC.
        /// </summary>
        public DateTime EventDateUTC { get; set; }

        /// <summary>
        /// Dados adicionais do evento serializados em formato JSON.
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// Nível de severidade do log (Debug, Info, Warn, Error, Fatal).
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Mensagem principal do log.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Data e hora em UTC quando o log deve ser removido do armazenamento.
        /// </summary>
        public DateTime PurgeDateUTC { get; set; }

        /// <summary>
        /// Tempo de retenção do log em dias.
        /// </summary>
        public int RetentionTime { get; set; }

        /// <summary>
        /// Nome do servidor que gerou o log.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Tipo de armazenamento utilizado para persistir o log.
        /// </summary>
        public string StorageType { get; set; }
    }
}