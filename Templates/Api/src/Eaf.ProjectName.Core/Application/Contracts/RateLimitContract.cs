using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.ProjectName.Contracts
{
    /// <summary>
    /// Expõe a decisão operacional de uma política de limite.
    /// </summary>
    public sealed class RateLimitDecision
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a operação foi permitida.
        /// </summary>
        public bool Allowed { get; set; }

        /// <summary>
        /// Obtém ou define o limite configurado.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Obtém ou define o consumo observado.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Obtém ou define os segundos até uma nova tentativa.
        /// </summary>
        public int RetryAfterSeconds { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da política aplicada.
        /// </summary>
        public string Policy { get; set; }
    }

    /// <summary>
    /// Exemplo de contrato para rate limit compartilhado entre módulos.
    /// </summary>
    public interface IRateLimitManager
    {
        /// <summary>
        /// Avalia uma operação sem expor conteúdo privado ao consumidor.
        /// </summary>
        /// <param name="policy">Identificador da política.</param>
        /// <param name="subject">Sujeito tenant-aware da política.</param>
        /// <param name="window">Janela de contagem.</param>
        /// <param name="limit">Quantidade máxima permitida.</param>
        /// <param name="cancellationToken">Token de cancelamento da operação.</param>
        /// <returns>A decisão operacional da política.</returns>
        Task<RateLimitDecision> CheckAsync(
            string policy,
            string subject,
            TimeSpan window,
            int limit,
            CancellationToken cancellationToken = default);
    }
}
