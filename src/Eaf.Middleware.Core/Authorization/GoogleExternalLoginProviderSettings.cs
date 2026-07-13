using Abp.Extensions;
using System;

namespace Eaf.Middleware.Core.Authentication
{
    /// <summary>
    /// Representa a classe GoogleExternalLoginProviderSettings.
    /// </summary>
    public class GoogleExternalLoginProviderSettings : IExternalLoginProviderSettings
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
        /// Obtém ou define UserInfoEndpoint.
        /// </summary>
        public string UserInfoEndpoint { get; set; }

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