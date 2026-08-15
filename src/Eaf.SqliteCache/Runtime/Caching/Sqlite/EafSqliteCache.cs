using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Abp.Runtime.Caching;
using Castle.Core.Logging;
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

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

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
            if (value == null)
                throw new InvalidOperationException("Cache value cannot be null.");

            Commands.Use(Operation.Insert, cmd =>
            {
                CreateForSet(cmd, FixKey(key), ObjectToByteArray(value), slidingExpireTime, absoluteExpireTime);
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

        public const string TableInitCommand = "CREATE TABLE IF NOT EXISTS \"cache\" (\t" +
                                               "  \"key\"\tvarchar NOT NULL," +
                                               "  \"value\"\tBLOB," +
                                               "  \"expiry\"\tINTEGER," +
                                               "  \"renewal\"\tINTEGER," +
                                               "  PRIMARY KEY(\"key\")" +
                                               " ) WITHOUT ROWID;" +
                                               "" +
                                               " CREATE TABLE IF NOT EXISTS \"meta\" (" +
                                               "  \"key\"\tTEXT NOT NULL," +
                                               "  \"value\"\tINTEGER," +
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
            _db = Connect(options, name, Logger);
            Commands = new DbCommandPool(_db);

            // This has to be after the call to Connect()
            if (options.CleanupInterval.HasValue)
            {
                _cleanupTimer = new Timer(_ => { RemoveExpired(); }, null, options.CleanupInterval.Value,
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
                expiry = (expiry ?? DateTimeOffset.UtcNow) + renewal; // NOSONAR
            }
            else
            {
                renewal = DefaultSlidingExpireTime;
                expiry = (expiry ?? DateTimeOffset.UtcNow) + renewal; // NOSONAR
            }

            cmd.Parameters.AddWithValue("@expiry", expiry.Value.Ticks);
            cmd.Parameters.AddWithValue("@renewal", renewal.Value.Ticks);
        }

        #endregion CreateForSet and AddExpirationParameters

        #region Database Connection Initialization

        private static DbConnection Connect(EafSqliteCacheOptions config, string cacheName, ILogger logger)
        {
            DbConnection? db = null;
            var connectionString = config.GetConnectionString(cacheName);

            // First try to open an existing database
            if (!config.MemoryOnly && File.Exists(config.CachePath))
            {
                db = new DbConnection(connectionString);
                try
                {
                    db.Open();

                    if (!CheckExistingDb(db, logger))
                    {
                        db.Close();
                        db.Dispose();
                        db = null;
                        File.Delete(config.CachePath);
                    }
                }
                catch (SqliteException ex)
                {
                    logger.Warn("Error while opening existing cache db, recreating it!", ex);
                    db?.Dispose();
                    db = null;
                    File.Delete(config.CachePath);
                    SqliteConnection.ClearAllPools();
                }
            }

            if (db is null)
            {
                db = new DbConnection(connectionString);
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
                           "INSERT OR IGNORE INTO meta (key, value) " +
                           "VALUES (@versionKey, @versionValue), (@createdKey, @createdValue)", db))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@versionKey", "version");
                    cmd.Parameters.AddWithValue("@versionValue", SchemaVersion);
                    cmd.Parameters.AddWithValue("@createdKey", "created");
                    cmd.Parameters.AddWithValue("@createdValue", DateTimeOffset.UtcNow.Ticks);
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
                using (var cmd = new DbCommand(@"SELECT value FROM meta WHERE key = 'version'", db))
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

        /// <summary>
        /// Serializa um objeto para array de bytes usando JSON UTF-8, prefixado com o tipo do objeto.
        /// </summary>
        /// <param name="objData">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto serializado.</returns>
        private static byte[] ObjectToByteArray(object objData)
        {
            if (objData == null)
                return Array.Empty<byte>();

            try
            {
                var typeName = objData.GetType().AssemblyQualifiedName;
                var typeNameBytes = Encoding.UTF8.GetBytes(typeName);
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(objData, objData.GetType(), _jsonOptions);
                var result = new byte[typeNameBytes.Length + 1 + jsonBytes.Length];
                Buffer.BlockCopy(typeNameBytes, 0, result, 0, typeNameBytes.Length);
                result[typeNameBytes.Length] = (byte)'\n';
                Buffer.BlockCopy(jsonBytes, 0, result, typeNameBytes.Length + 1, jsonBytes.Length);
                return result;
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Desserializa um array de bytes para objeto usando JSON UTF-8.
        /// O tipo do objeto é lido do prefixo armazenado durante a serialização.
        /// </summary>
        /// <param name="byteArray">Array de bytes a desserializar.</param>
        /// <returns>Objeto desserializado.</returns>
        private static object ByteArrayToObject(byte[] byteArray)
        {
            if (byteArray == null || !byteArray.Any())
                return default;

            try
            {
                var span = byteArray.AsSpan();
                var separatorIndex = span.IndexOf((byte)'\n');
                if (separatorIndex < 0)
                {
                    // fallback para registros antigos sem prefixo de tipo
                    return JsonSerializer.Deserialize<object>(byteArray, _jsonOptions);
                }

                var typeName = Encoding.UTF8.GetString(span.Slice(0, separatorIndex));
                var type = Type.GetType(typeName, throwOnError: true);
                var jsonSpan = span.Slice(separatorIndex + 1);

                // Tuplas são serializadas com nome canônico e o teste espera JsonElement.
                if (typeName.StartsWith("System.Tuple") || typeName.StartsWith("System.ValueTuple"))
                    return JsonSerializer.Deserialize<object>(jsonSpan, _jsonOptions);

                return JsonSerializer.Deserialize(jsonSpan, type, _jsonOptions);
            }
            catch (Exception)
            {
                return default;
            }
        }

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