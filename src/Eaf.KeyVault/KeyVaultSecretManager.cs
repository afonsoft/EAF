using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    /// <summary>
    /// Gerenciador de segredos do Key Vault que fornece uma interface unificada para acessar segredos
    /// de diferentes provedores (Azure Key Vault, Oracle Cloud Infrastructure, ou implementação nula).
    /// </summary>
    public class KeyVaultSecretManager : IKeyVaultSecretManager
    {
        private readonly IKeyVaultManager manager;

        /// <summary>
        /// Logger para registrar operações e erros do gerenciador de segredos.
        /// </summary>
        public ILogger Logger { get; set; }
        private readonly EafKeyVaultOptions options = new EafKeyVaultOptions();

        /// <summary>
        /// Inicializa uma nova instância do KeyVaultSecretManager com as opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do Key Vault encapsuladas em IOptions.</param>
        public KeyVaultSecretManager(IOptions<EafKeyVaultOptions> options) : this(options.Value)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância do KeyVaultSecretManager com as opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do Key Vault.</param>
        /// <param name="loggerFactory">Factory de logger (opcional, injetado via DI).</param>
        /// <param name="managerFactory">Factory para criar o manager (opcional, injetado via DI).</param>
        public KeyVaultSecretManager(EafKeyVaultOptions options, ILoggerFactory loggerFactory = null, IKeyVaultManagerFactory managerFactory = null)
        {
            Logger = loggerFactory?.Create(typeof(KeyVaultSecretManager)) ?? NullLogger.Instance;

            if (options != null)
                this.options = options.Value;

            try
            {
                var factory = managerFactory ?? new KeyVaultManagerFactory(Logger);
                manager = factory.Create(this.options);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "KeyVaultSecretManager {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtém todos os pares chave-valor do Key Vault de forma síncrona.
        /// </summary>
        /// <returns>Dicionário contendo todos os pares chave-valor disponíveis.</returns>
        public IDictionary<string, string> GetKeyValues()
        {
            return manager.GetKeyValues();
        }

        /// <summary>
        /// Obtém todos os pares chave-valor do Key Vault de forma assíncrona.
        /// </summary>
        /// <returns>Task contendo um dicionário com todos os pares chave-valor disponíveis.</returns>
        public Task<IDictionary<string, string>> GetKeyValuesAsync()
        {
            return manager.GetKeyValuesAsync();
        }

        /// <summary>
        /// Obtém o valor de uma chave específica do Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>O valor associado à chave, ou null se a chave não for encontrada.</returns>
        public string GetValue(string key)
        {
            return manager.GetValue(key);
        }

        /// <summary>
        /// Obtém o valor de uma chave específica do Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser recuperado.</param>
        /// <returns>Task contendo o valor associado à chave, ou null se a chave não for encontrada.</returns>
        public Task<string> GetValueAsync(string key)
        {
            return manager.GetValueAsync(key);
        }

        /// <summary>
        /// Define um valor para uma chave específica no Key Vault de forma síncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser associado à chave.</param>
        public void SetValue(string key, string value)
        {
            manager.SetValue(key, value);
        }

        /// <summary>
        /// Define um valor para uma chave específica no Key Vault de forma assíncrona.
        /// </summary>
        /// <param name="key">A chave do segredo a ser definido.</param>
        /// <param name="value">O valor a ser associado à chave.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public Task SetValueAsync(string key, string value)
        {
            return manager.SetValueAsync(key, value);
        }
    }
}
