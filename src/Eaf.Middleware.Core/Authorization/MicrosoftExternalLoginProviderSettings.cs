using Abp.Extensions;
using System;

namespace Eaf.Middleware.Core.Authentication
{
    /// <summary>
    /// Representa a classe MicrosoftExternalLoginProviderSettings.
    /// </summary>
    public class MicrosoftExternalLoginProviderSettings
    {
        /// <summary>
        /// Obtém ou define ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Obtém ou define ClientSecret.
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Obtém ou define TenantId.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// IsValid.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public bool IsValid()
        {
            return !ClientId.IsNullOrWhiteSpace() && !ClientSecret.IsNullOrWhiteSpace();
        }
    }
}