using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System;

namespace Abp.Runtime.Caching.Sqlite
{
    /// <summary>
    /// Representa a classe EafSqliteCacheOptions.
    /// </summary>
    public class EafSqliteCacheOptions : IOptions<EafSqliteCacheOptions>
    {
        EafSqliteCacheOptions IOptions<EafSqliteCacheOptions>.Value => this;

        /// <summary>
        /// Takes precedence over <see cref="CachePath"/>
        /// </summary>
        public bool MemoryOnly { get; set; } = false;

        private string _cachePath = "SqliteCache.db";

        /// <summary>
        /// Only if <see cref="MemoryOnly"/> is <c>false</c> />
        /// </summary>
        public string CachePath
        {
            get => _cachePath;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (value.StartsWith("Data Source="))
                {
                    value = value.Substring("Data Source=".Length);
                }
                value = value.Trim();

                if (value.Contains('=') && !value.StartsWith("./") && !value.StartsWith("../") && !value.StartsWith('/') && !value.Contains(':') && !value.EndsWith(".db"))
                {
                    throw new ArgumentException("CachePath must be a path and not a connection string!");
                }
                _cachePath = value;
            }
        }

        /// <summary>
        /// Specifies how often expired items are removed in the background.
        /// Background eviction is disabled if set to <c>null</c>.
        /// </summary>
        public TimeSpan? CleanupInterval { get; set; } = TimeSpan.FromMinutes(30);

        internal string ConnectionString
        {
            get
            {
                var sb = new SqliteConnectionStringBuilder();
                sb.DataSource = MemoryOnly
                    ? ":memory:" : CachePath;
                sb.Mode = MemoryOnly
                    ? SqliteOpenMode.Memory : SqliteOpenMode.ReadWriteCreate;
                sb.Cache = SqliteCacheMode.Shared;
                sb.Pooling = false;

                return sb.ConnectionString;
            }
        }
    }
}