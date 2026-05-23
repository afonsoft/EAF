using Abp.Authorization;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Representa a classe UserClaimsPrincipalFactory.
    /// </summary>
    public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
    {
        /// <summary>
        /// UserClaimsPrincipalFactory.
        /// </summary>
        /// <param name="userManager">Parâmetro userManager.</param>
        /// <param name="roleManager">Parâmetro roleManager.</param>
        /// <param name="optionsAccessor">Parâmetro optionsAccessor.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <returns>Resultado da operação.</returns>
        public UserClaimsPrincipalFactory(
            UserManager userManager,
            RoleManager roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                  userManager,
                  roleManager,
                  optionsAccessor,
                  unitOfWorkManager)
        {
        }
    }
}