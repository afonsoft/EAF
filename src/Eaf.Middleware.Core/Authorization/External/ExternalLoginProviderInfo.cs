using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe ExternalLoginProviderInfo.
    /// </summary>
    public class ExternalLoginProviderInfo
    {
        /// <summary>
        /// Inicializa uma nova instância da classe ExternalLoginProviderInfo.
        /// </summary>
        /// <param name="name">Nome do provedor de login externo.</param>
        /// <param name="clientId">ID do cliente OAuth.</param>
        /// <param name="clientSecret">Segredo do cliente OAuth.</param>
        /// <param name="tenantId">ID do tenant.</param>
        /// <param name="providerApiType">Tipo da API do provedor.</param>
        /// <param name="additionalParams">Parâmetros adicionais opcionais.</param>
        /// <param name="claimMappings">Mapeamentos de claims opcionais.</param>
        public ExternalLoginProviderInfo(
          string name,
          string clientId,
          string clientSecret,
          string tenantId,
          Type providerApiType,
          Dictionary<string, string> additionalParams = null,
          List<JsonClaimMap> claimMappings = null)
        {
            this.Name = name;
            this.ClientId = clientId;
            this.TenantId = tenantId;
            this.ClientSecret = clientSecret;
            this.ProviderApiType = providerApiType;
            this.AdditionalParams = additionalParams ?? new Dictionary<string, string>();
            this.ClaimMappings = claimMappings ?? new List<JsonClaimMap>();
        }

        public Dictionary<string, string> AdditionalParams { get; set; }
        public List<JsonClaimMap> ClaimMappings { get; set; }
        /// <summary>
        /// Obtém ou define ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Obtém ou define ClientSecret.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define TenantId.
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// Obtém ou define ProviderApiType.
        /// </summary>
        public Type ProviderApiType { get; set; }
    }
}