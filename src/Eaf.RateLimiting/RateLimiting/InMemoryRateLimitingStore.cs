using Abp.Dependency;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.RateLimiting
{
    /// <summary>
    /// Store de rate limit em memória usando janela fixa com timestamps.
    /// Adequado para instância única; para múltiplas réplicas use um backend distribuído.
    /// </summary>
    public class InMemoryRateLimitingStore : IRateLimitingStore, ISingletonDependency
    {
        private sealed class Bucket
        {
            public long Count;
            public DateTimeOffset ResetsAt;
        }

        private readonly ConcurrentDictionary<string, Bucket> _buckets = new();

        /// <inheritdoc />
        public Task<EafRateLimitResult> HitAsync(string key, TimeSpan window, long limit, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var bucket = _buckets.AddOrUpdate(
                key,
                _ => new Bucket { Count = 1, ResetsAt = now.Add(window) },
                (_, existing) =>
                {
                    if (existing.ResetsAt <= now)
                    {
                        existing.Count = 1;
                        existing.ResetsAt = now.Add(window);
                    }
                    else
                    {
                        Interlocked.Increment(ref existing.Count);
                    }
                    return existing;
                });

            return Task.FromResult(new EafRateLimitResult
            {
                Allowed = bucket.Count <= limit,
                Count = bucket.Count,
                Limit = limit,
                ResetsAt = bucket.ResetsAt
            });
        }
    }
}
