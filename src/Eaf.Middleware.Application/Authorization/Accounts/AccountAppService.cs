using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using Abp.UI;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Eaf.Middleware.Authorization.Accounts
{
    /// <summary>
    /// Representa a classe AccountAppService.
    /// </summary>
    public class AccountAppService : MiddlewareAppServiceBase, IAccountAppService
    {
        private readonly IImpersonationManager _impersonationManager;
        private readonly RoleManager _roleManager;
        private readonly Core.Editions.EditionManager _editionManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IWebUrlService _webUrlService;
        private readonly IRepository<UserTenantMembership, long> _membershipRepository;
        private readonly ITenantUserManager _tenantUserManager;

        /// <summary>
        /// AccountAppService.
        /// </summary>
        public AccountAppService(
            IUserEmailer userEmailer,
            IWebUrlService webUrlService,
            IImpersonationManager impersonationManager,
            RoleManager roleManager,
            Core.Editions.EditionManager editionManager,
            IRepository<UserTenantMembership, long> membershipRepository,
            ITenantUserManager tenantUserManager)
        {
            _userEmailer = userEmailer;
            _webUrlService = webUrlService;
            _impersonationManager = impersonationManager;
            _roleManager = roleManager;
            _editionManager = editionManager;
            _membershipRepository = membershipRepository;
            _tenantUserManager = tenantUserManager;

            AppUrlService = NullAppUrlService.Instance;
        }

        /// <summary>
        /// Obtém ou define AppUrlService.
        /// </summary>
        public IAppUrlService AppUrlService { get; set; }

        [Produces("application/json", "application/json-patch+json", "text/json")]
        public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }

        /// <summary>
        /// BackToImpersonator.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public virtual async Task<ImpersonateOutput> BackToImpersonator()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
                TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
            };
        }

        /// <summary>
        /// GetAllTenants.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<List<TenantListDto>> GetAllTenants()
        {
            var query = TenantManager.Tenants.Where(d => d.IsActive);
            var tenants = await query.OrderBy(d => d.Name).ToListAsync();

            return new List<TenantListDto>(ObjectMapper.Map<List<TenantListDto>>(tenants));
        }

        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }

        /// <summary>
        /// asdasd
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        [AbpAllowAnonymous]
        public virtual async Task<RegisterOutput> Register(RegisterInput input)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowSelfRegistration))
                throw new UserFriendlyException(L("SelfRegistrationIsDisabled"));

            var hostUser = await CreateHostUserAsync(input);

            switch (input.TenantSelectionMode)
            {
                case TenantSelectionMode.DefaultTenant:
                    return await RegisterDefaultTenantAsync(hostUser);

                case TenantSelectionMode.CreateNew:
                    return await RegisterCreateNewAsync(input, hostUser);

                case TenantSelectionMode.JoinExisting:
                    return await RegisterJoinExistingAsync(input, hostUser);

                default:
                    throw new UserFriendlyException(L("InvalidRegisterRequest"));
            }
        }

        private async Task<User> CreateHostUserAsync(RegisterInput input)
        {
            var user = new User
            {
                UserName = input.UserName.ToLowerInvariant(),
                Name = input.Name,
                Surname = input.Surname,
                EmailAddress = input.EmailAddress,
                IsActive = true,
                IsEmailConfirmed = false,
                IsLockoutEnabled = true,
            };
            user.SetNormalizedNames();

            CheckErrors(await UserManager.CreateAsync(user, input.Password));
            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<RegisterOutput> RegisterDefaultTenantAsync(User hostUser)
        {
            return new RegisterOutput
            {
                CanLogin = true,
                TenantId = null,
                TenancyName = null
            };
        }

        private async Task<RegisterOutput> RegisterCreateNewAsync(RegisterInput input, User hostUser)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowTenantCreation))
                throw new UserFriendlyException(L("TenantCreationIsDisabled"));

            if (string.IsNullOrWhiteSpace(input.TenancyName))
                throw new UserFriendlyException(L("InvalidTenancyName"));

            var edition = await _editionManager.GetOrCreateDefaultEditionAsync();

            var tenant = new Tenant(input.TenancyName, input.TenantName ?? input.TenancyName)
            {
                IsActive = true,
                EditionId = edition.Id
            };

            await TenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync();

            long shadowUserId;

            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                await CurrentUnitOfWork.SaveChangesAsync();

                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                var shadowUser = new User
                {
                    TenantId = tenant.Id,
                    UserName = hostUser.UserName,
                    Name = hostUser.Name,
                    Surname = hostUser.Surname,
                    EmailAddress = hostUser.EmailAddress,
                    IsActive = true,
                    IsEmailConfirmed = false,
                    IsLockoutEnabled = true,
                };
                shadowUser.SetNormalizedNames();

                CheckErrors(await UserManager.CreateAsync(shadowUser, input.Password));
                await CurrentUnitOfWork.SaveChangesAsync();

                CheckErrors(await UserManager.AddToRoleAsync(shadowUser, StaticRoleNames.Tenants.Admin));

                var userRole = _roleManager.Roles.SingleOrDefault(r => r.Name == StaticRoleNames.Tenants.User);
                if (userRole != null)
                    CheckErrors(await UserManager.AddToRoleAsync(shadowUser, userRole.Name));

                await CurrentUnitOfWork.SaveChangesAsync();

                shadowUserId = shadowUser.Id;
            }

            using (CurrentUnitOfWork.SetTenantId(null))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var membership = new UserTenantMembership
                {
                    UserId = hostUser.Id,
                    TenantId = tenant.Id,
                    TenantUserId = shadowUserId,
                    IsDefault = true
                };

                await _membershipRepository.InsertAsync(membership);
            }

            return new RegisterOutput
            {
                CanLogin = true,
                TenantId = tenant.Id,
                TenancyName = tenant.TenancyName
            };
        }

        private async Task<RegisterOutput> RegisterJoinExistingAsync(RegisterInput input, User hostUser)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowJoinRequests))
                throw new UserFriendlyException(L("JoinRequestsAreDisabled"));

            if (!input.ExistingTenantId.HasValue)
                throw new UserFriendlyException(L("TenantIsNotActive"));

            var request = await _tenantUserManager.CreatePendingMembershipAsync(
                hostUser.Id,
                input.ExistingTenantId.Value,
                input.JoinRequestMessage,
                input.Password);

            return new RegisterOutput
            {
                CanLogin = false,
                TenantId = request.TenantId,
                TenancyName = (await TenantManager.FindByIdAsync(request.TenantId))?.TenancyName
            };
        }

        /// <summary>
        /// ResetPassword.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));

            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        /// <summary>
        /// ResolveTenantId.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }

            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }

        /// <summary>
        /// SendEmailActivationLink.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );
        }

        /// <summary>
        /// SendPasswordResetCode.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewPasswordResetCode();
            await _userEmailer.SendPasswordResetLinkAsync(
                user,
                AppUrlService.CreatePasswordResetUrlFormat(AbpSession.TenantId)
                );
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }

        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await UserManager.FindByEmailAsync(inputEmailAddress);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            }

            return user;
        }
    }
}