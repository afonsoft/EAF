using System;

namespace Eaf.Middleware.Contracts
{
    /// <summary>
    /// Contract representing a rate limit decision returned to clients.
    /// </summary>
    public class RateLimitContract
    {
        /// <summary>
        /// True when the request is allowed.
        /// </summary>
        public bool Allowed { get; set; }

        /// <summary>
        /// Current number of requests within the window.
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Maximum number of requests allowed in the window.
        /// </summary>
        public long Limit { get; set; }

        /// <summary>
        /// UTC timestamp when the current window resets.
        /// </summary>
        public DateTime ResetAt { get; set; }

        /// <summary>
        /// Seconds to wait before retrying when denied.
        /// </summary>
        public long RetryAfterSeconds { get; set; }
    }
}
