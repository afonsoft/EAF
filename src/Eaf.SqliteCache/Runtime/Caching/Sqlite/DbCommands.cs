using System;

namespace Abp.Runtime.Caching.Sqlite
{
    /// <summary>
    /// Enumeração das operações disponíveis para o cache SQLite.
    /// Define os tipos de comandos SQL que podem ser executados no banco de dados de cache.
    /// </summary>
    internal enum Operation
    {
        /// <summary>Operação de inserção de novo item no cache.</summary>
        Insert,
        /// <summary>Operação de remoção de item específico do cache.</summary>
        Remove,
        /// <summary>Operação de remoção de itens expirados do cache.</summary>
        RemoveExpired,
        /// <summary>Operação de obtenção de item do cache.</summary>
        Get,
        /// <summary>Operação de renovação de expiração de item do cache.</summary>
        Refresh,
        /// <summary>Operação de inserção em lote de múltiplos itens.</summary>
        BulkInsert,
        /// <summary>Operação de remoção de todos os itens do cache.</summary>
        RemoveAll,
    }

    /// <summary>
    /// Classe estática que contém os comandos SQL pré-compilados para operações de cache SQLite.
    /// Fornece comandos otimizados para todas as operações de cache suportadas.
    /// </summary>
    internal static class DbCommands
    {
        /// <summary>
        /// Número total de operações disponíveis no enum Operation.
        /// </summary>
        public static readonly int Count = Enum.GetValues(typeof(Operation)).Length;

        /// <summary>
        /// Array de comandos SQL indexados por operação.
        /// Cada posição corresponde ao valor numérico do enum Operation.
        /// </summary>
        public static readonly string[] Commands = InitDbCommands();

        /// <summary>
        /// Cláusula SQL para verificar se um item não está expirado.
        /// Considera dois campos de expiração: AbsoluteExpiry e (NextExpiry, Ttl).
        /// </summary>
        private const string NotExpiredClause = " (expiry IS NULL OR expiry >= @now) ";

        /// <summary>
        /// Inicializa o array de comandos SQL para todas as operações de cache.
        /// Constrói comandos otimizados para inserção, remoção, obtenção e renovação de itens.
        /// </summary>
        /// <returns>Array de strings contendo os comandos SQL indexados por operação.</returns>
        private static string[] InitDbCommands()
        {
            var cmd = new string[Count];

            cmd[(int)Operation.Insert] =
                "INSERT OR REPLACE INTO cache (key, value, expiry, renewal) " +
                "VALUES (@key, @value, @expiry, @renewal)";

            cmd[(int)Operation.Refresh] =
                $"UPDATE cache " +
                $"SET expiry = (@now + renewal) " +
                $"WHERE " +
                $"  key = @key " +
                $"  AND expiry >= @now " +
                $"  AND renewal IS NOT NULL;";

            cmd[(int)Operation.Get] =
                // Get an unexpired item from the cache
                $"SELECT value FROM cache " +
                $"  WHERE key = @key " +
                $"  AND {NotExpiredClause};" +
                // And update the expiry if it is unexpired and has a renewal
                cmd[(int)Operation.Refresh];

            cmd[(int)Operation.Remove] =
                "DELETE FROM cache " +
                "  WHERE key = @key";

            cmd[(int)Operation.RemoveAll] =
                "DELETE FROM cache ";

            cmd[(int)Operation.RemoveExpired] =
                "DELETE FROM cache " +
                $"  WHERE NOT {NotExpiredClause};" +
                $"SELECT CHANGES();";

            cmd[(int)Operation.BulkInsert] =
                "INSERT OR REPLACE INTO cache (key, value, expiry, renewal) VALUES ";

            return cmd;
        }
    }
}