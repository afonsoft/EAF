using Abp;
using Abp.Application.Features;
using Abp.Localization;
using Abp.UI.Inputs;

namespace Eaf.Middleware.Web.Features
{
    /// <summary>
    /// Representa a classe MiddlewareFeatureProvider.
    /// </summary>
    public class MiddlewareFeatureProvider : FeatureProvider
    {
        /// <summary>
        /// SetFeatures.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var chatFeature = context.Create(
                AppFeatures.ChatFeature,
                defaultValue: "true",
                displayName: L(AppFeatures.ChatFeature),
                inputType: new CheckboxInputType(),
                scope: FeatureScopes.Tenant
            );
            chatFeature.CreateChildFeature(
               AppFeatures.TenantToHostChatFeature,
               defaultValue: "true",
               displayName: L(AppFeatures.TenantToHostChatFeature),
               inputType: new CheckboxInputType(),
               scope: FeatureScopes.Tenant
           );
            chatFeature.CreateChildFeature(
              AppFeatures.GroupChatFeature,
              defaultValue: "true",
              displayName: L(AppFeatures.GroupChatFeature),
              inputType: new CheckboxInputType(),
              scope: FeatureScopes.Tenant
          );
            chatFeature.CreateChildFeature(
               AppFeatures.TenantToTenantChatFeature,
               defaultValue: "false",
               displayName: L(AppFeatures.TenantToTenantChatFeature),
               inputType: new CheckboxInputType(),
               scope: FeatureScopes.Tenant
           );

            var planFeatures = context.Create(
                AppFeatures.PlanFeatures,
                defaultValue: "true",
                displayName: L(AppFeatures.PlanFeatures),
                inputType: new CheckboxInputType(),
                scope: FeatureScopes.Edition
            );

            planFeatures.CreateChildFeature(
                AppFeatures.PlanFeaturesMaxUserCount,
                defaultValue: "0",
                displayName: L(AppFeatures.PlanFeaturesMaxUserCount),
                inputType: new SingleLineStringInputType(),
                scope: FeatureScopes.Edition
            );

            planFeatures.CreateChildFeature(
                AppFeatures.PlanFeaturesMaxOrganizationUnitCount,
                defaultValue: "0",
                displayName: L(AppFeatures.PlanFeaturesMaxOrganizationUnitCount),
                inputType: new SingleLineStringInputType(),
                scope: FeatureScopes.Edition
            );

            planFeatures.CreateChildFeature(
                AppFeatures.PlanFeaturesApiCallLimit,
                defaultValue: "0",
                displayName: L(AppFeatures.PlanFeaturesApiCallLimit),
                inputType: new SingleLineStringInputType(),
                scope: FeatureScopes.Edition
            );

            planFeatures.CreateChildFeature(
                AppFeatures.PlanFeaturesStorageLimitGb,
                defaultValue: "0",
                displayName: L(AppFeatures.PlanFeaturesStorageLimitGb),
                inputType: new SingleLineStringInputType(),
                scope: FeatureScopes.Edition
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MiddlewareAppConsts.LocalizationSourceName);
        }
    }
}