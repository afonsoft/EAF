using BenchmarkDotNet.Attributes;
using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.Application.Benchmarks
{
    /// <summary>
    /// Benchmarks for EafSqlServerCache serialization and compression.
    /// </summary>
    [ShortRunJob]
    [MemoryDiagnoser]
    public class SqlServerCacheBenchmarks
    {
        private readonly InMemoryDistributedCache _distributedCache = new InMemoryDistributedCache();
        private EafSqlServerCache _cache = null!;
        private object _smallValue = null!;
        private object _largeValue = null!;

        /// <summary>
        /// Setup.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _cache = new EafSqlServerCache("TestCache", _distributedCache);
            _smallValue = new { Id = 1, Name = "Test" };
            _largeValue = new string('x', 10000);
        }

        /// <summary>
        /// Set small object.
        /// </summary>
        [Benchmark]
        public void SetSmallObject() => _cache.Set("small", _smallValue, TimeSpan.FromMinutes(1));

        /// <summary>
        /// Get small object.
        /// </summary>
        [Benchmark]
        public object? GetSmallObject()
        {
            _cache.Set("small", _smallValue, TimeSpan.FromMinutes(1));
            _cache.TryGetValue("small", out var value);
            return value;
        }

        /// <summary>
        /// Set large object.
        /// </summary>
        [Benchmark]
        public void SetLargeObject() => _cache.Set("large", _largeValue, TimeSpan.FromMinutes(1));

        /// <summary>
        /// Get large object.
        /// </summary>
        [Benchmark]
        public object? GetLargeObject()
        {
            _cache.Set("large", _largeValue, TimeSpan.FromMinutes(1));
            _cache.TryGetValue("large", out var value);
            return value;
        }

        private class InMemoryDistributedCache : IDistributedCache
        {
            private readonly ConcurrentDictionary<string, byte[]> _data = new ConcurrentDictionary<string, byte[]>();

            public byte[]? Get(string key) => _data.TryGetValue(key, out var value) ? value : null;

            public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
                => Task.FromResult<byte[]?>(Get(key));

            public void Refresh(string key)
            {
            }

            public Task RefreshAsync(string key, CancellationToken token = default)
                => Task.CompletedTask;

            public void Remove(string key) => _data.TryRemove(key, out _);

            public Task RemoveAsync(string key, CancellationToken token = default)
            {
                Remove(key);
                return Task.CompletedTask;
            }

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
                => _data[key] = value;

            public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
            {
                Set(key, value, options);
                return Task.CompletedTask;
            }
        }
    }
}
