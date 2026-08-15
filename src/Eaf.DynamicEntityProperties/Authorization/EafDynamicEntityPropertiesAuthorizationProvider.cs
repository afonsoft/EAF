using Abp.Authorization;
using Abp.Localization;

namespace Eaf.DynamicEntityProperties.Authorization
{
    /// <summary>
    /// Permission definitions for dynamic entity properties administration.
    /// </summary>
    public class EafDynamicEntityPropertiesAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var dynamicProperties = context.GetPermissionOrNull(EafDynamicEntityPropertiesPermissions.DynamicProperties)
                ?? context.CreatePermission(
                    EafDynamicEntityPropertiesPermissions.DynamicProperties,
                    new FixedLocalizableString("Dynamic Properties"));

            dynamicProperties.CreateChildPermission(
                EafDynamicEntityPropertiesPermissions.DynamicProperties_Create,
                new FixedLocalizableString("Create Dynamic Properties"));

            dynamicProperties.CreateChildPermission(
                EafDynamicEntityPropertiesPermissions.DynamicProperties_Edit,
                new FixedLocalizableString("Edit Dynamic Properties"));

            dynamicProperties.CreateChildPermission(
                EafDynamicEntityPropertiesPermissions.DynamicProperties_Delete,
                new FixedLocalizableString("Delete Dynamic Properties"));
        }
    }
}
