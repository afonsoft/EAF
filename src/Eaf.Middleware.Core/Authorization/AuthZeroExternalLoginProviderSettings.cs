using Abp.Extensions;
using System;

namespace Eaf.Middleware.Core.Authentication
{
    /// <summary>
    /// Representa a classe AuthZeroExternalLoginProviderSettings.
    /// </summary>
    public class AuthZeroExternalLoginProviderSettings
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
        /// Obtém ou define Endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// IsValid.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public bool IsValid()
        {
            return !ClientId.IsNullOrWhiteSpace() && !ClientSecret.IsNullOrWhiteSpace() && !Endpoint.IsNullOrWhiteSpace();
        }
    }
}