using Abp.Data;
using Abp.Runtime.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Runtime.Caching.Redis
{
    /// <summary>
    /// Implementação de cache distribuído baseada em Redis para o EAF.
    /// Utiliza <see cref="IDistributedCache"/> (RedisCache) para operações de leitura/escrita
    /// e <see cref="StackExchange.Redis"/> para limpar chaves por prefixo.
    /// </summary>
    public class EafRedisCache : CacheBase
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<RedisCacheOptions> _optionsAccessor;
        private readonly IConnectionMultiplexer? _connectionMultiplexer;

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
        /// Inicializa uma nova instância de <see cref="EafRedisCache"/>.
        /// </summary>
        /// <param name="name">Nome do cache.</param>
        /// <param name="cache">Implementação de <see cref="IDistributedCache"/>.</param>
        /// <param name="optionsAccessor">Opções do Redis.</param>
        /// <param name="connectionMultiplexer">Conexão Redis opcional para operações de Clear.</param>
        public EafRedisCache(
            string name,
            IDistributedCache cache,
            IOptions<RedisCacheOptions> optionsAccessor,
            IConnectionMultiplexer? connectionMultiplexer = null) : base(name)
        {
            _cache = cache;
            _optionsAccessor = optionsAccessor;
            _connectionMultiplexer = connectionMultiplexer;

            DefaultAbsoluteExpireTime = _distributedCacheEntryOptions.AbsoluteExpiration;
            DefaultSlidingExpireTime = _distributedCacheEntryOptions.SlidingExpiration.Value;
        }

        #region Fail-Open Helpers

        /// <summary>
        /// Determina se uma exceção é fatal e não deve ser interceptada pelo cache.
        /// </summary>
        private static bool IsFatalException(Exception exception)
        {
            return exception is OutOfMemoryException
                || exception is StackOverflowException
                || exception is ThreadAbortException;
        }

        /// <summary>
        /// Executa uma ação com comportamento fail-open: falhas são logadas e não quebram o host.
        /// </summary>
        private void ExecuteFailOpen(Action action, string context)
        {
            try
            {
                action();
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                Logger.Error($"Error in {context} in EafRedisCache", ex);
            }
        }

        /// <summary>
        /// Executa uma ação com retorno e comportamento fail-open.
        /// </summary>
        private T ExecuteFailOpen<T>(Func<T> action, string context, T defaultValue)
        {
            try
            {
                return action();
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                Logger.Error($"Error in {context} in EafRedisCache", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Executa uma ação assíncrona com comportamento fail-open.
        /// </summary>
        private async Task ExecuteFailOpenAsync(Func<Task> action, string context)
        {
            try
            {
                await action();
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                Logger.Error($"Error in {context} in EafRedisCache", ex);
            }
        }

        /// <summary>
        /// Executa uma ação assíncrona com retorno e comportamento fail-open.
        /// </summary>
        private async Task<T> ExecuteFailOpenAsync<T>(Func<Task<T>> action, string context, T defaultValue)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                Logger.Error($"Error in {context} in EafRedisCache", ex);
                return defaultValue;
            }
        }

        #endregion Fail-Open Helpers

        #region FixKey

        private string FixKey(string key)
        {
            if (key.Contains("_") && key.Contains(Name))
                return key;

            if (key.Contains(Name))
                return Name + "_" + key.Replace(Name, "");

            return Name + "_" + key;
        }

        #endregion FixKey

        #region Compress/Decompress

        /// <summary>
        /// Comprime um array de bytes usando GZipStream (síncrono).
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
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return bytes;
            }
        }

        /// <summary>
        /// Comprime um array de bytes usando GZipStream (assíncrono).
        /// </summary>
        private static async Task<byte[]> CompressBytesAsync(byte[] bytes, CancellationToken cancel = default)
        {
            if (bytes == null)
                return null!;

            if (bytes.Length == 0)
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
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return bytes;
            }
        }

        /// <summary>
        /// Descomprime um array de bytes usando GZipStream (síncrono).
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
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return bytes;
            }
        }

        /// <summary>
        /// Descomprime um array de bytes usando GZipStream (assíncrono).
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
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return bytes;
            }
        }

        #endregion Compress/Decompress

        /// <summary>
        /// Tenta recuperar um valor do cache.
        /// </summary>
        public override bool TryGetValue(string key, out object value)
        {
            var result = ExecuteFailOpen(() =>
            {
                var encodedCached = _cache.Get(FixKey(key));

                if (encodedCached != null)
                {
                    var cached = ByteArrayToObject(DecompressBytes(encodedCached));
                    if (cached != null)
                    {
                        return new ConditionalValue<object>(true, cached);
                    }
                }

                return new ConditionalValue<object>(false, null!);
            }, "TryGetValue", new ConditionalValue<object>(false, null!));

            value = result.Value;
            return result.HasValue;
        }

        /// <summary>
        /// Tenta recuperar um valor do cache de forma assíncrona.
        /// </summary>
        public override async Task<ConditionalValue<object>> TryGetValueAsync(string key)
        {
            return await ExecuteFailOpenAsync(async () =>
            {
                var encodedCached = await _cache.GetAsync(FixKey(key));

                if (encodedCached != null)
                {
                    var cached = ByteArrayToObject(await DecompressBytesAsync(encodedCached));
                    if (cached != null)
                    {
                        return new ConditionalValue<object>(true, cached);
                    }
                }

                return new ConditionalValue<object>(false, null!);
            }, "TryGetValueAsync", new ConditionalValue<object>(false, null!));
        }

        /// <summary>
        /// Armazena um valor no cache.
        /// </summary>
        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            ExecuteFailOpen(() =>
            {
                var encodedCurrent = ObjectToByteArray(value);
                var compressedData = CompressBytes(encodedCurrent);

                _cache.Set(FixKey(key), compressedData, CreateOptions(slidingExpireTime, absoluteExpireTime));
            }, "Set");
        }

        /// <summary>
        /// Armazena um valor no cache de forma assíncrona.
        /// </summary>
        public override Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            return ExecuteFailOpenAsync(async () =>
            {
                var encodedCurrent = ObjectToByteArray(value);
                var compressedData = await CompressBytesAsync(encodedCurrent);

                await _cache.SetAsync(FixKey(key), compressedData, CreateOptions(slidingExpireTime, absoluteExpireTime));
            }, "SetAsync");
        }

        /// <summary>
        /// Remove um item do cache.
        /// </summary>
        public override void Remove(string key)
        {
            ExecuteFailOpen(() => _cache.Remove(FixKey(key)), "Remove");
        }

        /// <summary>
        /// Remove um item do cache de forma assíncrona.
        /// </summary>
        public override Task RemoveAsync(string key)
        {
            return ExecuteFailOpenAsync(() => _cache.RemoveAsync(FixKey(key)), "RemoveAsync");
        }

        /// <summary>
        /// Limpa todas as chaves do cache com o prefixo deste cache.
        /// </summary>
        public override void Clear()
        {
            ExecuteFailOpen(() =>
            {
                if (_connectionMultiplexer != null)
                {
                    ClearWithMultiplexer(_connectionMultiplexer);
                    return;
                }

                var connectionString = _optionsAccessor.Value.Configuration;
                if (string.IsNullOrWhiteSpace(connectionString))
                    return;

                using var connection = ConnectionMultiplexer.Connect(connectionString);
                ClearWithMultiplexer(connection);
            }, "Clear");
        }

        /// <summary>
        /// Limpa todas as chaves do cache com o prefixo deste cache de forma assíncrona.
        /// </summary>
        public override Task ClearAsync()
        {
            Clear();
            return Task.CompletedTask;
        }

        private void ClearWithMultiplexer(IConnectionMultiplexer multiplexer)
        {
            var endpoint = multiplexer.GetEndPoints().FirstOrDefault();
            if (endpoint == null)
                return;

            var server = multiplexer.GetServer(endpoint);
            var prefix = GetKeyPrefix();

            var keys = server.Keys(pattern: prefix + "*", pageSize: 100).ToArray();
            if (keys.Length == 0)
                return;

            var db = multiplexer.GetDatabase();
            db.KeyDelete(keys);
        }

        private string GetKeyPrefix()
        {
            var instanceName = _optionsAccessor.Value.InstanceName;
            var prefix = string.IsNullOrWhiteSpace(instanceName)
                ? Name + "_"
                : instanceName + ":" + Name + "_";

            return prefix;
        }

        #region Helpers

        private static DistributedCacheEntryOptions CreateOptions(TimeSpan? slidingExpireTime, DateTimeOffset? absoluteExpireTime)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpireTime ?? _distributedCacheEntryOptions.SlidingExpiration
            };

            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpiration = absoluteExpireTime.Value;
            else
                options.AbsoluteExpirationRelativeToNow = _distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow;

            return options;
        }

        /// <summary>
        /// Serializa um objeto para array de bytes usando JSON UTF-8 prefixado com o tipo do objeto.
        /// </summary>
        private static byte[] ObjectToByteArray(object objData)
        {
            if (objData == null)
                return Array.Empty<byte>();

            try
            {
                var typeName = objData.GetType().AssemblyQualifiedName;
                var typeNameBytes = Encoding.UTF8.GetBytes(typeName);

                var buffer = new ArrayBufferWriter<byte>(typeNameBytes.Length + 64);
                buffer.Write(typeNameBytes);

                var separatorSpan = buffer.GetSpan(1);
                separatorSpan[0] = (byte)'\n';
                buffer.Advance(1);

                using (var writer = new Utf8JsonWriter(buffer))
                {
                    JsonSerializer.Serialize(writer, objData, objData.GetType(), _jsonOptions);
                    writer.Flush();
                }

                return buffer.WrittenMemory.ToArray();
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Desserializa um array de bytes para objeto usando JSON UTF-8.
        /// </summary>
        private static object ByteArrayToObject(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return default!;

            try
            {
                var span = byteArray.AsSpan();
                var separatorIndex = span.IndexOf((byte)'\n');
                if (separatorIndex < 0)
                {
                    return JsonSerializer.Deserialize<object>(byteArray, _jsonOptions)!;
                }

                var typeName = Encoding.UTF8.GetString(span.Slice(0, separatorIndex));
                var type = Type.GetType(typeName, throwOnError: true);
                var jsonSpan = span.Slice(separatorIndex + 1);

                if (typeName.StartsWith("System.Tuple") || typeName.StartsWith("System.ValueTuple"))
                    return JsonSerializer.Deserialize<object>(jsonSpan, _jsonOptions)!;

                return JsonSerializer.Deserialize(jsonSpan, type, _jsonOptions)!;
            }
            catch (Exception ex) when (!IsFatalException(ex))
            {
                return default!;
            }
        }

        #endregion Helpers
    }
}
