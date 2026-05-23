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
            CheckChatFeaturesInternal(targetTenantId, sourceTenantId, ChatSide.Receiver);
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
            var localizationPosfix = side == ChatSide.Sender ? "ForSender" : "ForReceiver";
            if (sourceTenantId.HasValue)
            {
                if (!_featureChecker.IsEnabled(sourceTenantId.Value, AppFeatures.ChatFeature))
                {
                    throw new UserFriendlyException(L("ChatFeatureIsNotEnabled" + localizationPosfix));
                }

                if (targetTenantId.HasValue)
                {
                    if (sourceTenantId == targetTenantId)
                    {
                        return;
                    }

                    if (!_featureChecker.IsEnabled(sourceTenantId.Value, AppFeatures.TenantToTenantChatFeature))
                    {
                        throw new UserFriendlyException(L("TenantToTenantChatFeatureIsNotEnabled" + localizationPosfix));
                    }
                }
                else
                {
                    if (!_featureChecker.IsEnabled(sourceTenantId.Value, AppFeatures.TenantToHostChatFeature))
                    {
                        throw new UserFriendlyException(L("TenantToHostChatFeatureIsNotEnabled" + localizationPosfix));
                    }
                }
            }
            else
            {
                if (targetTenantId.HasValue && !_featureChecker.IsEnabled(targetTenantId.Value, AppFeatures.TenantToHostChatFeature))
                {
                    throw new UserFriendlyException(L("TenantToHostChatFeatureIsNotEnabled" + (side == ChatSide.Sender ? "ForReceiver" : "ForSender")));
                }
            }
        }
    }
}