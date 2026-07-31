using Abp;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Eaf.Middleware.Authorization
{
    /// <summary>
    /// Representa a classe MiddlewareAuthorizationProvider.
    /// </summary>
    public class MiddlewareAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        /// <summary>
        /// MiddlewareAuthorizationProvider.
        /// </summary>
        /// <param name="isMultiTenancyEnabled">Parâmetro isMultiTenancyEnabled.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        /// <summary>
        /// MiddlewareAuthorizationProvider.
        /// </summary>
        /// <param name="multiTenancyConfig">Parâmetro multiTenancyConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        /// <summary>
        /// SetPermissions.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(MiddlewarePermissions.Pages) ?? context.CreatePermission(MiddlewarePermissions.Pages, L("Pages"));
            pages.CreateChildPermission(MiddlewarePermissions.Pages_Dashboard, L("Dashboard"));

            var administration = pages.CreateChildPermission(MiddlewarePermissions.Pages_Administration, L("Administration"));

            var roles = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Users_Delegation, L("UserDelegation"));

            var languages = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"));
            languages.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"));
            languages.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"));
            languages.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_AuditLogs, L("AuditLogs"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Settings, L("Settings"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_UiCustomization, L("VisualSettings"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var tenants = pages.CreateChildPermission(MiddlewarePermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            var editions = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);

            var organizationUnits = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits_Create, L("CreatingNewOrganizationUnit"));
            organizationUnits.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits_Edit, L("EditingOrganizationUnit"));
            organizationUnits.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits_Delete, L("DeletingOrganizationUnit"));
            organizationUnits.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingOrganizationUnitMembers"));
            organizationUnits.CreateChildPermission(MiddlewarePermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingOrganizationUnitRoles"));

            var massNotifications = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_MassNotifications, L("MassNotifications"));
            massNotifications.CreateChildPermission(MiddlewarePermissions.Pages_Administration_MassNotifications_Create, L("CreatingNewMassNotification"));
            massNotifications.CreateChildPermission(MiddlewarePermissions.Pages_Administration_MassNotifications_Delete, L("CancelingMassNotification"));

            var payments = administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Payments, L("Payments"));
            payments.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Payments_Create, L("CreatingNewPayment"));
            payments.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Payments_Process, L("ProcessingPayment"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpConsts.LocalizationSourceName);
        }
    }
}