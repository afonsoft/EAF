using Abp.Extensions;
using Abp.UI;
using System;

namespace Eaf.Middleware.Core.Authentication
{
    /// <summary>
    /// Representa a classe OpenIdConnectExternalLoginProviderSettings.
    /// </summary>
    public class OpenIdConnectExternalLoginProviderSettings : IExternalLoginProviderSettings
    {
        /// <summary>
        /// Obtém ou define Authority.
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// Obtém ou define ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Obtém ou define ClientSecret.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Obtém ou define LoginUrl.
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// Obtém ou define ValidateIssuer.
        /// </summary>
        public bool ValidateIssuer { get; set; }

        /// <summary>
        /// IsValid.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public bool IsValid()
        {
            bool valid = !ClientId.IsNullOrWhiteSpace() ||
                         !Authority.IsNullOrWhiteSpace();

            if (valid && !Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("Property name \"Authority\" must start with \"https://\"");
            }

            return valid;
        }
    }
}