using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    /// <summary>
    /// Interface para gerenciamento de chaves e valores no Key Vault.
    /// Fornece operações síncronas e assíncronas para manipulação de segredos.
    /// </summary>
    internal interface IKeyVaultManager
    {
        /// <summary>
        /// Obtém todos os pares chave-valor do Key Vault de forma síncrona.
        /// </summary>
        /// <returns>Dicionário contendo todas as chaves e seus respectivos valores.</returns>
        IDictionary<string, string> GetKeyValues();

        /// <summary>
        /// Obtém o valor de uma chave específica do Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>O valor associado à chave especificada.</returns>
        string GetValue(string key);

        /// <summary>
        /// Obtém todos os pares chave-valor do Key Vault de forma assíncrona.
        /// </summary>
        /// <returns>Task contendo dicionário com todas as chaves e seus respectivos valores.</returns>
        Task<IDictionary<string, string>> GetKeyValuesAsync();

        /// <summary>
        /// Obtém o valor de uma chave específica do Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>Task contendo o valor associado à chave especificada.</returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// Define um valor para uma chave específica no Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser associado à chave.</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Define um valor para uma chave específica no Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser associado à chave.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task SetValueAsync(string key, string value);
    }
}