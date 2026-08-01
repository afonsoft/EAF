using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Roles;
using Abp.Organizations;
using System.Linq;
using Abp;
using System.Threading.Tasks;


namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Used to perform database operations for <see cref="UserManager"/>.
    /// </summary>
    public class UserStore : AbpUserStore<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

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
            _unitOfWorkManager = unitOfWorkManager;
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

        /// <summary>
        /// Busca o username do usuário pelo id sem restringir pelo tenant atual.
        /// A busca por chave primária não deve depender do filtro de multi-tenancy,
        /// pois o método pode ser chamado dentro de uma nova unidade de trabalho
        /// sem o contexto de tenant do usuário.
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        /// <returns>Resultado da operação.</returns>
        public override async Task<string> GetUserNameFromDatabaseAsync(long userId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var user = await UserRepository.GetAsync(userId);
                return user.UserName;
            }
        }

        /// <summary>
        /// Busca o username do usuário pelo id sem restringir pelo tenant atual (síncrono).
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        /// <returns>Resultado da operação.</returns>
        public override string GetUserNameFromDatabase(long userId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var user = UserRepository.Get(userId);
                return user.UserName;
            }
        }
    }
}