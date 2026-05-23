using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe CreateLdapUserInput.
    /// </summary>
    public class CreateLdapUserInput
    {
        [Required]
        public string[] AssignedRoleNames { get; set; }

        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        [Required]
        public string[] UserNames { get; set; }
    }
}