using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.ProjectName.Contracts
{
    /// <summary>
    /// Descreve uma decisão operacional de moderação sem PII pública.
    /// </summary>
    public sealed class ModerationAuditEntry
    {
        /// <summary>
        /// Obtém ou define o tenant da ação.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Obtém ou define o usuário executor.
        /// </summary>
        public Guid? ExecutorUserId { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da ação.
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// Obtém ou define o alvo anonimizado.
        /// </summary>
        public string AnonymizedTarget { get; set; }

        /// <summary>
        /// Obtém ou define o motivo da decisão.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Obtém ou define a decisão tomada.
        /// </summary>
        public string Decision { get; set; }

        /// <summary>
        /// Obtém ou define o identificador de correlação.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Obtém ou define a data UTC da ação.
        /// </summary>
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// Obtém ou define o jogo associado, quando aplicável.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// Obtém ou define a partida associada, quando aplicável.
        /// </summary>
        public Guid? MatchId { get; set; }

        /// <summary>
        /// Obtém ou define o report associado, quando aplicável.
        /// </summary>
        public Guid? ReportId { get; set; }
    }

    /// <summary>
    /// Exemplo de contrato para gravação de auditoria de moderação.
    /// </summary>
    public interface IModerationAuditWriter
    {
        /// <summary>
        /// Grava uma ação de moderação dentro do tenant autorizado.
        /// </summary>
        /// <param name="entry">Registro operacional da ação.</param>
        /// <param name="cancellationToken">Token de cancelamento da operação.</param>
        /// <returns>Uma tarefa que representa a gravação.</returns>
        Task WriteAsync(
            ModerationAuditEntry entry,
            CancellationToken cancellationToken = default);
    }
}
