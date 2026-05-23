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
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MiddlewareAppConsts.LocalizationSourceName);
        }
    }
}