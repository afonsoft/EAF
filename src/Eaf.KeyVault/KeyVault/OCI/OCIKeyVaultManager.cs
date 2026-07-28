using Abp.Threading;
using Castle.Core.Logging;
using Eaf.Hosting.Configuration;
using System;
using System.Collections.Generic;
using Oci.Common;
using Oci.Common.Auth;
using Oci.SecretsService;
using Oci.SecretsService.Models;
using System.Threading.Tasks;
using Oci.SecretsService.Requests;
using NLog.LayoutRenderers;
using System.Linq;
using System.Net;

namespace Eaf.KeyVault
{
    internal class OCIKeyVaultManager : IKeyVaultManager // NOSONAR
    {
        private readonly EafKeyVaultOptions options;
        private readonly ILogger logger;
        private readonly SecretsClient client;

        /// <summary>
        /// OCIKeyVaultManager.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <param name="logger">Parâmetro logger.</param>
        /// <returns>Resultado da operação.</returns>
        public OCIKeyVaultManager(EafKeyVaultOptions options, ILogger logger)
        {
            this.options = options;
            this.logger = logger;

            Environment.SetEnvironmentVariable("OCI_SDK_DEFAULT_RETRY_ENABLED", "true");

            try
            {
                if (!string.IsNullOrEmpty(options.Oci.UserId) &&
                    !string.IsNullOrEmpty(options.Oci.Region) &&
                    !string.IsNullOrEmpty(options.Oci.TenantId) &&
                    options.Oci.KeySupplier != null)
                {
                    client = new SecretsClient(new SimpleAuthenticationDetailsProvider
                    {
                        TenantId = options.Oci.TenantId,
                        UserId = options.Oci.UserId,
                        Fingerprint = options.Oci.Fingerprint,
                        Region = Region.FromRegionId(options.Oci.Region),
                        PrivateKeySupplier = options.Oci.KeySupplier
                    }, new ClientConfiguration(), options.Endpoint != null ? options.Endpoint.ToString() : null);
                }
                else if (!string.IsNullOrEmpty(options.Oci.ConfigFile))
                    client = new SecretsClient(new ConfigFileAuthenticationDetailsProvider(options.Oci.ConfigFile, options.Oci.Profile ?? "DEFAULT"), new ClientConfiguration(), options.Endpoint != null ? options.Endpoint.ToString() : null);
                else
                    client = new SecretsClient(new ConfigFileAuthenticationDetailsProvider(options.Oci.Profile ?? "DEFAULT"), new ClientConfiguration(), options.Endpoint != null ? options.Endpoint.ToString() : null);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Error on create a client of OCI SecretsClient {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetKeyValues.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public IDictionary<string, string> GetKeyValues()
        {
            return AsyncHelper.RunSync(() => GetKeyValuesAsync());
        }

        /// <summary>
        /// GetKeyValuesAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<IDictionary<string, string>> GetKeyValuesAsync()
        {
            var item = new Dictionary<string, string>();
            try
            {
                var response = await client.ListSecretBundleVersions(new ListSecretBundleVersionsRequest
                {
                    Limit = 100,
                    SecretId = options.Oci.SecretId,
                    SortBy = ListSecretBundleVersionsRequest.SortByEnum.VersionNumber,
                    SortOrder = ListSecretBundleVersionsRequest.SortOrderEnum.Asc
                });

                foreach (var i in response.Items)
                {
                    item.Add(i.VersionNumber.ToString(), await GetValueAsync(i.SecretId));
                }

                return item;
            }
            catch (Exception ex)
            {
                logger.Error("GetKeyValuesAsync", ex);
                return item;
            }
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetValue(string key)
        {
            return AsyncHelper.RunSync(() => GetValueAsync(key));
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
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(options.Oci.VaultId))
                {
                    var response = await client.GetSecretBundleByName(new GetSecretBundleByNameRequest { VaultId = options.Oci.VaultId ?? options.Oci.SecretId, SecretName = key });
                    var secretIdValue = (Base64SecretBundleContentDetails)response.SecretBundle.SecretBundleContent;
                    return Base64Decode(secretIdValue.Content);
                }
                else
                {
                    var response = await client.GetSecretBundle(new GetSecretBundleRequest { SecretId = options.Oci.SecretId });
                    var secretIdValue = (Base64SecretBundleContentDetails)response.SecretBundle.SecretBundleContent;
                    return Base64Decode(secretIdValue.Content);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"GetKeyValues", ex);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// SetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public Task SetValueAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        private static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return base64EncodedData;
            }
        }
    }
}