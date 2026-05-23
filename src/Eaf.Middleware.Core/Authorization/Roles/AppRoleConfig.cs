using Abp.MultiTenancy;
using Abp.Zero.Configuration;

namespace Eaf.Middleware.Authorization.Roles
{
    /// <summary>
    /// Representa a classe AppRoleConfig.
    /// </summary>
    public static class AppRoleConfig
    {
        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="roleManagementConfig">Parâmetro roleManagementConfig.</param>
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            //Static host roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.Admin,
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: true)
                );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.User,
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: false)
                );

            //Static tenant roles
            roleManagementConfig.StaticRoles.Add(
               new StaticRoleDefinition(
                   StaticRoleNames.Tenants.Admin,
                   MultiTenancySides.Tenant,
                   grantAllPermissionsByDefault: true)
               );

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.User,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: false)
                );
        }
    }
}