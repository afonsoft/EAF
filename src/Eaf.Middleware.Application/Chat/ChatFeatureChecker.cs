using Abp.Application.Features;
using Abp.UI;

namespace Eaf.Middleware.Chat
{
    /// <summary>
    /// Representa a classe ChatFeatureChecker.
    /// </summary>
    public class ChatFeatureChecker : MiddlewareAppServiceBase, IChatFeatureChecker
    {
        private readonly IFeatureChecker _featureChecker;

        /// <summary>
        /// ChatFeatureChecker.
        /// </summary>
        /// <param name="featureChecker">Parâmetro featureChecker.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatFeatureChecker(
            IFeatureChecker featureChecker
        )
        {
            _featureChecker = featureChecker;
        }

        /// <summary>
        /// CheckChatFeatures.
        /// </summary>
        /// <param name="sourceTenantId">Parâmetro sourceTenantId.</param>
        /// <param name="targetTenantId">Parâmetro targetTenantId.</param>
        public void CheckChatFeatures(int? sourceTenantId, int? targetTenantId)
        {
            CheckChatFeaturesInternal(sourceTenantId, targetTenantId, ChatSide.Sender);
            CheckChatFeaturesInternal(targetTenantId, sourceTenantId, ChatSide.Receiver); // NOSONAR
        }

        /// <summary>
        /// CheckChatGroupFeature.
        /// </summary>
        /// <param name="sourceTenantId">Parâmetro sourceTenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public bool CheckChatGroupFeature(int? sourceTenantId)
        {
            return _featureChecker.IsEnabled(sourceTenantId.Value, AppFeatures.GroupChatFeature);
        }

        private void CheckChatFeaturesInternal(int? sourceTenantId, int? targetTenantId, ChatSide side)
        {
            var localizationSuffix = GetSideSuffix(side);
            if (!sourceTenantId.HasValue)
            {
                if (targetTenantId.HasValue)
                    AssertFeatureEnabled(targetTenantId.Value, AppFeatures.TenantToHostChatFeature, GetTenantToHostChatFeatureErrorKey(side));
                return;
            }

            AssertFeatureEnabled(sourceTenantId.Value, AppFeatures.ChatFeature, "ChatFeatureIsNotEnabled" + localizationSuffix);

            if (!targetTenantId.HasValue)
            {
                AssertFeatureEnabled(sourceTenantId.Value, AppFeatures.TenantToHostChatFeature, "TenantToHostChatFeatureIsNotEnabled" + localizationSuffix);
                return;
            }

            if (sourceTenantId == targetTenantId)
                return;

            AssertFeatureEnabled(sourceTenantId.Value, AppFeatures.TenantToTenantChatFeature, "TenantToTenantChatFeatureIsNotEnabled" + localizationSuffix);
        }

        private static string GetSideSuffix(ChatSide side)
        {
            return side == ChatSide.Sender ? "ForSender" : "ForReceiver";
        }

        private static string GetTenantToHostChatFeatureErrorKey(ChatSide side)
        {
            return "TenantToHostChatFeatureIsNotEnabled" + (side == ChatSide.Sender ? "ForReceiver" : "ForSender");
        }

        private void AssertFeatureEnabled(int tenantId, string featureName, string errorKey)
        {
            if (!_featureChecker.IsEnabled(tenantId, featureName))
                throw new UserFriendlyException(L(errorKey));
        }
    }
}