using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eaf.Middleware.Identity
{
    /// <summary>
    /// Representa a classe SignInManager.
    /// </summary>
    public class SignInManager : AbpSignInManager<Tenant, Role, User>
    {
        /// <summary>
        /// SignInManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger, // NOSONAR
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> userConfirmation
            ) : base(
                userManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                unitOfWorkManager,
                settingManager,
                schemes,
                userConfirmation)
        {
        }
    }
}