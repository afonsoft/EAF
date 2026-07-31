namespace Eaf.Middleware
{
    /// <summary>
    /// Representa a classe AppFeatures.
    /// </summary>
    public static class AppFeatures
    {
        public const string ChatFeature = "App.ChatFeature";
        public const string TenantToHostChatFeature = "App.ChatFeature.TenantToHost";
        public const string TenantToTenantChatFeature = "App.ChatFeature.TenantToTenant";
        public const string GroupChatFeature = "App.ChatFeature.GroupChat";

        public const string PlanFeatures = "App.PlanFeatures";
        public const string PlanFeaturesMaxUserCount = "App.PlanFeatures.MaxUserCount";
        public const string PlanFeaturesMaxOrganizationUnitCount = "App.PlanFeatures.MaxOrganizationUnitCount";
        public const string PlanFeaturesApiCallLimit = "App.PlanFeatures.ApiCallLimit";
        public const string PlanFeaturesStorageLimitGb = "App.PlanFeatures.StorageLimitGb";
    }
}