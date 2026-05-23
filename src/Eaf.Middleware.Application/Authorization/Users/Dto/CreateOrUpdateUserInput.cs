using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe CreateOrUpdateUserInput.
    /// </summary>
    public class CreateOrUpdateUserInput
    {
        [Required]
        public string[] AssignedRoleNames { get; set; }

        /// <summary>
        /// Obtém ou define SendActivationEmail.
        /// </summary>
        public bool SendActivationEmail { get; set; }

        /// <summary>
        /// Obtém ou define SetRandomPassword.
        /// </summary>
        public bool SetRandomPassword { get; set; }

        [Required]
        public UserEditDto User { get; set; }
    }
}