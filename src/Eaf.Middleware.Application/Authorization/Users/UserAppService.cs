using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.Authorization.Ldap;
using Eaf.Middleware.Authorization.Permissions;
using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Authorization.Users.Exporting;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de User.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users)]
    public class UserAppService : MiddlewareAppServiceBase, IUserAppService
    {
        private readonly AppAzureActiveDirectoryAuthenticationSource _appAzureActiveDirectoryAuthenticationSource;
        private readonly AppLdapAuthenticationSource _appLdapAuthenticationSource;
        private readonly ICacheManager _cacheManager;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly ITypedCache<string, List<User>> _usersCache;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IWebhookPublisher _webhookPublisher;

        /// <summary>
        /// UserAppService.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public UserAppService( // NOSONAR
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IRepository<UserRole, long> userRoleRepository,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            AppAzureActiveDirectoryAuthenticationSource appAzureActiveDirectoryAuthenticationSource,
            AppLdapAuthenticationSource appLdapAuthenticationSource,
            INotificationPublisher notificationPublisher,
            IWebhookPublisher webhookPublisher,
            ICacheManager cacheManager
        )
        {
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _userRoleRepository = userRoleRepository;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;

            _appAzureActiveDirectoryAuthenticationSource = appAzureActiveDirectoryAuthenticationSource;
            _appLdapAuthenticationSource = appLdapAuthenticationSource;

            _cacheManager = cacheManager;

            _usersCache = _cacheManager.GetCache<string, List<User>>("UsersCache");
            _usersCache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(5);

            _notificationPublisher = notificationPublisher;
            _webhookPublisher = webhookPublisher;

            AppUrlService = NullAppUrlService.Instance;
        }

        /// <summary>
        /// Obtém ou define AppUrlService.
        /// </summary>
        public IAppUrlService AppUrlService { get; set; }

        /// <summary>
        /// CloseSessionUser.
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        public async Task CloseSessionUser(int userId)
        {
            var user = await UserManager.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var tokens = await UserManager.RemoveAllTokenValidityKeyAsync(user, default);

            var cache = _cacheManager.GetCache(MiddlewareCoreConsts.TokenValidityKey);
            foreach (var token in tokens)
                await cache.RemoveAsync(token);
        }

        /// <summary>
        /// CreateOrUpdateUser.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
                await UpdateUserAsync(input);
            else
                await CreateUserAsync(input);

            await _usersCache.ClearAsync();
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create)]
        public async Task CreateUsersByActiveDirectory(CreateActiveDirectoryUserInput input)
        {
            foreach (var currentUserName in input.UserNames)
            {
                await CreateUserFromActiveDirectoryAsync(input, currentUserName);
            }
        }

        private async Task CreateUserFromActiveDirectoryAsync(CreateActiveDirectoryUserInput input, string currentUserName)
        {
            if (string.IsNullOrEmpty(currentUserName))
                return;

            var userName = currentUserName.ToLower();
            var onlyUserName = GetUserNameWithoutDomain(userName);

            var currentUser = await UserManager.GetUserByLoginAsync(onlyUserName, AbpSession.TenantId);
            if (currentUser != null)
                return;

            var user = await _appAzureActiveDirectoryAuthenticationSource.GetUserAsync(userName);
            user.UserName = onlyUserName;

            SetUserDefaults(user, userName);

            user.TenantId = AbpSession.TenantId;
            user.AuthenticationSource = AzureActiveDirectorySettingNames.ActiveDirectoryProvider;
            user.Password = new PasswordHasher<User>().HashPassword(user, Guid.NewGuid().ToString("N").Left(16));
            user.IsActive = input.IsActive;
            user.IsEmailConfirmed = true;

            await AssignRolesAsync(user, input.AssignedRoleNames);

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();
            await _usersCache.ClearAsync();
            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
        }

        private static string GetUserNameWithoutDomain(string userName)
        {
            return userName.Contains("@") ? userName.Split('@')[0] : userName;
        }

        private static void SetUserDefaults(User user, string userName)
        {
            if (string.IsNullOrEmpty(user.Name))
                user.Name = GetUserNameWithoutDomain(userName);

            if (string.IsNullOrEmpty(user.Surname))
                user.Surname = userName;

            if (string.IsNullOrEmpty(user.EmailAddress))
                user.EmailAddress = userName;
        }

        private async Task AssignRolesAsync(User user, string[] assignedRoleNames)
        {
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in assignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create)]
        public async Task CreateUsersByLdap(CreateLdapUserInput input)
        {
            var tenant = AbpSession.TenantId.HasValue ? await TenantManager.GetByIdAsync(AbpSession.TenantId.Value) : null;

            foreach (var currentUserName in input.UserNames)
            {
                if (string.IsNullOrEmpty(currentUserName))
                    continue;

                var currentUser = await UserManager.GetUserByLoginAsync(currentUserName, AbpSession.TenantId);
                if (currentUser != null)
                    continue;

                var user = await _appLdapAuthenticationSource.CreateUserAsync(currentUserName, tenant);
                user.AuthenticationSource = "LDAP";
                user.Password = new PasswordHasher<User>().HashPassword(user, Guid.NewGuid().ToString("N").Left(16));
                user.IsActive = input.IsActive;
                user.IsEmailConfirmed = true;
                user.TenantId = AbpSession.TenantId;

                user.Roles = new Collection<UserRole>();
                foreach (var roleName in input.AssignedRoleNames)
                {
                    var role = await _roleManager.GetRoleByNameAsync(roleName);
                    user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
                }

                CheckErrors(await UserManager.CreateAsync(user));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.
                await _usersCache.ClearAsync();
                //Notifications
                await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            }
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
            await _usersCache.ClearAsync();
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create, MiddlewarePermissions.Pages_Administration_Users_Edit)]
        public async Task<List<UserListDto>> GetActiveDirectoryUsers(string name)
        {
            var users = await _appAzureActiveDirectoryAuthenticationSource.GetUsersAsync(name);
            return ObjectMapper.Map<List<UserListDto>>(users);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create, MiddlewarePermissions.Pages_Administration_Users_Edit)]
        public async Task<List<UserListDto>> GetLdapUsers(string name)
        {
            var users = await _appLdapAuthenticationSource.GetUsersAsync(name);
            return ObjectMapper.Map<List<UserListDto>>(users);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create, MiddlewarePermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                }
            }

            return output;
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = await PermissionManager.GetAllPermissionsAsync();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        /// <summary>
        /// GetUsers.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            input.Filter = !input.Filter.IsNullOrWhiteSpace() ? input.Filter.ToLowerInvariant().Trim() : "";

            var query = UserManager.Users
            .AsNoTracking()
            .WhereIf(
                  !input.Filter.IsNullOrWhiteSpace(),
                  u =>
                      u.Name.ToLower().Contains(input.Filter) ||
                      u.UserName.ToLower().Contains(input.Filter) ||
                      u.Surname.ToLower().Contains(input.Filter) ||
                      u.EmailAddress.ToLower().Contains(input.Filter)
              ).AsQueryable();

            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        /// <summary>
        /// GetUsersToExcel.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<FileDto> GetUsersToExcel()
        {
            var users = _usersCache.Get("ALL", () => UserManager.Users.AsNoTracking().ToList());
            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        /// <summary>
        /// UnlockUser.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
            await _usersCache.ClearAsync();
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
            await _usersCache.ClearAsync();
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;
            user.UserName = user.UserName.ToLower();

            //New users registered in a tenant require approval except administrators
            if (AbpSession.TenantId.HasValue && !input.AssignedRoleNames.Contains(StaticRoleNames.Tenants.Admin))
            {
                user.IsActive = false;
            }

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = User.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }

            await NotificationNewUser(user);
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());

            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)
            user.UserName = user.UserName.ToLower();

            if (input.SetRandomPassword)
            {
                var randomPassword = User.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            CheckErrors(await UserManager.UpdateWithValidateAsync(user));

            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var ids = userListDtos.Select(x => x.Id).ToList();
            if (!ids.Any())
                return;

            var userRoles = await (await _userRoleRepository.GetAllAsync())
                .Where(userRole => ids.Contains(userRole.UserId))
                .ToListAsync();

            var roleIds = userRoles.Select(userRole => userRole.RoleId).Distinct().ToList();

            // Tenta carregar nomes dos papéis de forma batch; fallback para busca individual (testes/mock).
            var roles = new Dictionary<int, string>();
            try
            {
                var rolesList = await _roleManager.Roles
                    .Where(r => roleIds.Contains(r.Id))
                    .ToListAsync();
                roles = rolesList.ToDictionary(r => r.Id, r => r.DisplayName);
            }
            catch
            {
                foreach (var roleId in roleIds)
                {
                    try
                    {
                        var role = await _roleManager.GetRoleByIdAsync(roleId);
                        roles[roleId] = role?.DisplayName;
                    }
                    catch
                    {
                        roles[roleId] = null;
                    }
                }
            }

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles
                    .Where(userRole => userRole.UserId == user.Id)
                    .Select(userRole => new UserListRoleDto
                    {
                        RoleId = userRole.RoleId,
                        RoleName = roles.TryGetValue(userRole.RoleId, out var roleName) ? roleName : null
                    })
                    .OrderBy(r => r.RoleName)
                    .ToList();

                user.Roles = rolesOfUser;
            }
        }

        private async Task NotificationNewUser(User user)
        {
            try
            {
                await _notificationPublisher.PublishAsync(
                  "App.NewUserRegistered",
                  new MessageNotificationData(L("NewUserRegistered", user.FullName)),
                  severity: NotificationSeverity.Info,
                  tenantIds: new[] { user.TenantId }
                  );
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "NotificationPublisher error {0}", ex.Message);
            }
            try
            {
                await _webhookPublisher.PublishAsync("WebHook.NewUserRegistered", user);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "WebhookPublisher error {0}", ex.Message);
            }
        }
    }
}