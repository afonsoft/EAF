using Abp;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Threading;
using Abp.UI;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.CollectionExtensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eaf.Middleware;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// User manager. Used to implement domain logic for users. Extends <see cref="AbpUserManager{TRole,TUser}"/>.
    /// </summary>
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _userRepository;
        private const string TokenValidityKeyProvider = "TokenValidityKeyProvider";

        /// <summary>
        /// UserManager.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public UserManager(
            UserStore userStore,
            IRepository<User, long> userRepository,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager> logger,
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            ISettingManager settingManager,
            ILocalizationManager localizationManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            IRepository<UserLogin, long> userLoginRepository)
            : base(
                  roleManager,
                  userStore,
                  optionsAccessor,
                  passwordHasher,
                  userValidators,
                  passwordValidators,
                  keyNormalizer,
                  errors,
                  services,
                  logger,
                  permissionManager,
                  unitOfWorkManager,
                  cacheManager,
                  organizationUnitRepository,
                  userOrganizationUnitRepository,
                  organizationUnitSettings,
                  settingManager,
                  userLoginRepository
                  )
        {
            _userRepository = userRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _localizationManager = localizationManager;
        }

        /// <summary>
        /// GetUser.
        /// </summary>
        /// <param name="userIdentifier">Parâmetro userIdentifier.</param>
        /// <returns>Resultado da operação.</returns>
        public User GetUser(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
        }

        /// <summary>
        /// GetUserAsync.
        /// </summary>
        /// <param name="userIdentifier">Parâmetro userIdentifier.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            if (user == null)
                throw new Exception("There is no user: " + userIdentifier);

            return user;
        }

        /// <summary>
        /// GetUserByLoginAsync.
        /// </summary>
        /// <param name="userName">Parâmetro userName.</param>
        /// <param name="tanantId">Parâmetro tanantId.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<User> GetUserByLoginAsync(string userName, int? tanantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tanantId))
            {
                var normalizedUserName = userName.ToUpperInvariant().Trim();
                return await _userRepository.FirstOrDefaultAsync(ua => ua.NormalizedUserName == normalizedUserName);
            }
        }

        /// <summary>
        /// GetUserOrNull.
        /// </summary>
        /// <param name="userIdentifier">Parâmetro userIdentifier.</param>
        /// <returns>Resultado da operação.</returns>
        public User GetUserOrNull(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
        }

        [UnitOfWork]
        public virtual async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
        {
            using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
            {
                return await FindByIdAsync(userIdentifier.UserId.ToString());
            }
        }

        /// <summary>
        /// SetGrantedPermissionsAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="permissions">Parâmetro permissions.</param>
        public override async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(user, permissions);

            await base.SetGrantedPermissionsAsync(user, permissions);
        }

        /// <summary>
        /// SetRolesAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="roleNames">Parâmetro roleNames.</param>
        /// <returns>Resultado da operação.</returns>
        public override Task<IdentityResult> SetRolesAsync(User user, string[] roleNames)
        {
            if (user.Name == "admin" && !roleNames.Contains(StaticRoleNames.Host.Admin))
            {
                throw new UserFriendlyException(L("AdminRoleCannotRemoveFromAdminUser"));
            }

            return base.SetRolesAsync(user, roleNames);
        }

        private void CheckPermissionsToUpdate(User user, IEnumerable<Permission> permissions)
        {
            if (user.Name == AbpUserBase.AdminUserName &&
                (!permissions.Any(p => p.Name == MiddlewarePermissions.Pages_Administration_Roles_Edit) ||
                !permissions.Any(p => p.Name == MiddlewarePermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminUser"));
            }
        }

        private new string L(string name)
        {
            return _localizationManager.GetString("EafCore", name);
        }

        /// <summary>
        /// UpdateWithValidateAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<IdentityResult> UpdateWithValidateAsync(User user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress, user.AuthenticationSource);
            if (!result.Succeeded)
            {
                return result;
            }

            //Admin user's username can not be changed!
            if (user.UserName != AbpUserBase.AdminUserName
                && (await GetOldUserNameAsync(user.Id)) == AbpUserBase.AdminUserName)
            {
                throw new UserFriendlyException(
                    string.Format(L("CanNotRenameAdminUser"), AbpUserBase.AdminUserName));
            }

            return await AbpUserStore.UpdateAsync(user, default);
        }

        /// <summary>
        /// CheckDuplicateUsernameOrEmailAddressAsync.
        /// </summary>
        /// <param name="expectedUserId">Parâmetro expectedUserId.</param>
        /// <param name="userName">Parâmetro userName.</param>
        /// <param name="emailAddress">Parâmetro emailAddress.</param>
        /// <param name="authenticationSource">Parâmetro authenticationSource.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual async Task<IdentityResult> CheckDuplicateUsernameOrEmailAddressAsync(long? expectedUserId,
            string userName, string emailAddress, string authenticationSource)
        {
            User user;
            if (!string.IsNullOrEmpty(userName))
            {
                user = (await FindByNameAsync(userName));
                if (user != null && user.Id != expectedUserId)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "1", Description = string.Format(L("Identity.DuplicateUserName"), userName) });
                }
            }
            if (!string.IsNullOrEmpty(emailAddress))
            {
                user = (await FindByEmailAsync(emailAddress));
                if (user != null && user.Id != expectedUserId && user.AuthenticationSource == authenticationSource)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "2", Description = string.Format(L("Identity.DuplicateEmail"), emailAddress) });
                }
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// RemoveAllTokenValidityKeyAsync.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="cancellationToken">Parâmetro cancellationToken.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual async Task<List<string>> RemoveAllTokenValidityKeyAsync(
                    [NotNull] User user,
                    CancellationToken cancellationToken)
        {
            var tokens = new List<string>();
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Check.NotNull(user, nameof(user));
                await _userRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);
                tokens = user.Tokens.Where(t => t.LoginProvider == TokenValidityKeyProvider && t.UserId == user.Id).Select(t => t.Name).ToList();
                user.Tokens.RemoveAll(t => t.LoginProvider == TokenValidityKeyProvider && t.UserId == user.Id);
            });

            return tokens;
        }
    }
}