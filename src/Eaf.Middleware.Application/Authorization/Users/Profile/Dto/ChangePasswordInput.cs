using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Representa a classe ChangePasswordInput.
    /// </summary>
    public class ChangePasswordInput
    {
        [Required]
        [DisableAuditing]
        public string CurrentPassword { get; set; }

        [Required]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}