using Abp.Authorization;
using Abp.Localization;

namespace Eaf.Workflow.Authorization
{
    /// <summary>Permission names for the workflow engine.</summary>
    public static class EafWorkflowPermissions
    {
        /// <summary>Base permission for workflow administration (definitions).</summary>
        public const string Pages_Administration_Workflows = "Pages.Administration.Workflows";

        /// <summary>Create workflow definitions.</summary>
        public const string Pages_Administration_Workflows_Create = "Pages.Administration.Workflows.Create";

        /// <summary>Edit workflow definitions.</summary>
        public const string Pages_Administration_Workflows_Edit = "Pages.Administration.Workflows.Edit";

        /// <summary>Delete workflow definitions.</summary>
        public const string Pages_Administration_Workflows_Delete = "Pages.Administration.Workflows.Delete";

        /// <summary>Publish workflow definitions.</summary>
        public const string Pages_Administration_Workflows_Publish = "Pages.Administration.Workflows.Publish";

        /// <summary>Start workflow requests from the catalog.</summary>
        public const string Pages_Workflows = "Pages.Workflows";

        /// <summary>Decide approval tasks in the inbox.</summary>
        public const string Pages_Workflows_Approvals = "Pages.Workflows.Approvals";
    }

    /// <summary>
    /// Permission definitions for the workflow module.
    /// </summary>
    public class EafWorkflowAuthorizationProvider : AuthorizationProvider
    {
        /// <inheritdoc />
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var workflows = context.GetPermissionOrNull(EafWorkflowPermissions.Pages_Administration_Workflows)
                ?? context.CreatePermission(
                    EafWorkflowPermissions.Pages_Administration_Workflows,
                    new FixedLocalizableString("Workflows"));

            workflows.CreateChildPermission(EafWorkflowPermissions.Pages_Administration_Workflows_Create,
                new FixedLocalizableString("Create Workflow Definitions"));
            workflows.CreateChildPermission(EafWorkflowPermissions.Pages_Administration_Workflows_Edit,
                new FixedLocalizableString("Edit Workflow Definitions"));
            workflows.CreateChildPermission(EafWorkflowPermissions.Pages_Administration_Workflows_Delete,
                new FixedLocalizableString("Delete Workflow Definitions"));
            workflows.CreateChildPermission(EafWorkflowPermissions.Pages_Administration_Workflows_Publish,
                new FixedLocalizableString("Publish Workflow Definitions"));

            var catalog = context.GetPermissionOrNull(EafWorkflowPermissions.Pages_Workflows)
                ?? context.CreatePermission(
                    EafWorkflowPermissions.Pages_Workflows,
                    new FixedLocalizableString("Workflow Requests"));

            catalog.CreateChildPermission(EafWorkflowPermissions.Pages_Workflows_Approvals,
                new FixedLocalizableString("Approve Workflow Tasks"));
        }
    }
}
