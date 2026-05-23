using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching.Sqlite
{
    internal class DbCommandPool : IDisposable
    {
        /// <summary>
        /// Number of connections to open to the database at startup. Ramps up as concurrency increases.
        /// </summary>
        private const int InitialConcurrency = 4;

        private readonly ConcurrentBag<SqliteCommand>[] _commands = new ConcurrentBag<SqliteCommand>[DbCommands.Count];
        private readonly ConcurrentBag<SqliteConnection> _connections = new ConcurrentBag<SqliteConnection>();
        private readonly string _connectionString;

        /// <summary>
        /// DbCommandPool.
        /// </summary>
        /// <param name="db">Parâmetro db.</param>
        /// <returns>Resultado da operação.</returns>
        public DbCommandPool(SqliteConnection db)
        {
            _connectionString = db.ConnectionString;
            for (int i = 0; i < _commands.Length; ++i)
            {
                _commands[i] = new ConcurrentBag<SqliteCommand>();
            }

            for (int i = 0; i < InitialConcurrency; ++i)
            {
                var connection = new SqliteConnection(_connectionString);
                connection.Open();
                _connections.Add(connection);
            }
        }

        /// <summary>
        /// Use.
        /// </summary>
        /// <param name="type">Parâmetro type.</param>
        /// <param name="handler">Parâmetro handler.</param>
        public void Use(Operation type, Action<SqliteCommand> handler)
        {
            Use<bool>(type, (cmd) =>
            {
                handler(cmd);
                return true;
            });
        }

        public R Use<R>(Operation type, Func<SqliteCommand, R> handler)
        {
            if (!_connections.TryTake(out var db))
            {
                db = new SqliteConnection(_connectionString);
                db.Open();
            }

            var pool = _commands[(int)type];
            if (!pool.TryTake(out var command))
            {
                command = new SqliteCommand(DbCommands.Commands[(int)type], db);
            }

            try
            {
                command.Connection = db;
                return handler(command);
            }
            finally
            {
                command.Parameters.Clear();
                pool.Add(command);
                _connections.Add(db);
            }
        }

        public async Task<R> UseAsync<R>(Operation type, Func<SqliteCommand, Task<R>> handler)
        {
            if (!_connections.TryTake(out var db))
            {
                db = new SqliteConnection(_connectionString);
                await db.OpenAsync();
            }

            var pool = _commands[(int)type];
            if (!pool.TryTake(out var command))
            {
                command = new SqliteCommand(DbCommands.Commands[(int)type], db);
            }

            try
            {
                return await handler(command);
            }
            finally
            {
                command.Parameters.Clear();
                pool.Add(command);
                _connections.Add(db);
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (var pool in _commands)
            {
                while (pool.TryTake(out var cmd))
                {
                    cmd.Dispose();
                }
            }

            foreach (var conn in _connections)
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}