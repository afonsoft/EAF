namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a interface IChatFeatureChecker.
    /// </summary>
    public interface IChatFeatureChecker
    {
        void CheckChatFeatures(int? sourceTenantId, int? targetTenantId);

        bool CheckChatGroupFeature(int? sourceTenantId);
    }
}