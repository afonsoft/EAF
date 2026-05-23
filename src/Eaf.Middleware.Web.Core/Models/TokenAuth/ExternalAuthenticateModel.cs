using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe ExternalAuthenticateModel.
    /// </summary>
    public class ExternalAuthenticateModel
    {
        [Required]
        [MaxLength(UserLogin.MaxLoginProviderLength)]
        public string AuthProvider { get; set; }

        [Required]
        public string ProviderAccessCode { get; set; }

        [Required]
        [MaxLength(UserLogin.MaxProviderKeyLength)]
        public string ProviderKey { get; set; }

        /// <summary>
        /// Obtém ou define ReturnUrl.
        /// </summary>
        public string ReturnUrl { get; set; }

        public bool? SingleSignIn { get; set; }
    }
}