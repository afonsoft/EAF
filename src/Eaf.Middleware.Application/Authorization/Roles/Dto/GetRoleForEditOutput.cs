using Eaf.Middleware.Authorization.Permissions.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Authorization.Roles.Dto
{
    /// <summary>
    /// Representa a classe GetRoleForEditOutput.
    /// </summary>
    public class GetRoleForEditOutput
    {
        public List<string> GrantedPermissionNames { get; set; }
        public List<FlatPermissionDto> Permissions { get; set; }
        /// <summary>
        /// Obtém ou define Role.
        /// </summary>
        public RoleEditDto Role { get; set; }
    }
}