using Abp;
using Castle.Core.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Authentication.External.OpenIdConnect
{
    /// <summary>
    /// Representa a classe OpenIdConnectAuthProviderApi.
    /// </summary>
    public class OpenIdConnectAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "OpenIdConnect";

        /// <summary>
        /// OpenIdConnectAuthProviderApi.
        /// </summary>
        /// <param name="logger">Parâmetro logger.</param>
        /// <returns>Resultado da operação.</returns>
        public OpenIdConnectAuthProviderApi(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// GetUserInfo.
        /// </summary>
        /// <param name="accessCode">Parâmetro accessCode.</param>
        /// <returns>Resultado da operação.</returns>
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            string additionalParam = this.ProviderInfo.AdditionalParams["Authority"];
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager = !string.IsNullOrEmpty(additionalParam) ? new ConfigurationManager<OpenIdConnectConfiguration>(additionalParam + "/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever()) : throw new ApplicationException("Authentication:OpenId:Issuer configuration is required.");
            ValidateTokenResult validateTokenResult = await ValidateToken(accessCode, additionalParam, configurationManager);

            Claim claim1 = validateTokenResult.Principal.Claims.FirstOrDefault(c => c.Type == "name");
            if (claim1 == null)
                throw new AbpException("name claim is missing !");

            Claim claim2 = validateTokenResult.Principal.Claims.FirstOrDefault(c => c.Type == "unique_name");
            if (claim2 == null)
                throw new AbpException("unique_name claim is missing !");

            string[] strArray = claim1.Value.Split(' ');

            return new ExternalAuthUserInfo()
            {
                Provider = "OpenIdConnect",
                ProviderKey = validateTokenResult.Token.Subject,
                Name = strArray[0],
                Surname = strArray.Length > 1 ? strArray[1] : strArray[0],
                EmailAddress = claim2.Value,
                AccessCode = accessCode,
                Object = null
            };
        }

        private Task<ValidateTokenResult> ValidateToken(string token, string issuer, IConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentNullException(nameof(issuer));

            return ValidateTokenInternal(token, issuer, configurationManager);
        }

        private async Task<ValidateTokenResult> ValidateTokenInternal(string token, string issuer, IConfigurationManager<OpenIdConnectConfiguration> configurationManager, CancellationToken ct = default)
        {
            ICollection<SecurityKey> signingKeys = (await configurationManager.GetConfigurationAsync(ct)).SigningKeys;

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuer = bool.Parse(this.ProviderInfo.AdditionalParams["ValidateIssuer"]),
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5.0),
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            principal.AddMappedClaims(this.ProviderInfo.ClaimMappings);

            Claim claim = principal.Claims.FirstOrDefault(c => c.Type == "aud");
            if (claim == null)
                throw new AbpException("aud claim is missing !");

            if (this.ProviderInfo.ClientId != claim.Value)
                throw new AbpException("ClientId couldn't verified.");

            return new ValidateTokenResult((JwtSecurityToken)validatedToken, principal);
        }

        private sealed class ValidateTokenResult
        {
            /// <summary>
            /// ValidateTokenResult.
            /// </summary>
            /// <returns>Resultado da operação.</returns>
            public ValidateTokenResult()
            {
            }

            /// <summary>
            /// ValidateTokenResult.
            /// </summary>
            /// <param name="token">Parâmetro token.</param>
            /// <param name="principal">Parâmetro principal.</param>
            /// <returns>Resultado da operação.</returns>
            public ValidateTokenResult(JwtSecurityToken token, ClaimsPrincipal principal)
            {
                this.Token = token;
                this.Principal = principal;
            }

            /// <summary>
            /// Obtém ou define Principal.
            /// </summary>
            public ClaimsPrincipal Principal { get; set; }
            /// <summary>
            /// Obtém ou define Token.
            /// </summary>
            public JwtSecurityToken Token { get; set; }
        }
    }
}