using Eaf.Middleware.Authorization.Permissions.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe GetUserPermissionsForEditOutput.
    /// </summary>
    public class GetUserPermissionsForEditOutput
    {
        public List<string> GrantedPermissionNames { get; set; }
        public List<FlatPermissionDto> Permissions { get; set; }
    }
}