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
        private static readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan _minCleanupThreshold = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan _maxCleanupThreshold = TimeSpan.FromHours(1);
        private static readonly ConcurrentDictionary<string, SemaphoreHolder> _windowLocks = new ConcurrentDictionary<string, SemaphoreHolder>();
        private static readonly Timer _cleanupTimer = new Timer(_ => CleanupIdleLocks(), null, _cleanupInterval, _cleanupInterval);

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

            var holder = _windowLocks.GetOrAdd(windowKey, _ => new SemaphoreHolder(window));
            await holder.WaitAsync(cancellationToken);
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
                holder.Release();
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

        private static void CleanupIdleLocks()
        {
            try
            {
                var now = Clock.Now;
                foreach (var kvp in _windowLocks)
                {
                    if (kvp.Value.CanCleanup(now, _minCleanupThreshold, _maxCleanupThreshold))
                    {
                        if (_windowLocks.TryRemove(kvp.Key, out var removed))
                        {
                            removed?.Dispose();
                        }
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors to avoid crashing the rate limiter.
            }
        }

        private class SemaphoreHolder : IDisposable
        {
            private const int MaxConcurrency = 1;
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(MaxConcurrency, MaxConcurrency);

            public SemaphoreHolder(TimeSpan window)
            {
                Window = window;
                LastUsed = Clock.Now;
            }

            public TimeSpan Window { get; }

            public DateTime LastUsed { get; private set; }

            public async Task WaitAsync(CancellationToken cancellationToken)
            {
                LastUsed = Clock.Now;
                await _semaphore.WaitAsync(cancellationToken);
            }

            public void Release()
            {
                LastUsed = Clock.Now;
                _semaphore.Release();
            }

            public bool CanCleanup(DateTime now, TimeSpan minThreshold, TimeSpan maxThreshold)
            {
                var threshold = TimeSpan.FromTicks(Math.Min(Window.Ticks * 2, maxThreshold.Ticks));
                if (threshold < minThreshold)
                    threshold = minThreshold;

                return (now - LastUsed) > threshold && _semaphore.CurrentCount == MaxConcurrency;
            }

            public void Dispose()
            {
                _semaphore.Dispose();
            }
        }
    }
}
