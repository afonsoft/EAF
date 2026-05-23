using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    internal class AzureKeyVaultManager : IKeyVaultManager
    {
        private readonly ILogger logger;
        private readonly SecretClient client;

        /// <summary>
        /// AzureKeyVaultManager.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <param name="logger">Parâmetro logger.</param>
        /// <returns>Resultado da operação.</returns>
        public AzureKeyVaultManager(EafKeyVaultOptions options, ILogger logger)
        {
            this.logger = logger;

            try
            {
                if (options.Azure.Certificate != null && !string.IsNullOrEmpty(options.Azure.ApplicationId) && !string.IsNullOrEmpty(options.Azure.TenantId))
                    client = new SecretClient(options.Endpoint, new ClientCertificateCredential(options.Azure.TenantId, options.Azure.ApplicationId, options.Azure.Certificate));
                else if (options.Azure.Certificate == null && !string.IsNullOrEmpty(options.Azure.ClientSecret) && !string.IsNullOrEmpty(options.Azure.ApplicationId) && !string.IsNullOrEmpty(options.Azure.TenantId))
                    client = new SecretClient(options.Endpoint, new ClientSecretCredential(options.Azure.TenantId, options.Azure.ApplicationId, options.Azure.ClientSecret));
                else
                    client = new SecretClient(options.Endpoint, new DefaultAzureCredential());
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Error on create a client of Azure SecretsClient {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetKeyValues.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public IDictionary<string, string> GetKeyValues()
        {
            try
            {
                var itens = new Dictionary<string, string>();
                var prop = client.GetPropertiesOfSecrets();

                foreach (var p in prop)
                {
                    if (p.Enabled.Value)
                        itens.Add(p.Name, GetValue(p.Name));
                }

                return itens;
            }
            catch (Exception ex)
            {
                logger.Error("GetKeyValues", ex);
                throw;
            }
        }

        /// <summary>
        /// GetKeyValuesAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<IDictionary<string, string>> GetKeyValuesAsync()
        {
            try
            {
                var itens = new Dictionary<string, string>();
                var prop = client.GetPropertiesOfSecrets();

                foreach (var p in prop)
                {
                    if (p.Enabled.Value)
                        itens.Add(p.Name, await GetValueAsync(p.Name));
                }

                return itens;
            }
            catch (Exception ex)
            {
                logger.Error("GetKeyValues", ex);
                throw;
            }
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetValue(string key)
        {
            try
            {
                var keyVault = client.GetSecret(key);
                return keyVault.Value.Value;
            }
            catch (Exception ex)
            {
                logger.Error($"GetValue key {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// GetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<string> GetValueAsync(string key)
        {
            try
            {
                var keyVault = await client.GetSecretAsync(key);
                return keyVault.Value.Value;
            }
            catch (Exception ex)
            {
                logger.Error($"GetValue key {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// SetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public void SetValue(string key, string value)
        {
            try
            {
                client.SetSecret(key, value);
            }
            catch (Exception ex)
            {
                logger.Error($"SetVaule key {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// SetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public async Task SetValueAsync(string key, string value)
        {
            try
            {
                await client.SetSecretAsync(key, value);
            }
            catch (Exception ex)
            {
                logger.Error($"SetVauleAsync key {key}", ex);
                throw;
            }
        }
    }
}