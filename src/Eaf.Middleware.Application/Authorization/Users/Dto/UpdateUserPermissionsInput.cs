using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe UpdateUserPermissionsInput.
    /// </summary>
    public class UpdateUserPermissionsInput
    {
        [Required]
        public List<string> GrantedPermissionNames { get; set; }

        [Range(1, int.MaxValue)]
        public long Id { get; set; }
    }
}