namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe TwoFactorLoginSettingsEditDto.
    /// </summary>
    public class TwoFactorLoginSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define IsEmailProviderEnabled.
        /// </summary>
        public bool IsEmailProviderEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsEnabled.
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsEnabledForApplication.
        /// </summary>
        public bool IsEnabledForApplication { get; set; }
        /// <summary>
        /// Obtém ou define IsGoogleAuthenticatorEnabled.
        /// </summary>
        public bool IsGoogleAuthenticatorEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsRememberBrowserEnabled.
        /// </summary>
        public bool IsRememberBrowserEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsSmsProviderEnabled.
        /// </summary>
        public bool IsSmsProviderEnabled { get; set; }
    }
}