using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Localization;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Implementação de <see cref="ITenantUserManager"/>.
    /// Gerencia memberships de usuários host em tenants, criando shadow users e replicando roles.
    /// </summary>
    public class TenantUserManager : DomainService, ITenantUserManager
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserTenantMembership, long> _membershipRepository;
        private readonly UserManager _userManager;
        private readonly ITenantRolePermissionReplicationService _rolePermissionReplicationService;

        /// <summary>
        /// Obtém ou define a sessão ABP.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// TenantUserManager.
        /// </summary>
        public TenantUserManager(
            IRepository<Tenant> tenantRepository,
            IRepository<User, long> userRepository,
            IRepository<UserTenantMembership, long> membershipRepository,
            UserManager userManager,
            ITenantRolePermissionReplicationService rolePermissionReplicationService)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _membershipRepository = membershipRepository;
            _userManager = userManager;
            _rolePermissionReplicationService = rolePermissionReplicationService;

            AbpSession = NullAbpSession.Instance;
            LocalizationSourceName = MiddlewareLocalizationHelper.DefaultSourceName;
        }

        /// <inheritdoc/>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <inheritdoc/>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <inheritdoc/>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Garante a membership do host no tenant, criando/atualizando o shadow user e replicando roles.
        /// </summary>
        [UnitOfWork]
        public virtual async Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false)
        {
            var tenantList = await _tenantRepository.GetAllListAsync(t => t.Id == tenantId);
            if (!tenantList.Any())
                throw new UserFriendlyException(L("TenantNotFound"));

            User hostUser;
            IList<string> hostRoleNames;
            UserTenantMembership membership;

            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                hostUser = await _userRepository.GetAsync(hostUserId);

                if (hostUser.TenantId.HasValue)
                    throw new UserFriendlyException(L("OnlyHostUsersCanHaveTenantMemberships"));

                hostRoleNames = await _userManager.GetRolesAsync(hostUser);

                membership = await _membershipRepository
                    .FirstOrDefaultAsync(m => m.UserId == hostUserId && m.TenantId == tenantId);
            }

            if (membership != null)
            {
                if (isDefault)
                    await SetDefaultAsync(hostUserId, tenantId);
                return membership;
            }

            long tenantUserId;

            using (CurrentUnitOfWork.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
            {
                var shadowUser = await _userManager.FindByNameAsync(hostUser.UserName);

                if (shadowUser == null)
                {
                    shadowUser = new User
                    {
                        TenantId = tenantId,
                        UserName = hostUser.UserName,
                        Name = hostUser.Name,
                        Surname = hostUser.Surname,
                        EmailAddress = hostUser.EmailAddress,
                        IsActive = true,
                        IsEmailConfirmed = true,
                        IsLockoutEnabled = false
                    };
                    shadowUser.SetNormalizedNames();

                    var createResult = await _userManager.CreateAsync(shadowUser, GenerateShadowPassword());
                    if (!createResult.Succeeded)
                        throw new UserFriendlyException(createResult.Errors.FirstOrDefault()?.Description ?? L("FailedToCreateShadowUser"));
                }

                foreach (var roleName in hostRoleNames)
                {
                    if (await _userManager.IsInRoleAsync(shadowUser, roleName))
                        continue;

                    await _rolePermissionReplicationService.CopyRolePermissionsFromHostAsync(tenantId, roleName);

                    var addResult = await _userManager.AddToRoleAsync(shadowUser, roleName);
                    if (!addResult.Succeeded)
                        throw new UserFriendlyException(addResult.Errors.FirstOrDefault()?.Description ?? L("FailedToAddRoleToShadowUser"));
                }

                tenantUserId = shadowUser.Id;
            }

            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                membership = new UserTenantMembership
                {
                    UserId = hostUserId,
                    TenantId = tenantId,
                    TenantUserId = tenantUserId,
                    IsDefault = isDefault
                };

                await _membershipRepository.InsertAsync(membership);

                if (isDefault)
                    await ClearOtherDefaultsAsync(hostUserId, tenantId);
            }

            return membership;
        }

        /// <summary>
        /// Remove a membership do host no tenant.
        /// </summary>
        [UnitOfWork]
        public virtual async Task RemoveMembershipAsync(long hostUserId, int tenantId)
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var membership = await _membershipRepository
                    .FirstOrDefaultAsync(m => m.UserId == hostUserId && m.TenantId == tenantId);

                if (membership != null)
                    await _membershipRepository.DeleteAsync(membership);
            }
        }

        /// <summary>
        /// Define o tenant padrão para o usuário host.
        /// </summary>
        [UnitOfWork]
        public virtual async Task SetDefaultAsync(long hostUserId, int tenantId)
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var membership = await _membershipRepository
                    .FirstOrDefaultAsync(m => m.UserId == hostUserId && m.TenantId == tenantId);

                if (membership == null)
                    throw new UserFriendlyException(L("TenantMembershipNotFound"));

                membership.IsDefault = true;
                await ClearOtherDefaultsAsync(hostUserId, tenantId);
            }
        }

        /// <summary>
        /// Retorna o id do shadow user dentro do tenant.
        /// </summary>
        public virtual async Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId)
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var membership = await _membershipRepository
                    .FirstOrDefaultAsync(m => m.UserId == hostUserId && m.TenantId == tenantId);

                return membership?.TenantUserId;
            }
        }

        /// <summary>
        /// Lista todas as memberships de um usuário host.
        /// </summary>
        public virtual async Task<IReadOnlyList<UserTenantMembership>> GetMembershipsAsync(long hostUserId)
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var memberships = await _membershipRepository.GetAllListAsync(m => m.UserId == hostUserId);
                return memberships.AsReadOnly();
            }
        }

        private async Task ClearOtherDefaultsAsync(long hostUserId, int exceptTenantId)
        {
            var defaults = await _membershipRepository.GetAllListAsync(
                m => m.UserId == hostUserId && m.IsDefault && m.TenantId != exceptTenantId);

            foreach (var item in defaults)
            {
                item.IsDefault = false;
            }
        }

        /// <summary>
        /// Gera uma senha temporária para o shadow user que atende às regras padrão do Identity.
        /// </summary>
        private static string GenerateShadowPassword()
        {
            // Guid hex contém letras minúsculas e dígitos; adiciona maiúscula, dígito e caractere não alfanumérico.
            return $"{Guid.NewGuid().ToString("N")[..12]}A1!";
        }
    }
}
