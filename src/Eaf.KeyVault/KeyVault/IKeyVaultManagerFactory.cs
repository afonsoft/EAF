namespace Eaf.KeyVault
{
    /// <summary>
    /// Factory para criar instâncias de IKeyVaultManager baseado na configuração.
    /// </summary>
    internal interface IKeyVaultManagerFactory
    {
        /// <summary>
        /// Cria uma instância de IKeyVaultManager baseada nas opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do KeyVault.</param>
        /// <returns>Instância de IKeyVaultManager.</returns>
        IKeyVaultManager Create(EafKeyVaultOptions options);
    }
}
