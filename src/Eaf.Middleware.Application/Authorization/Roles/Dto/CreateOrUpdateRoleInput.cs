using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Roles.Dto
{
    /// <summary>
    /// Representa a classe CreateOrUpdateRoleInput.
    /// </summary>
    public class CreateOrUpdateRoleInput
    {
        [Required]
        public List<string> GrantedPermissionNames { get; set; }

        [Required]
        public RoleEditDto Role { get; set; }
    }
}