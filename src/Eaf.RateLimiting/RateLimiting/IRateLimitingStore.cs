using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.RateLimiting
{
    /// <summary>
    /// Resultado de uma verificação de rate limit.
    /// </summary>
    public class EafRateLimitResult
    {
        /// <summary>Indica se a requisição é permitida.</summary>
        public bool Allowed { get; set; }

        /// <summary>Contagem atual na janela.</summary>
        public long Count { get; set; }

        /// <summary>Limite configurado.</summary>
        public long Limit { get; set; }

        /// <summary>Quando a janela atual expira (para cabeçalho Retry-After).</summary>
        public DateTimeOffset ResetsAt { get; set; }
    }

    /// <summary>
    /// Abstração de store de contadores de rate limit (janela deslizante).
    /// A implementação padrão é em memória; backends distribuídos (ex.: Redis)
    /// podem ser registrados via DI.
    /// </summary>
    public interface IRateLimitingStore
    {
        /// <summary>
        /// Incrementa o contador de <paramref name="key"/> na janela corrente e retorna o resultado.
        /// </summary>
        Task<EafRateLimitResult> HitAsync(string key, TimeSpan window, long limit, CancellationToken cancellationToken = default);
    }
}
