using Abp;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Caching;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Runtime.Caching.Memory;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Eaf.Middleware.Tests.Helpers
{
    /// <summary>
    /// Helper para criação de managers e dependências substituídas em testes Core BDD.
    /// </summary>
    public static class CoreManagerTestHelper
    {
        public static IUnitOfWorkManager CreateUnitOfWorkManager()
        {
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());
            activeUnitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
            unitOfWorkManager.Current.Returns(activeUnitOfWork);

            var uowHandle = Substitute.For<IUnitOfWorkCompleteHandle>();
            uowHandle.CompleteAsync().Returns(Task.CompletedTask);
            unitOfWorkManager.Begin().Returns(uowHandle);
            unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>()).Returns(uowHandle);
            unitOfWorkManager.Begin(Arg.Any<TransactionScopeOption>()).Returns(uowHandle);

            return unitOfWorkManager;
        }

        public static UserManager CreateUserManager()
        {
            return CreateUserManager(out _);
        }

        public static UserManager CreateUserManager(out IRepository<User, long> userRepository)
        {
            userRepository = Substitute.For<IRepository<User, long>, ISupportsExplicitLoading<User, long>>();
            userRepository.GetAll().Returns(new List<User>().AsQueryable());

            var userStore = Substitute.For<UserStore>(new object[10]);
            userStore.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);

            var optionsAccessor = Options.Create(new IdentityOptions());
            var passwordHasher = Substitute.For<IPasswordHasher<User>>();
            var userValidators = Array.Empty<IUserValidator<User>>();
            var passwordValidators = Array.Empty<IPasswordValidator<User>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var services = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<UserManager>>();
            var roleManager = CreateRoleManager();
            var permissionManager = Substitute.For<IPermissionManager>();
            var unitOfWorkManager = CreateUnitOfWorkManager();
            var cacheManager = Substitute.For<ICacheManager>();
            var settingManager = Substitute.For<ISettingManager>();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var userOrganizationUnitRepository = Substitute.For<IRepository<UserOrganizationUnit, long>>();
            var organizationUnitSettings = Substitute.For<IOrganizationUnitSettings>();
            var userLoginRepository = Substitute.For<IRepository<UserLogin, long>>();

            var userManager = Substitute.For<UserManager>(new object[]
            {
                userStore, userRepository, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                keyNormalizer, errors, services, logger, roleManager, permissionManager, unitOfWorkManager,
                cacheManager, settingManager, localizationManager, organizationUnitRepository,
                userOrganizationUnitRepository, organizationUnitSettings, userLoginRepository
            });

            userManager.GetUserIdAsync(Arg.Any<User>()).Returns(t => ((User)t[0]).Id.ToString());
            userManager.GetUserNameAsync(Arg.Any<User>()).Returns(t => ((User)t[0]).UserName);
            userManager.GetEmailAsync(Arg.Any<User>()).Returns(t => ((User)t[0]).EmailAddress);
            userManager.GetPhoneNumberAsync(Arg.Any<User>()).Returns(t => ((User)t[0]).PhoneNumber);
            userManager.IsEmailConfirmedAsync(Arg.Any<User>()).Returns(true);
            userManager.IsPhoneNumberConfirmedAsync(Arg.Any<User>()).Returns(true);
            userManager.GetTwoFactorEnabledAsync(Arg.Any<User>()).Returns(false);
            userManager.GetRolesAsync(Arg.Any<User>()).Returns(new List<string>());
            userManager.UpdateAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            userManager.InitializeOptionsAsync(Arg.Any<int?>()).Returns(Task.CompletedTask);
            userManager.CheckPasswordAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(true);
            userManager.IsLockedOutAsync(Arg.Any<User>()).Returns(false);
            userManager.ResetAccessFailedCountAsync(Arg.Any<User>()).Returns(IdentityResult.Success);
            userManager.AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.CreateAsync(Arg.Any<User>()).Returns(t => { ((User)t[0]).Id = 2; return IdentityResult.Success; });

            return userManager;
        }

        public static RoleManager CreateRoleManager()
        {
            return CreateRoleManager(out _);
        }

        public static RoleManager CreateRoleManager(out IPermissionManager permissionManager)
        {
            var roleStore = Substitute.For<RoleStore>(new object[]
            {
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            });
            var roleValidators = Array.Empty<IRoleValidator<Role>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var logger = Substitute.For<ILogger<RoleManager>>();
            permissionManager = Substitute.For<IPermissionManager>();
            var roleManagementConfig = Substitute.For<IRoleManagementConfig>();
            var cacheManager = Substitute.For<ICacheManager>();
            var unitOfWorkManager = CreateUnitOfWorkManager();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var organizationUnitRoleRepository = Substitute.For<IRepository<OrganizationUnitRole, long>>();

            var roleManager = Substitute.For<RoleManager>(new object[]
            {
                roleStore, roleValidators, keyNormalizer, errors, logger, permissionManager,
                roleManagementConfig, cacheManager, unitOfWorkManager, localizationManager,
                organizationUnitRepository, organizationUnitRoleRepository
            });

            roleManager.SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>()).Returns(Task.CompletedTask);
            roleManager.FeatureDependencyContext = new FeatureDependencyContext(Substitute.For<IIocResolver>(), Substitute.For<IFeatureChecker>());
            return roleManager;
        }

        public static UserClaimsPrincipalFactory CreateUserClaimsPrincipalFactory(UserManager userManager = null, RoleManager roleManager = null)
        {
            userManager ??= CreateUserManager();
            roleManager ??= CreateRoleManager();
            var unitOfWorkManager = CreateUnitOfWorkManager();
            var options = Options.Create(new IdentityOptions());
            return new UserClaimsPrincipalFactory(userManager, roleManager, options, unitOfWorkManager);
        }

        public static ImpersonationManager CreateImpersonationManager(IAbpSession abpSession = null)
        {
            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("AppImpersonationCache").Returns(new AbpMemoryCache("AppImpersonationCache"));

            var userManager = CreateUserManager();
            var targetUser = new User
            {
                Id = 2,
                TenantId = 1,
                UserName = "target",
                Name = "Target",
                Surname = "User",
                EmailAddress = "target@example.com",
                IsActive = true,
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = true,
                IsTwoFactorEnabled = false
            };
            var impersonatorUser = new User
            {
                Id = 1,
                TenantId = 1,
                UserName = "admin",
                Name = "Admin",
                Surname = "User",
                EmailAddress = "admin@example.com",
                IsActive = true,
                IsEmailConfirmed = true,
                IsPhoneNumberConfirmed = true,
                IsTwoFactorEnabled = false
            };
            userManager.FindByIdAsync("1").Returns(impersonatorUser);
            userManager.FindByIdAsync("2").Returns(targetUser);

            var principalFactory = CreateUserClaimsPrincipalFactory(userManager, CreateRoleManager());
            var userTokenRepository = Substitute.For<IRepository<UserToken, long>>();
            userTokenRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<UserToken, bool>>>()).Returns((UserToken)null);

            var sut = new ImpersonationManager(cacheManager, userManager, principalFactory, userTokenRepository);
            sut.AbpSession = abpSession ?? Substitute.For<IAbpSession>();
            return sut;
        }

        public static TenantManager CreateTenantManager(out UserManager userManager, out RoleManager roleManager, out IPasswordHasher<User> passwordHasher, out IRepository<Tenant> tenantRepository, out IPermissionManager permissionManager)
        {
            tenantRepository = Substitute.For<IRepository<Tenant>>();
            tenantRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<Tenant, bool>>>()).Returns((Tenant)null);
            tenantRepository.InsertAsync(Arg.Any<Tenant>()).Returns(t =>
            {
                var tenant = (Tenant)t[0];
                tenant.Id = 1;
                return tenant;
            });

            var tenantFeatureRepository = Substitute.For<IRepository<TenantFeatureSetting, long>>();
            var unitOfWorkManager = CreateUnitOfWorkManager();
            roleManager = CreateRoleManager(out permissionManager);
            roleManager.Roles.Returns(new List<Role> { new Role(1, "Admin", "Admin") { Id = 1, TenantId = 1, IsStatic = true } }.AsQueryable());
            roleManager.CreateStaticRoles(Arg.Any<int>()).Returns(IdentityResult.Success);
            permissionManager.GetAllPermissionsAsync(Arg.Any<MultiTenancySides>()).Returns(new List<Permission>());

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.SendEmailActivationLinkAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

            userManager = CreateUserManager();
            userManager.CreateAsync(Arg.Any<User>()).Returns(t =>
            {
                var user = (User)t[0];
                user.Id = 2;
                return IdentityResult.Success;
            });

            var notificationSubscriptionManager = Substitute.For<INotificationSubscriptionManager>();
            notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(Arg.Any<UserIdentifier>()).Returns(Task.CompletedTask);

            var featureValueStore = Substitute.For<IAbpZeroFeatureValueStore>();
            passwordHasher = Substitute.For<IPasswordHasher<User>>();
            passwordHasher.HashPassword(Arg.Any<User>(), Arg.Any<string>()).Returns("hashed");

            var editionManager = Substitute.For<EditionManager>(new object[]
            {
                Substitute.For<IRepository<Edition>>(),
                Substitute.For<IAbpZeroFeatureValueStore>(),
                Substitute.For<IUnitOfWorkManager>()
            });

            var tenantManager = new TenantManager(
                tenantRepository,
                tenantFeatureRepository,
                unitOfWorkManager,
                roleManager,
                userEmailer,
                userManager,
                notificationSubscriptionManager,
                featureValueStore,
                passwordHasher,
                editionManager
            );
            tenantManager.UnitOfWorkManager = unitOfWorkManager;
            return tenantManager;
        }
    }
}
