using Abp.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Identity;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Authorization.Roles;

namespace Eaf.Middleware
{
    /// <summary>
    /// Helper para criar manualmente os wrappers de identity em testes BDD.
    /// </summary>
    internal static class IdentityHelper
    {
        public static IUnitOfWorkManager CreateUnitOfWorkManager()
        {
            var activeUow = Substitute.For<IUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            activeUow.DisableFilter(Arg.Any<string[]>()).Returns(Substitute.For<IDisposable>());
            activeUow.CompleteAsync().Returns(Task.CompletedTask);

            var uowManager = Substitute.For<IUnitOfWorkManager>();
            uowManager.Current.Returns(activeUow);
            uowManager.Begin().Returns(activeUow);
            uowManager.Begin(Arg.Any<UnitOfWorkOptions>()).Returns(activeUow);

            return uowManager;
        }

        public static UserManager CreateUserManager(IUnitOfWorkManager unitOfWorkManager = null, User user = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();

            var users = new List<User>();
            if (user != null)
                users.Add(user);

            var userRepository = Substitute.For<IRepository<User, long>>();
            userRepository.GetAll().Returns(users.AsQueryable());
            userRepository.GetAllAsync().Returns(Task.FromResult(users.AsQueryable()));
            userRepository.FirstOrDefaultAsync(Arg.Any<long>()).Returns(Task.FromResult(user));

            var userStore = new UserStore(
                userRepository,
                Substitute.For<IRepository<UserLogin, long>>(),
                Substitute.For<IRepository<UserRole, long>>(),
                Substitute.For<IRepository<Role>>(),
                unitOfWorkManager,
                Substitute.For<IRepository<UserClaim, long>>(),
                Substitute.For<IRepository<UserPermissionSetting, long>>(),
                Substitute.For<IRepository<UserOrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>(),
                Substitute.For<IRepository<UserToken, long>>()
            );

            var roleManager = CreateRoleManager(unitOfWorkManager);

            var userManager = new UserManager(
                userStore,
                userRepository,
                Options.Create(new IdentityOptions()),
                Substitute.For<IPasswordHasher<User>>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager>>(),
                roleManager,
                Substitute.For<IPermissionManager>(),
                unitOfWorkManager,
                Substitute.For<ICacheManager>(),
                Substitute.For<ISettingManager>(),
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<UserOrganizationUnit, long>>(),
                Substitute.For<IOrganizationUnitSettings>(),
                Substitute.For<IRepository<UserLogin, long>>()
            );

            return userManager;
        }

        public static RoleManager CreateRoleManager(IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();

            var roleRepository = Substitute.For<IRepository<Role>>();
            roleRepository.GetAll().Returns(new List<Role>().AsQueryable());
            roleRepository.GetAllAsync().Returns(Task.FromResult(new List<Role>().AsQueryable()));

            var roleStore = new RoleStore(
                unitOfWorkManager,
                roleRepository,
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            return new RoleManager(
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                unitOfWorkManager,
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            );
        }

        public static UserClaimsPrincipalFactory CreateUserClaimsPrincipalFactory(UserManager userManager, RoleManager roleManager, IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();
            return new UserClaimsPrincipalFactory(
                userManager,
                roleManager,
                Options.Create(new IdentityOptions()),
                unitOfWorkManager
            );
        }

        public static LogInManager CreateLogInManager(UserManager userManager, RoleManager roleManager, IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();
            var claimsPrincipalFactory = CreateUserClaimsPrincipalFactory(userManager, roleManager, unitOfWorkManager);

            return new LogInManager(
                userManager,
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<IRepository<Tenant>>(),
                unitOfWorkManager,
                Substitute.For<ISettingManager>(),
                Substitute.For<IRepository<UserLoginAttempt, long>>(),
                Substitute.For<IUserManagementConfig>(),
                Substitute.For<IIocResolver>(),
                Substitute.For<IPasswordHasher<User>>(),
                roleManager,
                claimsPrincipalFactory
            );
        }

        public static SignInManager CreateSignInManager(UserManager userManager, RoleManager roleManager, IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();
            var claimsPrincipalFactory = CreateUserClaimsPrincipalFactory(userManager, roleManager, unitOfWorkManager);

            return new SignInManager(
                userManager,
                Substitute.For<IHttpContextAccessor>(),
                claimsPrincipalFactory,
                Options.Create(new IdentityOptions()),
                Substitute.For<ILogger<SignInManager<User>>>(),
                unitOfWorkManager,
                Substitute.For<ISettingManager>(),
                Substitute.For<IAuthenticationSchemeProvider>(),
                new DefaultUserConfirmation<User>()
            );
        }

        public static Eaf.Middleware.Identity.SecurityStampValidator CreateSecurityStampValidator(SignInManager signInManager, IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();

            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return new Eaf.Middleware.Identity.SecurityStampValidator(
                Options.Create(new SecurityStampValidatorOptions()),
                signInManager,
                loggerFactory,
                unitOfWorkManager
            );
        }
    }
}
