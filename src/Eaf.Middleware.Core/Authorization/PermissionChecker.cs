using Abp.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;

namespace Eaf.Middleware.Authorization
{
    /// <summary>
    /// Representa a classe PermissionChecker.
    /// </summary>
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        /// <summary>
        /// PermissionChecker.
        /// </summary>
        /// <param name="userManager">Parâmetro userManager.</param>
        /// <returns>Resultado da operação.</returns>
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}