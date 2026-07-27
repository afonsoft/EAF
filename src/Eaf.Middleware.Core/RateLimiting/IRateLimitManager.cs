using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.RateLimiting
{
    /// <summary>
    /// Shared rate limit manager used by host applications.
    /// </summary>
    public interface IRateLimitManager
    {
        /// <summary>
        /// Checks whether a request is allowed under the specified policy and subject.
        /// </summary>
        Task<RateLimitDecision> CheckAsync(
            string policy,
            string subject,
            TimeSpan window,
            int limit,
            CancellationToken cancellationToken = default);
    }
}
