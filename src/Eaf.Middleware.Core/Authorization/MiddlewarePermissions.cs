namespace Eaf.Middleware.Authorization
{
    /// <summary>
    /// Representa a classe MiddlewarePermissions.
    /// </summary>
    public static class MiddlewarePermissions
    {
        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Pages = "Pages";
        public const string Pages_Administration = "Pages.Administration";
        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";
        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";
        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Maintenance = "Pages.Administration.Maintenance";
        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Settings = "Pages.Administration.Settings";
        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";
        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";
        public const string Pages_Administration_Users_Delegation = "Pages.Administration.Users.Delegation";
        public const string Pages_Dashboard = "Pages.Dashboard";
        //HOST-SPECIFIC PERMISSIONS

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";

        public const string Pages_Administration_Editions = "Pages.Administration.Editions";
        public const string Pages_Administration_Editions_Create = "Pages.Administration.Editions.Create";
        public const string Pages_Administration_Editions_Delete = "Pages.Administration.Editions.Delete";
        public const string Pages_Administration_Editions_Edit = "Pages.Administration.Editions.Edit";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_Create = "Pages.Administration.OrganizationUnits.Create";
        public const string Pages_Administration_OrganizationUnits_Delete = "Pages.Administration.OrganizationUnits.Delete";
        public const string Pages_Administration_OrganizationUnits_Edit = "Pages.Administration.OrganizationUnits.Edit";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";
        public const string Pages_Administration_OrganizationUnits_ManageRoles = "Pages.Administration.OrganizationUnits.ManageRoles";

        public const string Pages_Administration_MassNotifications = "Pages.Administration.MassNotifications";
        public const string Pages_Administration_MassNotifications_Create = "Pages.Administration.MassNotifications.Create";
        public const string Pages_Administration_MassNotifications_Delete = "Pages.Administration.MassNotifications.Delete";

        public const string Pages_Administration_Payments = "Pages.Administration.Payments";
        public const string Pages_Administration_Payments_Create = "Pages.Administration.Payments.Create";
        public const string Pages_Administration_Payments_Process = "Pages.Administration.Payments.Process";
    }
}