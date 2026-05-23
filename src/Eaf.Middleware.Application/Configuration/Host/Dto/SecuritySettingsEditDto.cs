using Eaf.Middleware.Security;

namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe SecuritySettingsEditDto.
    /// </summary>
    public class SecuritySettingsEditDto
    {
        /// <summary>
        /// Obtém ou define AllowOneConcurrentLoginPerUser.
        /// </summary>
        public bool AllowOneConcurrentLoginPerUser { get; set; }

        /// <summary>
        /// Obtém ou define DefaultPasswordComplexity.
        /// </summary>
        public PasswordComplexitySetting DefaultPasswordComplexity { get; set; }
        /// <summary>
        /// Obtém ou define PasswordComplexity.
        /// </summary>
        public PasswordComplexitySetting PasswordComplexity { get; set; }
        /// <summary>
        /// Obtém ou define TwoFactorLogin.
        /// </summary>
        public TwoFactorLoginSettingsEditDto TwoFactorLogin { get; set; }
        /// <summary>
        /// Obtém ou define UseDefaultPasswordComplexitySettings.
        /// </summary>
        public bool UseDefaultPasswordComplexitySettings { get; set; }
        /// <summary>
        /// Obtém ou define UserLockOut.
        /// </summary>
        public UserLockOutSettingsEditDto UserLockOut { get; set; }
    }
}