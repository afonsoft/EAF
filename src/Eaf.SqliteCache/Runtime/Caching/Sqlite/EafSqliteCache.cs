using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml;
using Abp.Runtime.Caching;
using Castle.Core.Logging;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using SQLitePCL;
using DbCommand = Microsoft.Data.Sqlite.SqliteCommand;
using DbConnection = Microsoft.Data.Sqlite.SqliteConnection;

namespace Abp.Runtime.Caching.Sqlite
{
    /// <summary>
    /// Implementação de cache baseada em SQLite que fornece armazenamento persistente de dados em cache.
    /// Suporta expiração automática de itens e limpeza periódica de dados expirados.
    /// </summary>
    public class EafSqliteCache : CacheBase
    {
        private readonly Timer? _cleanupTimer;
        private readonly DbConnection _db;
        private readonly object _lock = new object();
        private bool _disposed;

        /// <summary>
        /// Obtém o pool de comandos de banco de dados para operações de cache.
        /// </summary>
        private DbCommandPool Commands { get; }

        /// <summary>
        /// Remove todos os itens do cache.
        /// </summary>
        public override void Clear()
        {
            Commands.Use(Operation.RemoveAll, cmd => { cmd.ExecuteNonQuery(); });
        }

        /// <summary>
        /// Remove um item específico do cache usando sua chave.
        /// </summary>
        /// <param name="key">Chave do item a ser removido</param>
        public override void Remove(string key)
        {
            Commands.Use(Operation.Remove, cmd =>
            {
                cmd.Parameters.AddWithValue("@key", FixKey(key));
                cmd.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Adiciona ou atualiza um item no cache com configurações de expiração opcionais.
        /// </summary>
        /// <param name="key">Chave única do item</param>
        /// <param name="value">Valor a ser armazenado</param>
        /// <param name="slidingExpireTime">Tempo de expiração deslizante (opcional)</param>
        /// <param name="absoluteExpireTime">Tempo de expiração absoluto (opcional)</param>
        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            Commands.Use(Operation.Insert, cmd =>
            {
                CreateForSet(cmd, FixKey(key), ObjectToByteArray(value));
                cmd.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Tenta obter um valor do cache usando a chave especificada.
        /// Remove automaticamente itens expirados durante a consulta.
        /// </summary>
        /// <param name="key">Chave do item a ser recuperado</param>
        /// <param name="value">Valor recuperado do cache (se encontrado)</param>
        /// <returns>True se o item foi encontrado e não expirou; caso contrário, false</returns>
        public override bool TryGetValue(string key, out object value)
        {
            var item = (byte[])Commands.Use(Operation.Get, cmd =>
            {
                cmd.Parameters.AddWithValue("@key", FixKey(key));
                cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                return cmd.ExecuteScalar();
            })!;

            if (item == null || !item.Any())
            {
                value = null;
                return false;
            }

            value = ByteArrayToObject(item);
            return true;
        }

        #region Expired

        /// <summary>
        /// RemoveExpired.
        /// </summary>
        public void RemoveExpired()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                var removed = (long)Commands.Use(Operation.RemoveExpired, cmd =>
                {
                    cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                    return cmd.ExecuteScalar();
                })!;

                if (removed > 0)
                {
                    Logger.TraceFormat("Evicted {0} expired entries from cache", removed);
                }
            }
        }

        #endregion Expired

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

        #region TableInitCommand

        public const int SchemaVersion = 1;

        public const string TableInitCommand = "CREATE TABLE IF NOT EXISTS \"cache\" (	" +
                                               "  \"key\"	varchar NOT NULL," +
                                               "  \"value\"	BLOB," +
                                               "  \"expiry\"	INTEGER," +
                                               "  \"renewal\"	INTEGER," +
                                               "  PRIMARY KEY(\"key\")" +
                                               " ) WITHOUT ROWID;" +
                                               "" +
                                               " CREATE TABLE IF NOT EXISTS \"meta\" (" +
                                               "  \"key\"	TEXT NOT NULL," +
                                               "  \"value\"	INTEGER," +
                                               "  PRIMARY KEY(\"key\")" +
                                               " ) WITHOUT ROWID;" +
                                               "" +
                                               " CREATE INDEX IF NOT EXISTS \"cache_expiry\" ON \"cache\" (" +
                                               "  \"expiry\"" +
                                               " )";

        #endregion TableInitCommand

        #region constructor

        /// <summary>
        /// EafSqliteCache.
        /// </summary>
        /// <param name="name">Parâmetro name.</param>
        /// <param name="options">Parâmetro options.</param>
        /// <returns>Resultado da operação.</returns>
        public EafSqliteCache(string name, EafSqliteCacheOptions options) : base(name)
        {
            _db = Connect(options, Logger);
            Commands = new DbCommandPool(_db);

            // This has to be after the call to Connect()
            if (options.CleanupInterval.HasValue)
            {
                _cleanupTimer = new Timer(_ => { RemoveExpired(); }, null, TimeSpan.Zero,
                    options.CleanupInterval.Value);
            }
        }

        static EafSqliteCache()
        {
            Batteries.Init();
        }

        #endregion constructor

        #region CreateForSet and AddExpirationParameters

        private void CreateForSet(DbCommand cmd, string key, byte[] value, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);

            AddExpirationParameters(cmd, slidingExpireTime, absoluteExpireTime);
        }

        private void AddExpirationParameters(DbCommand cmd, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            DateTimeOffset? expiry = null;
            TimeSpan? renewal = null;

            if (absoluteExpireTime.HasValue)
            {
                expiry = absoluteExpireTime.Value.ToUniversalTime();
            }
            else if (DefaultAbsoluteExpireTime.HasValue)
            {
                expiry = DefaultAbsoluteExpireTime.Value.ToUniversalTime();
            }

            if (slidingExpireTime.HasValue)
            {
                renewal = slidingExpireTime.Value;
                expiry = (expiry ?? DateTimeOffset.UtcNow) + renewal;
            }
            else
            {
                renewal = DefaultSlidingExpireTime;
                expiry = (expiry ?? DateTimeOffset.UtcNow) + renewal;
            }

            cmd.Parameters.AddWithValue("@expiry", expiry?.Ticks ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@renewal", renewal?.Ticks ?? (object)DBNull.Value);
        }

        #endregion CreateForSet and AddExpirationParameters

        #region Database Connection Initialization

        private static DbConnection Connect(EafSqliteCacheOptions config, ILogger logger)
        {
            DbConnection? db = null;

            // First try to open an existing database
            if (!config.MemoryOnly && File.Exists(config.CachePath))
            {
                db = new DbConnection(config.ConnectionString);
                db.Open();

                if (!CheckExistingDb(db, logger))
                {
                    db.Close();
                    db.Dispose();
                    db = null;
                    File.Delete(config.CachePath);
                }
            }

            if (db is null)
            {
                db = new DbConnection(config.ConnectionString);
                db.Open();
                Initialize(db);
            }

            // Explicitly set default journal mode and fsync behavior
            using (var cmd = new DbCommand("PRAGMA journal_mode = WAL;", db))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new DbCommand("PRAGMA synchronous = NORMAL;", db))
            {
                cmd.ExecuteNonQuery();
            }

            return db;
        }

        private static void Initialize(DbConnection db)
        {
            using (var transaction = db.BeginTransaction())
            {
                using (var cmd = new DbCommand(TableInitCommand, db))
                {
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new DbCommand(
                           $"INSERT OR IGNORE INTO meta (key, value) " +
                           $"VALUES " +
                           $@"(""version"", {SchemaVersion}), " +
                           $@"(""created"", {DateTimeOffset.UtcNow.Ticks})", db))
                {
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private static bool CheckExistingDb(DbConnection db, ILogger logger)
        {
            try
            {
                // Check for correct structure
                using (var cmd = new DbCommand(@"SELECT COUNT(*) from sqlite_master", db))
                {
                    var result = (long)cmd.ExecuteScalar()!;
                    // We are expecting two tables and one additional index
                    if (result != 3)
                    {
                        logger.Trace("Incorrect/incompatible existing cache db structure found!");
                        return false;
                    }
                }

                // Check for correct version
                using (var cmd = new DbCommand(@"SELECT value FROM meta WHERE key = ""version""", db))
                {
                    var result = (long)cmd.ExecuteScalar()!;
                    if (result != SchemaVersion)
                    {
                        logger.TraceFormat("Existing cache db has unsupported schema version {0}", result);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Error while checking compatibility of existing cache db!", ex);
                return false;
            }

            return true;
        }

        #endregion Database Connection Initialization

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

        #region Dispose

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _cleanupTimer?.Dispose();
                Commands.Dispose();

                _db.Close();
                _db.Dispose();
                base.Dispose();
            }
        }

        #endregion Dispose
    }
}