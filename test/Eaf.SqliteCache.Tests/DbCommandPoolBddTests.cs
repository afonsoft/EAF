using Abp.Runtime.Caching.Sqlite;
using Microsoft.Data.Sqlite;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    public class DbCommandPoolBddTests
    {
        private static SqliteConnection CriarConexaoInicializada()
        {
            var options = new EafSqliteCacheOptions { MemoryOnly = true };
            var connection = new SqliteConnection(options.ConnectionString);
            connection.Open();

            using var cmd = new SqliteCommand(EafSqliteCache.TableInitCommand, connection);
            cmd.ExecuteNonQuery();

            return connection;
        }

        [Fact]
        public void Dado_PoolInicializado_Quando_ExecutarUseGet_Entao_DeveRetornarNuloSemErro()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            var result = pool.Use(Operation.Get, cmd =>
            {
                cmd.Parameters.AddWithValue("@key", "chave-inexistente");
                cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                return cmd.ExecuteScalar();
            });

            result.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_PoolInicializado_Quando_ExecutarUseAsyncGet_Entao_DeveRetornarNuloSemErro()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            var result = await pool.UseAsync(Operation.Get, async cmd =>
            {
                cmd.Parameters.AddWithValue("@key", "chave-inexistente");
                cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                await Task.Yield();
                return cmd.ExecuteScalar();
            });

            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_PoolComComandosReutilizados_Quando_ExecutarMultiplasOperacoes_Entao_DeveManterConexao()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            pool.Use(Operation.RemoveAll, cmd => cmd.ExecuteNonQuery());
            pool.Use(Operation.Insert, cmd =>
            {
                cmd.Parameters.AddWithValue("@key", "chave1");
                cmd.Parameters.AddWithValue("@value", new byte[] { 1, 2, 3 });
                cmd.Parameters.AddWithValue("@expiry", DBNull.Value);
                cmd.Parameters.AddWithValue("@renewal", DBNull.Value);
                return cmd.ExecuteNonQuery();
            });

            var result = pool.Use(Operation.Get, cmd =>
            {
                cmd.Parameters.AddWithValue("@key", "chave1");
                cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                return cmd.ExecuteScalar();
            });

            result.ShouldBeOfType<byte[]>();
        }

        [Fact]
        public void Dado_PoolComConexoesExauridas_Quando_CriarNovaConexao_Entao_DeveFuncionarSemErro()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            for (int i = 0; i < 10; i++)
            {
                pool.Use(Operation.Get, cmd =>
                {
                    cmd.Parameters.AddWithValue("@key", $"chave-{i}");
                    cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                    return cmd.ExecuteScalar();
                });
            }

            pool.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_PoolInicializado_Quando_Dispose_Entao_DeveLiberarRecursosSemErro()
        {
            using var connection = CriarConexaoInicializada();
            var pool = new DbCommandPool(connection);

            Should.NotThrow(() => pool.Dispose());
            Should.NotThrow(() => pool.Dispose());
        }

        [Fact]
        public void Dado_PoolComMultiplasThreads_Quando_Use_Entao_DeveCriarNovaConexaoQuandoNecessario()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            Parallel.For(0, 10, options, i =>
            {
                pool.Use(Operation.Get, cmd =>
                {
                    cmd.Parameters.AddWithValue("@key", $"chave-{i}");
                    cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                    Task.Delay(100).GetAwaiter().GetResult();
                    return cmd.ExecuteScalar();
                });
            });

            pool.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_PoolComMultiplasTasks_Quando_UseAsync_Entao_DeveCriarNovaConexaoQuandoNecessario()
        {
            using var connection = CriarConexaoInicializada();
            using var pool = new DbCommandPool(connection);

            var tasks = Enumerable.Range(0, 10).Select(i => pool.UseAsync(Operation.Get, async cmd =>
            {
                cmd.Parameters.AddWithValue("@key", $"chave-{i}");
                cmd.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.Ticks);
                await Task.Delay(100);
                return cmd.ExecuteScalar();
            })).ToArray();

            await Task.WhenAll(tasks);

            pool.ShouldNotBeNull();
        }
    }
}
