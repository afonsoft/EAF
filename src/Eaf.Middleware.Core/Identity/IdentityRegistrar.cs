using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Features;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eaf.Middleware.Identity
{
    /// <summary>
    /// Representa a classe IdentityRegistrar.
    /// </summary>
    public static class IdentityRegistrar
    {
        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="services">Parâmetro services.</param>
        /// <returns>Resultado da operação.</returns>
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddAbpIdentity<Tenant, User, Role>()
            .AddAbpTenantManager<TenantManager>()
            .AddAbpEditionManager<EditionManager>()
            .AddAbpRoleManager<RoleManager>()
            .AddAbpUserManager<UserManager>()
            .AddAbpSignInManager<SignInManager>()
            .AddAbpLogInManager<LogInManager>()
            .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
            .AddAbpSecurityStampValidator<SecurityStampValidator>()
            .AddPermissionChecker<PermissionChecker>()
            .AddAbpUserStore<UserStore>()
            .AddAbpRoleStore<RoleStore>()
            .AddFeatureValueStore<FeatureValueStore>()
            .AddDefaultTokenProviders();
        }
    }
}