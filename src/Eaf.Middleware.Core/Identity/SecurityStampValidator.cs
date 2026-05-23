using Abp.Authorization;
using Abp.Domain.Uow;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eaf.Middleware.Identity
{
    /// <summary>
    /// Representa a classe SecurityStampValidator.
    /// </summary>
    public class SecurityStampValidator : AbpSecurityStampValidator<Tenant, Role, User>
    {
        /// <summary>
        /// SecurityStampValidator.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <param name="signInManager">Parâmetro signInManager.</param>
        /// <param name="loggerFactory">Parâmetro loggerFactory.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <returns>Resultado da operação.</returns>
        public SecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            SignInManager signInManager,
            ILoggerFactory loggerFactory,
            IUnitOfWorkManager unitOfWorkManager)
            : base(options, signInManager, loggerFactory, unitOfWorkManager)
        {
        }
    }
}