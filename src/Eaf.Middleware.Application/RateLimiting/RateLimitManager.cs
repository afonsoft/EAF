using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Timing;
using Eaf.Middleware.RateLimiting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.Application.RateLimiting
{
    /// <summary>
    /// Cache-based rate limit manager. Future implementations may use Redis for atomic increments.
    /// </summary>
    public class RateLimitManager : IRateLimitManager, ITransientDependency
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _windowLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        private readonly ICacheManager _cacheManager;

        public RateLimitManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public async Task<RateLimitDecision> CheckAsync(
            string policy,
            string subject,
            TimeSpan window,
            int limit,
            CancellationToken cancellationToken = default)
        {
            if (limit <= 0)
            {
                return new RateLimitDecision { Allowed = true };
            }

            var windowKey = GetWindowKey(policy, subject, window);
            var cache = _cacheManager.GetCache("EafRateLimit").AsTyped<string, long>();
            var resetAt = GetWindowReset(window);
            var absoluteExpiry = new DateTimeOffset(resetAt);

            var windowLock = _windowLocks.GetOrAdd(windowKey, _ => new SemaphoreSlim(1, 1));
            await windowLock.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var current = await cache.GetOrDefaultAsync(windowKey);
                current++;

                await cache.SetAsync(
                    windowKey,
                    current,
                    slidingExpireTime: null,
                    absoluteExpireTime: absoluteExpiry);

                if (current > limit)
                {
                    return new RateLimitDecision
                    {
                        Allowed = false,
                        RetryAfterSeconds = Math.Max(0, (long)(resetAt - Clock.Now).TotalSeconds),
                        ResetAt = resetAt
                    };
                }

                return new RateLimitDecision
                {
                    Allowed = true,
                    ResetAt = resetAt
                };
            }
            finally
            {
                windowLock.Release();
            }
        }

        private static string GetWindowKey(string policy, string subject, TimeSpan window)
        {
            var windowStart = Clock.Now.Ticks / window.Ticks;
            return $"{policy}:{subject}:{windowStart}";
        }

        private static DateTime GetWindowReset(TimeSpan window)
        {
            var now = Clock.Now;
            return now.Add(window).Subtract(TimeSpan.FromTicks(now.Ticks % window.Ticks));
        }
    }
}
