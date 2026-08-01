namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa o modo de seleção de tenant durante o registro público.
    /// </summary>
    public enum TenantSelectionMode
    {
        /// <summary>
        /// Usa o tenant padrão do sistema.
        /// </summary>
        DefaultTenant,

        /// <summary>
        /// Cria um novo tenant.
        /// </summary>
        CreateNew,

        /// <summary>
        /// Solicita ingresso em um tenant existente.
        /// </summary>
        JoinExisting
    }
}
