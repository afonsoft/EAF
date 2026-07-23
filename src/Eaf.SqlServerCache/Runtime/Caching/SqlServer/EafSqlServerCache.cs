using Abp.Runtime.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Runtime.Caching.SqlServer
{
    /// <summary>
    /// Representa a classe EafSqlServerCache.
    /// </summary>
    public class EafSqlServerCache : CacheBase
    {
        private readonly IDistributedCache _cache;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly DistributedCacheEntryOptions _distributedCacheEntryOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        };

        /// <summary>
        /// EafSqlServerCache.
        /// </summary>
        /// <param name="name">Parâmetro name.</param>
        /// <param name="cache">Parâmetro cache.</param>
        public EafSqlServerCache(string name, IDistributedCache cache) : base(name)
        {
            _cache = cache;
            DefaultAbsoluteExpireTime = _distributedCacheEntryOptions.AbsoluteExpiration;
            DefaultSlidingExpireTime = _distributedCacheEntryOptions.SlidingExpiration.Value;
        }

        #region FixKey

        private string FixKey(string key)
        {
            if (key.Contains("_") && key.Contains(Name))
            {
                return key;
            }

            if (key.Contains(Name))
            {
                return Name + "_" + key.Replace(Name, "");
            }

            return Name + "_" + key;
        }

        #endregion FixKey

        #region Compress/Decompress

        /// <summary>
        /// Compress byte for use em cache with GZipStream (synchronous).
        /// </summary>
        private static byte[] CompressBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var outputStream = new MemoryStream();
                using (var compressionStream = new GZipStream(outputStream, CompressionLevel.Optimal, true))
                {
                    compressionStream.Write(bytes, 0, bytes.Length);
                }
                return outputStream.ToArray();
            }
            catch
            {
                return bytes;
            }
        }

        /// <summary>
        /// Compress byte for use em cache with GZipStream (asynchronous).
        /// </summary>
        private static async Task<byte[]> CompressBytesAsync(byte[] bytes, CancellationToken cancel = default)
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var outputStream = new MemoryStream();
                using (var compressionStream = new GZipStream(outputStream, CompressionLevel.Optimal, true))
                {
                    await compressionStream.WriteAsync(bytes.AsMemory(0, bytes.Length), cancel);
                }
                return outputStream.ToArray();
            }
            catch (Exception)
            {
                return bytes;
            }
        }

        /// <summary>
        /// Decompress byte from cache with GZipStream (synchronous).
        /// </summary>
        private static byte[] DecompressBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var inputStream = new MemoryStream(bytes);
                using var outputStream = new MemoryStream();
                using (var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                {
                    compressionStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
            catch (Exception)
            {
                return bytes;
            }
        }

        /// <summary>
        /// Decompress byte from cache with GZipStream (asynchronous).
        /// </summary>
        private static async Task<byte[]> DecompressBytesAsync(byte[] bytes, CancellationToken cancel = default)
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var inputStream = new MemoryStream(bytes);
                using var outputStream = new MemoryStream();
                using (var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                {
                    await compressionStream.CopyToAsync(outputStream, cancel);
                }
                return outputStream.ToArray();
            }
            catch (Exception)
            {
                return bytes;
            }
        }

        #endregion Compress/Decompress

        /// <summary>
        /// TryGetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        /// <returns>Resultado da operação.</returns>
        public override bool TryGetValue(string key, out object value)
        {
            try
            {
                var encodedCached = _cache.Get(FixKey(key));

                if (encodedCached != null)
                {
                    var cached = ByteArrayToObject(DecompressBytes(encodedCached));
                    if (cached != null)
                    {
                        value = cached;
                        return true;
                    }
                }

                value = null;
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in TryGetValue in EafSqlServerCache", ex);
                value = null;
                return false;
            }
        }

        /// <summary>
        /// TryGetValueAsync.
        /// </summary>
        public override async Task<Abp.Data.ConditionalValue<object>> TryGetValueAsync(string key)
        {
            try
            {
                var encodedCached = await _cache.GetAsync(FixKey(key));

                if (encodedCached != null)
                {
                    var cached = ByteArrayToObject(await DecompressBytesAsync(encodedCached));
                    if (cached != null)
                    {
                        return new Abp.Data.ConditionalValue<object>(true, cached);
                    }
                }

                return new Abp.Data.ConditionalValue<object>(false, null);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in TryGetValueAsync in EafSqlServerCache", ex);
                return new Abp.Data.ConditionalValue<object>(false, null);
            }
        }

        /// <summary>
        /// Set.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        /// <param name="slidingExpireTime">Parâmetro slidingExpireTime.</param>
        /// <param name="absoluteExpireTime">Parâmetro absoluteExpireTime.</param>
        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            var encodedCurrent = ObjectToByteArray(value);
            var compressedData = CompressBytes(encodedCurrent);

            _cache.Set(FixKey(key), compressedData, CreateOptions(slidingExpireTime, absoluteExpireTime));
        }

        /// <summary>
        /// SetAsync.
        /// </summary>
        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            var encodedCurrent = ObjectToByteArray(value);
            var compressedData = await CompressBytesAsync(encodedCurrent);

            await _cache.SetAsync(FixKey(key), compressedData, CreateOptions(slidingExpireTime, absoluteExpireTime));
        }

        /// <summary>
        /// Remove.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        public override void Remove(string key)
        {
            _cache.Remove(FixKey(key));
        }

        /// <summary>
        /// RemoveAsync.
        /// </summary>
        public override Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(FixKey(key));
        }

        /// <summary>
        /// Clear.
        /// </summary>
        public override void Clear()
        {
            //Ignore
        }

        #region Helpers

        private static DistributedCacheEntryOptions CreateOptions(TimeSpan? slidingExpireTime, DateTimeOffset? absoluteExpireTime)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpireTime,
                SlidingExpiration = slidingExpireTime ?? _distributedCacheEntryOptions.SlidingExpiration,
                AbsoluteExpirationRelativeToNow = absoluteExpireTime.HasValue
                    ? absoluteExpireTime.Value - DateTimeOffset.UtcNow
                    : _distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow
            };
        }

        /// <summary>
        /// Serializa um objeto para array de bytes usando JSON UTF-8.
        /// </summary>
        /// <param name="objData">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto serializado.</returns>
        private static byte[] ObjectToByteArray(object objData)
        {
            if (objData == null)
                return Array.Empty<byte>();

            try
            {
                return JsonSerializer.SerializeToUtf8Bytes(objData, _jsonOptions);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Desserializa um array de bytes para objeto usando JSON UTF-8.
        /// </summary>
        /// <param name="byteArray">Array de bytes a desserializar.</param>
        /// <returns>Objeto desserializado.</returns>
        private static object ByteArrayToObject(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return default;

            try
            {
                return JsonSerializer.Deserialize<object>(byteArray, _jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        #endregion Helpers
    }
}
