using Abp.Authorization.Roles;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Users;

namespace Eaf.Middleware.Authorization.Roles
{
    /// <summary>
    /// Representa a classe RoleStore.
    /// </summary>
    public class RoleStore : AbpRoleStore<Role, User>
    {
        /// <summary>
        /// RoleStore.
        /// </summary>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <param name="roleRepository">Parâmetro roleRepository.</param>
        /// <param name="rolePermissionSettingRepository">Parâmetro rolePermissionSettingRepository.</param>
        /// <returns>Resultado da operação.</returns>
        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}