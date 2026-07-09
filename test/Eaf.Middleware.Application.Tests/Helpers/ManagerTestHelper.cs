using Abp;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Caching;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Eaf.Middleware.Application.Tests.Helpers
{
    /// <summary>
    /// Helper para criação de managers substituídos em testes BDD.
    /// </summary>
    public static class ManagerTestHelper
    {
        public static UserManager CreateUserManager()
        {
            return CreateUserManager(out _);
        }

        public static UserManager CreateUserManager(out IRepository<User, long> userRepository)
        {
            return CreateUserManager(out userRepository, out _);
        }

        public static UserManager CreateUserManager(out IRepository<User, long> userRepository, out UserStore userStore)
        {
            userStore = Substitute.For<UserStore>(new object[10]);
            userStore.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);

            userRepository = Substitute.For<IRepository<User, long>, ISupportsExplicitLoading<User, long>>();
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
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());
            unitOfWorkManager.Current.Returns(activeUnitOfWork);

            var uowHandle = Substitute.For<IUnitOfWorkCompleteHandle>();
            uowHandle.CompleteAsync().Returns(Task.CompletedTask);
            unitOfWorkManager.Begin().Returns(uowHandle);
            unitOfWorkManager.Begin(Arg.Any<UnitOfWorkOptions>()).Returns(uowHandle);
            unitOfWorkManager.Begin(Arg.Any<TransactionScopeOption>()).Returns(uowHandle);
            var cacheManager = Substitute.For<ICacheManager>();
            var settingManager = Substitute.For<ISettingManager>();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var userOrganizationUnitRepository = Substitute.For<IRepository<UserOrganizationUnit, long>>();
            var organizationUnitSettings = Substitute.For<IOrganizationUnitSettings>();
            var userLoginRepository = Substitute.For<IRepository<UserLogin, long>>();

            return Substitute.For<UserManager>(new object[]
            {
                userStore, userRepository, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                keyNormalizer, errors, services, logger, roleManager, permissionManager, unitOfWorkManager,
                cacheManager, settingManager, localizationManager, organizationUnitRepository,
                userOrganizationUnitRepository, organizationUnitSettings, userLoginRepository
            });
        }

        public static RoleManager CreateRoleManager()
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
            var permissionManager = Substitute.For<IPermissionManager>();
            var roleManagementConfig = Substitute.For<IRoleManagementConfig>();
            var cacheManager = Substitute.For<ICacheManager>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var localizationManager = Substitute.For<ILocalizationManager>();
            var organizationUnitRepository = Substitute.For<IRepository<OrganizationUnit, long>>();
            var organizationUnitRoleRepository = Substitute.For<IRepository<OrganizationUnitRole, long>>();

            return Substitute.For<RoleManager>(new object[]
            {
                roleStore, roleValidators, keyNormalizer, errors, logger, permissionManager,
                roleManagementConfig, cacheManager, unitOfWorkManager, localizationManager,
                organizationUnitRepository, organizationUnitRoleRepository
            });
        }

        public static TenantManager CreateTenantManager()
        {
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var tenantFeatureRepository = Substitute.For<IRepository<TenantFeatureSetting, long>>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var roleManager = CreateRoleManager();
            var userEmailer = Substitute.For<IUserEmailer>();
            var userManager = CreateUserManager();
            var notificationSubscriptionManager = Substitute.For<INotificationSubscriptionManager>();
            var featureValueStore = Substitute.For<IAbpZeroFeatureValueStore>();
            var passwordHasher = Substitute.For<IPasswordHasher<User>>();
            var editionManager = Substitute.For<EditionManager>(new object[]
            {
                Substitute.For<IRepository<Edition>>(),
                Substitute.For<IAbpZeroFeatureValueStore>(),
                Substitute.For<IUnitOfWorkManager>()
            });

            return Substitute.For<TenantManager>(new object[]
            {
                tenantRepository, tenantFeatureRepository, unitOfWorkManager, roleManager, userEmailer,
                userManager, notificationSubscriptionManager, featureValueStore, passwordHasher, editionManager
            });
        }
    }
}
