using System;

namespace Eaf.Middleware.RateLimiting
{
    /// <summary>
    /// Result of a rate limit check.
    /// </summary>
    public class RateLimitDecision
    {
        /// <summary>
        /// True when the request is allowed.
        /// </summary>
        public bool Allowed { get; set; }

        /// <summary>
        /// Seconds to wait before retrying when denied.
        /// </summary>
        public long RetryAfterSeconds { get; set; }

        /// <summary>
        /// UTC timestamp when the current window resets.
        /// </summary>
        public DateTime ResetAt { get; set; }
    }
}
