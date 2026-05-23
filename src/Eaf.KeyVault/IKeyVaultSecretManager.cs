using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    /// <summary>
    /// Interface pública para gerenciamento de segredos do Key Vault.
    /// Fornece acesso seguro a chaves e valores armazenados em provedores de Key Vault como Azure e OCI.
    /// </summary>
    public interface IKeyVaultSecretManager
    {
        /// <summary>
        /// Obtém todos os pares chave-valor disponíveis no Key Vault de forma síncrona.
        /// </summary>
        /// <returns>Dicionário contendo todas as chaves e seus respectivos valores descriptografados.</returns>
        IDictionary<string, string> GetKeyValues();

        /// <summary>
        /// Obtém o valor descriptografado de uma chave específica do Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>O valor descriptografado associado à chave especificada, ou null se não encontrado.</returns>
        string GetValue(string key);

        /// <summary>
        /// Obtém todos os pares chave-valor disponíveis no Key Vault de forma assíncrona.
        /// </summary>
        /// <returns>Task contendo dicionário com todas as chaves e seus respectivos valores descriptografados.</returns>
        Task<IDictionary<string, string>> GetKeyValuesAsync();

        /// <summary>
        /// Obtém o valor descriptografado de uma chave específica do Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>Task contendo o valor descriptografado associado à chave especificada, ou null se não encontrado.</returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// Define um valor criptografado para uma chave específica no Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser criptografado e associado à chave.</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Define um valor criptografado para uma chave específica no Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser criptografado e associado à chave.</param>
        /// <returns>Task representando a operação assíncrona de criptografia e armazenamento.</returns>
        Task SetValueAsync(string key, string value);
    }
}