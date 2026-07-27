using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Localization;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Implementação genérica de replicação de roles/permissões do host para tenants.
    /// </summary>
    public class TenantRolePermissionReplicationService : DomainService, ITenantRolePermissionReplicationService
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        /// <summary>
        /// Obtém ou define a sessão ABP.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// TenantRolePermissionReplicationService.
        /// </summary>
        public TenantRolePermissionReplicationService(
            IRepository<Tenant> tenantRepository,
            RoleManager roleManager,
            IPermissionManager permissionManager)
        {
            _tenantRepository = tenantRepository;
            _roleManager = roleManager;
            _permissionManager = permissionManager;

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
        /// Garante que a role exista no tenant e atribui as permissões informadas.
        /// Executa dentro de <see cref="IActiveUnitOfWork.SetTenantId(int?, bool)"/> + EnableFilter(MayHaveTenant).
        /// </summary>
        [UnitOfWork]
        public virtual async Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames)
        {
            var tenantList = await _tenantRepository.GetAllListAsync(t => t.Id == tenantId);
            if (!tenantList.Any())
                throw new UserFriendlyException(L("TenantNotFound"));

            using (CurrentUnitOfWork.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new Role(tenantId, roleName, roleName);
                    var createResult = await _roleManager.CreateAsync(role);
                    if (!createResult.Succeeded)
                        throw new UserFriendlyException(createResult.Errors.FirstOrDefault()?.Description ?? L("FailedToCreateTenantRole"));
                }

                var permissionList = permissionNames?.ToList() ?? new List<string>();
                if (permissionList.Any())
                {
                    var allPermissions = _permissionManager.GetAllPermissions().ToList();
                    var permissionsToGrant = allPermissions.Where(p => permissionList.Contains(p.Name)).ToList();

                    if (permissionsToGrant.Any())
                        await _roleManager.SetGrantedPermissionsAsync(role, permissionsToGrant);
                }
            }
        }

        /// <summary>
        /// Copia as permissões da role host para a role do tenant.
        /// </summary>
        [UnitOfWork]
        public virtual async Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName)
        {
            using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var hostRole = await _roleManager.FindByNameAsync(roleName);
                if (hostRole == null)
                    return;

                var hostPermissions = (await _roleManager.GetGrantedPermissionsAsync(hostRole))
                    .Select(p => p.Name)
                    .ToList();

                await EnsureRoleInTenantAsync(tenantId, roleName, hostPermissions);
            }
        }
    }
}
