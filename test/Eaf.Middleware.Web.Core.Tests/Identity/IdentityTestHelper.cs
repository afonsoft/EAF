using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
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
using Castle.MicroKernel.Registration;
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

namespace Eaf.Middleware.Web.Core.Tests.Identity
{
    /// <summary>
    /// Helper para criar manualmente os wrappers de identity em testes BDD Web.Core.
    /// </summary>
    public static class IdentityTestHelper
    {
        public static User CreateUser(string userName = "admin", long id = 1, int? tenantId = 1, string securityStamp = null)
        {
            securityStamp ??= Guid.NewGuid().ToString("N");
            var user = new User
            {
                Id = id,
                TenantId = tenantId,
                UserName = userName,
                Name = userName,
                Surname = userName,
                EmailAddress = $"{userName}@email.com",
                SecurityStamp = securityStamp,
                AuthenticationSource = "System",
                IsActive = true,
                IsEmailConfirmed = true,
                Tokens = new List<UserToken>()
            };
            user.SetNormalizedNames();
            return user;
        }

        public static UserToken CreateTokenValidityKeyToken(User user, string tokenKey, string value = null, DateTime? expireDate = null)
        {
            expireDate ??= DateTime.UtcNow.AddDays(1);
            return new TestableUserToken(user, "TokenValidityKeyProvider", tokenKey, value, expireDate);
        }

        private sealed class TestableUserToken : UserToken
        {
            public TestableUserToken(AbpUserBase user, string loginProvider, string name, string value, DateTime? expireDate)
                : base(user, loginProvider, name, value, expireDate)
            {
            }
        }

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

        public static UserManager CreateUserManager(User user = null, IUnitOfWorkManager unitOfWorkManager = null)
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

            return new UserManager(
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

        public static Eaf.Middleware.Authorization.LogInManager CreateApplicationLogInManager(UserManager userManager, RoleManager roleManager, IUnitOfWorkManager unitOfWorkManager = null)
        {
            unitOfWorkManager ??= CreateUnitOfWorkManager();
            var claimsPrincipalFactory = CreateUserClaimsPrincipalFactory(userManager, roleManager, unitOfWorkManager);

            return new Eaf.Middleware.Authorization.LogInManager(
                userManager,
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<IRepository<Tenant>>(),
                unitOfWorkManager,
                Substitute.For<ISettingManager>(),
                Substitute.For<IRepository<UserLoginAttempt, long>>(),
                Substitute.For<IUserManagementConfig>(),
                Substitute.For<IIocResolver>(),
                roleManager,
                Substitute.For<IPasswordHasher<User>>(),
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

        public static void RegisterJwtDependencies(
            UserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            ICacheManager cacheManager)
        {
            if (!IocManager.Instance.IsRegistered<IUnitOfWorkManager>())
                IocManager.Instance.IocContainer.Register(Component.For<IUnitOfWorkManager>().Instance(unitOfWorkManager).LifestyleSingleton());

            if (!IocManager.Instance.IsRegistered<ISettingManager>())
                IocManager.Instance.IocContainer.Register(Component.For<ISettingManager>().Instance(settingManager).LifestyleSingleton());

            if (!IocManager.Instance.IsRegistered<ICacheManager>())
                IocManager.Instance.IocContainer.Register(Component.For<ICacheManager>().Instance(cacheManager).LifestyleSingleton());

            if (!IocManager.Instance.IsRegistered<UserManager>())
                IocManager.Instance.IocContainer.Register(Component.For<UserManager>().Instance(userManager).LifestyleSingleton());
        }
    }
}
