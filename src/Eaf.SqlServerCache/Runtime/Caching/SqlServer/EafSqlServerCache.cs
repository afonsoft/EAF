using Abp.Runtime.Caching;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Eaf.Runtime.Caching.SqlServer
{
    /// <summary>
    /// Representa a classe EafSqlServerCache.
    /// </summary>
    public class EafSqlServerCache : CacheBase
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// EafSqlServerCache.
        /// </summary>
        /// <param name="name">Parâmetro name.</param>
        /// <param name="cache">Parâmetro cache.</param>
        /// <returns>Resultado da operação.</returns>
        public EafSqlServerCache(string name, IDistributedCache cache) : base(name)
        {
            _cache = cache;
            DefaultAbsoluteExpireTime = _distributedCacheEntryOptions.AbsoluteExpiration;
            DefaultSlidingExpireTime = _distributedCacheEntryOptions.SlidingExpiration.Value;
        }

        #region DistributedCacheEntryOptions

        /// <summary>
        /// But in large-scale applications where we face a lot of calls that must be cached, it is better to create a static field
        /// </summary>
        private static DistributedCacheEntryOptions _distributedCacheEntryOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10), //expira se ficar mais de 10 minutos sem usar o cache
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) //expira depois de 12h do cache criado.
        };

        #endregion DistributedCacheEntryOptions

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

        #region DecompressBytesAsync & CompressBytesAsync

        /// <summary>
        /// Compress byte for use em cache with GZipStream
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private static async Task<byte[]> CompressBytesAsync(byte[] bytes, CancellationToken cancel = default(CancellationToken))
        {
            try
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var compressionStream = new GZipStream(outputStream, CompressionLevel.Optimal))
                    {
                        await compressionStream.WriteAsync(bytes, 0, bytes.Length, cancel);
                    }
                    return outputStream.ToArray();
                }
            }
            catch
            {
                return bytes;
            }
        }

        /// <summary>
        /// Decompress byte from cache with GZipStream
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        private static async Task<byte[]> DecompressBytesAsync(byte[] bytes, CancellationToken cancel = default(CancellationToken))
        {
            try
            {
                using (var inputStream = new MemoryStream(bytes))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            await compressionStream.CopyToAsync(outputStream, cancel);
                        }
                        return outputStream.ToArray();
                    }
                }
            }
            catch
            {
                return bytes;
            }
        }

        #endregion DecompressBytesAsync & CompressBytesAsync

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
                var encodedCached = _cache.GetAsync(FixKey(key)).GetAwaiter().GetResult();

                if (encodedCached != null)
                {
                    var cached = ByteArrayToObject(DecompressBytesAsync(encodedCached).GetAwaiter().GetResult());
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
        /// Set.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        /// <param name="slidingExpireTime">Parâmetro slidingExpireTime.</param>
        /// <param name="absoluteExpireTime">Parâmetro absoluteExpireTime.</param>
        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            var encodedCurrent = ObjectToByteArray(value);
            _cache.SetAsync(FixKey(key), CompressBytesAsync(encodedCurrent).GetAwaiter().GetResult(), new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpireTime ?? DefaultAbsoluteExpireTime, SlidingExpiration = slidingExpireTime ?? DefaultSlidingExpireTime }).GetAwaiter();
        }

        /// <summary>
        /// Remove.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        public override void Remove(string key)
        {
            _cache.RemoveAsync(FixKey(key)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Clear.
        /// </summary>
        public override void Clear()
        {
            //Ignore
        }

        #region ObjectToByteArray and ByteArrayToObject

        private static byte[] ObjectToByteArray(object objData)
        {
            if (objData == null)
            {
                return default;
            }

            try
            {
                var serializer = new ConfigurationContainer().UseAutoFormatting()
                    .UseOptimizedNamespaces()
                    .Create();

                using (var contentStream = new MemoryStream())
                {
                    using (var writer = XmlWriter.Create(contentStream))
                    {
                        serializer.Serialize(writer, objData);
                        writer.Flush();
                    }

                    contentStream.Seek(0, SeekOrigin.Begin);
                    return Encoding.ASCII.GetBytes(new StreamReader(contentStream).ReadToEnd());
                }
            }
            catch
            {
                return SerializeToStream(objData);
            }
        }

        private static object ByteArrayToObject(byte[] byteArray)
        {
            if (byteArray == null || !byteArray.Any())
            {
                return default;
            }

            try
            {
                var serializer = new ConfigurationContainer().UseAutoFormatting()
                    .UseOptimizedNamespaces()
                    .Create();

                using (var contentStream = new MemoryStream(byteArray))
                {
                    using (var reader = XmlReader.Create(contentStream))
                    {
                        return serializer.Deserialize(reader);
                    }
                }
            }
            catch
            {
                return DeserializeFromStream(byteArray);
            }
        }

#pragma warning disable SYSLIB0011

        private static byte[] SerializeToStream(object objectType)
        {
            var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectType);
            return stream.ToArray();
        }

        private static object DeserializeFromStream(byte[] objectByte)
        {
            var stream = new MemoryStream(objectByte);
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            var objectType = formatter.Deserialize(stream);
            return objectType;
        }

#pragma warning disable SYSLIB0011

        #endregion ObjectToByteArray and ByteArrayToObject
    }
}