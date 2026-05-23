using Eaf.Middleware.Core.Authentication;
using System.Collections.Generic;

namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe ExternalLoginProviderSettingsEditDto.
    /// </summary>
    public class ExternalLoginProviderSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define Google.
        /// </summary>
        public GoogleExternalLoginProviderSettings Google { get; set; }
        /// <summary>
        /// Obtém ou define Google_IsEnabled.
        /// </summary>
        public bool Google_IsEnabled { get; set; }
        /// <summary>
        /// Obtém ou define Microsoft.
        /// </summary>
        public MicrosoftExternalLoginProviderSettings Microsoft { get; set; }
        /// <summary>
        /// Obtém ou define Microsoft_IsEnabled.
        /// </summary>
        public bool Microsoft_IsEnabled { get; set; }
        /// <summary>
        /// Obtém ou define OpenIdConnect.
        /// </summary>
        public OpenIdConnectExternalLoginProviderSettings OpenIdConnect { get; set; }
        /// <summary>
        /// Obtém ou define OpenIdConnect_IsEnabled.
        /// </summary>
        public bool OpenIdConnect_IsEnabled { get; set; }
        public List<JsonClaimMapDto> OpenIdConnectClaimsMapping { get; set; }
        /// <summary>
        /// Obtém ou define AuthZero.
        /// </summary>
        public AuthZeroExternalLoginProviderSettings AuthZero { get; set; }
        /// <summary>
        /// Obtém ou define AuthZero_IsEnabled.
        /// </summary>
        public bool AuthZero_IsEnabled { get; set; }
    }
}