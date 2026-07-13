using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Roles;
using Abp.Organizations;
using System.Linq;
using Abp;


namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Used to perform database operations for <see cref="UserManager"/>.
    /// </summary>
    public class UserStore : AbpUserStore<Role, User>
    {
        /// <summary>
        /// UserStore.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public UserStore(
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserClaim, long> userCliamRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IRepository<UserToken, long> userTokenRepository)
            : base(unitOfWorkManager,
                  userRepository,
                  roleRepository,
                  userRoleRepository,
                  userLoginRepository,
                  userCliamRepository,
            userPermissionSettingRepository,
            userOrganizationUnitRepository,
            organizationUnitRoleRepository,
            userTokenRepository)
        {
        }

        /// <summary>
        /// GetUserById.
        /// </summary>
        /// <param name="id">Parâmetro id.</param>
        /// <returns>Resultado da operação.</returns>
        public User GetUserById(long id)
        {
            var user = UserRepository.GetAll().FirstOrDefault(d => d.Id == id);
            if (user == null)
                throw new AbpException("There is no user: " + id);

            return user;
        }
    }
}