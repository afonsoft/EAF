namespace Eaf.DynamicEntityProperties.Authorization
{
    /// <summary>
    /// Permission name constants for the dynamic entity properties module.
    /// </summary>
    public static class EafDynamicEntityPropertiesPermissions
    {
        /// <summary>
        /// Root permission for managing dynamic properties.
        /// </summary>
        public const string DynamicProperties = "Pages.Administration.DynamicProperties";

        /// <summary>
        /// Permission for creating dynamic properties and their values.
        /// </summary>
        public const string DynamicProperties_Create = "Pages.Administration.DynamicProperties.Create";

        /// <summary>
        /// Permission for editing dynamic properties and their values.
        /// </summary>
        public const string DynamicProperties_Edit = "Pages.Administration.DynamicProperties.Edit";

        /// <summary>
        /// Permission for deleting dynamic properties and their values.
        /// </summary>
        public const string DynamicProperties_Delete = "Pages.Administration.DynamicProperties.Delete";
    }
}
