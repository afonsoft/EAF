using System.Collections.Generic;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe AuthenticateResultModel.
    /// </summary>
    public class AuthenticateResultModel
    {
        /// <summary>
        /// Obtém ou define AccessToken.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Obtém ou define EncryptedAccessToken.
        /// </summary>
        public string EncryptedAccessToken { get; set; }

        /// <summary>
        /// Obtém ou define ExpireInSeconds.
        /// </summary>
        public int ExpireInSeconds { get; set; }

        /// <summary>
        /// Obtém ou define PasswordResetCode.
        /// </summary>
        public string PasswordResetCode { get; set; }
        /// <summary>
        /// Obtém ou define RequiresTwoFactorVerification.
        /// </summary>
        public bool RequiresTwoFactorVerification { get; set; }
        /// <summary>
        /// Obtém ou define ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// Obtém ou define ShouldResetPassword.
        /// </summary>
        public bool ShouldResetPassword { get; set; }
        public IList<string> TwoFactorAuthProviders { get; set; }
        /// <summary>
        /// Obtém ou define TwoFactorRememberClientToken.
        /// </summary>
        public string TwoFactorRememberClientToken { get; set; }
        /// <summary>
        /// Obtém ou define UserId.
        /// </summary>
        public long UserId { get; set; }
    }
}