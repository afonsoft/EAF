using Eaf.Middleware.Security;

namespace Eaf.Middleware.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Representa a classe GetPasswordComplexitySettingOutput.
    /// </summary>
    public class GetPasswordComplexitySettingOutput
    {
        /// <summary>
        /// Obtém ou define Setting.
        /// </summary>
        public PasswordComplexitySetting Setting { get; set; }
    }
}