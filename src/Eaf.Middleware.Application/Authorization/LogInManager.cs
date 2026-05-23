using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization
{
    /// <summary>
    /// Representa a classe LogInManager.
    /// </summary>
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        /// <summary>
        /// LogInManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public LogInManager(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            RoleManager roleManager,
            IPasswordHasher<User> passwordHasher,
            UserClaimsPrincipalFactory claimsPrincipalFactory)
            : base(
                  userManager,
                  multiTenancyConfig,
                  tenantRepository,
                  unitOfWorkManager,
                  settingManager,
                  userLoginAttemptRepository,
                  userManagementConfig,
                  iocResolver,
                  passwordHasher,
                  roleManager,
                  claimsPrincipalFactory)
        {
        }

        /// <summary>
        /// CreateLoginResultAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="tenant">Parâmetro tenant.</param>
        /// <returns>Resultado da operação.</returns>
        public new async Task<AbpLoginResult<Tenant, User>> CreateLoginResultAsync(User user,
       Tenant tenant = null)
        {
            return await base.CreateLoginResultAsync(user, tenant);
        }
    }
}