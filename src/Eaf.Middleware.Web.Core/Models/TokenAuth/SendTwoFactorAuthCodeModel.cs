using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Representa a classe SendTwoFactorAuthCodeModel.
    /// </summary>
    public class SendTwoFactorAuthCodeModel
    {
        [Required]
        public string Provider { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }
    }
}