namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe HostUserManagementSettingsEditDto.
    /// </summary>
    public class HostUserManagementSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define AllowOneConcurrentLoginPerUser.
        /// </summary>
        public bool AllowOneConcurrentLoginPerUser { get; set; }
        /// <summary>
        /// Obtém ou define IsCookieConsentEnabled.
        /// </summary>
        public bool IsCookieConsentEnabled { get; set; }
        /// <summary>
        /// Obtém ou define IsEmailConfirmationRequiredForLogin.
        /// </summary>
        public bool IsEmailConfirmationRequiredForLogin { get; set; }

        /// <summary>
        /// Obtém ou define IsRegisterRequiredForLogin.
        /// </summary>
        public bool IsRegisterRequiredForLogin { get; set; }
        /// <summary>
        /// Obtém ou define StoreExternalTokenInformation.
        /// </summary>
        public bool StoreExternalTokenInformation { get; set; }

        /// <summary>
        /// Obtém ou define TokenExpiration.
        /// </summary>
        public int TokenExpiration { get; set; }

        /// <summary>
        /// Obtém ou define UseCaptchaOnLogin.
        /// </summary>
        public bool UseCaptchaOnLogin { get; set; }
    }
}