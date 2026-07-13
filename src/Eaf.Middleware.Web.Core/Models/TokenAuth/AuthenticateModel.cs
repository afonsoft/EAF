using Abp.Auditing;
using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe AuthenticateModel.
    /// </summary>
    public class AuthenticateModel
    {
        [DisableAuditing]
        public string CaptchaResponse { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        /// <summary>
        /// Obtém ou define RememberClient.
        /// </summary>
        public bool RememberClient { get; set; } // NOSONAR

        /// <summary>
        /// Obtém ou define ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }

        public bool? SingleSignIn { get; set; }

        /// <summary>
        /// Obtém ou define TwoFactorRememberClientToken.
        /// </summary>
        public string TwoFactorRememberClientToken { get; set; }

        /// <summary>
        /// Obtém ou define TwoFactorVerificationCode.
        /// </summary>
        public string TwoFactorVerificationCode { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }
    }
}