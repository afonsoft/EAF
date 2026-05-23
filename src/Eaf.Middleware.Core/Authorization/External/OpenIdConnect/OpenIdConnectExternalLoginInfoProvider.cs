using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.External.OpenIdConnect
{
    /// <summary>
    /// Representa a classe OpenIdConnectExternalLoginInfoProvider.
    /// </summary>
    public class OpenIdConnectExternalLoginInfoProvider : IExternalLoginInfoProvider
    {
        /// <summary>
        /// OpenIdConnectExternalLoginInfoProvider.
        /// </summary>
        /// <param name="clientId">Parâmetro clientId.</param>
        /// <param name="clientSecret">Parâmetro clientSecret.</param>
        /// <param name="authority">Parâmetro authority.</param>
        /// <param name="loginUrl">Parâmetro loginUrl.</param>
        /// <param name="validateIssuer">Parâmetro validateIssuer.</param>
        /// <param name="jsonClaimMaps">Parâmetro jsonClaimMaps.</param>
        /// <returns>Resultado da operação.</returns>
        public OpenIdConnectExternalLoginInfoProvider(
          string clientId,
          string clientSecret,
          string authority,
          string loginUrl,
          bool validateIssuer,
          List<JsonClaimMap> jsonClaimMaps)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.Authority = authority;
            this.LoginUrl = loginUrl;
            this.ValidateIssuer = validateIssuer;
            this.JsonClaimMaps = jsonClaimMaps;
            this.CreateExternalLoginProviderInfo();
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; } = "OpenIdConnect";

        protected string Authority { get; set; }
        protected string ClientId { get; set; }

        protected string ClientSecret { get; set; }
        protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }
        protected List<JsonClaimMap> JsonClaimMaps { get; set; }
        protected string LoginUrl { get; set; }

        protected bool ValidateIssuer { get; set; }

        /// <summary>
        /// GetExternalLoginInfo.
        /// </summary>
        public virtual ExternalLoginProviderInfo GetExternalLoginInfo() => this.ExternalLoginProviderInfo;

        private void CreateExternalLoginProviderInfo()
        {
            string clientId = this.ClientId;
            string clientSecret = this.ClientSecret;
            Type providerApiType = typeof(OpenIdConnectAuthProviderApi);
            Dictionary<string, string> additionalParams = new Dictionary<string, string>();
            additionalParams.Add("Authority", this.Authority);
            additionalParams.Add("LoginUrl", this.LoginUrl);
            additionalParams.Add("ValidateIssuer", this.ValidateIssuer.ToString());
            List<JsonClaimMap> jsonClaimMaps = this.JsonClaimMaps;
            this.ExternalLoginProviderInfo = new ExternalLoginProviderInfo("OpenIdConnect", clientId, clientSecret, null, providerApiType, additionalParams, jsonClaimMaps);
        }
    }
}