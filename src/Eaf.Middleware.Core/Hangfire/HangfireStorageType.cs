namespace Eaf.Hangfire
{
    /// <summary>
    /// Tipo de armazenamento utilizado pelo Hangfire para persistir jobs.
    /// </summary>
    public enum HangfireStorageType
    {
        /// <summary>
        /// Armazenamento em SQL Server (recomendado para produção com SQL Server).
        /// </summary>
        SqlServer,

        /// <summary>
        /// Armazenamento em Redis (recomendado quando o provider de banco não é SQL Server e Redis está habilitado).
        /// </summary>
        Redis,

        /// <summary>
        /// Armazenamento em memória (dados perdidos ao reiniciar a aplicação).
        /// </summary>
        InMemory
    }
}
