using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe CreateActiveDirectoryUserInput.
    /// </summary>
    public class CreateActiveDirectoryUserInput
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