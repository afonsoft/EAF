using Abp.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;

namespace Eaf.Middleware.Authorization.Roles
{
    /// <summary>
    /// Represents a role in the system.
    /// </summary>
    public class Role : AbpRole<User>
    {
        //Can add application specific role properties here

        /// <summary>
        /// Role.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public Role()
        {
        }

        /// <summary>
        /// Role.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="displayName">Parâmetro displayName.</param>
        /// <returns>Resultado da operação.</returns>
        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        /// <summary>
        /// Role.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="name">Parâmetro name.</param>
        /// <param name="displayName">Parâmetro displayName.</param>
        /// <returns>Resultado da operação.</returns>
        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }
    }
}