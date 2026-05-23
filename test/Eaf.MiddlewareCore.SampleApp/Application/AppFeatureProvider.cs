using Abp.Application.Features;
using Abp.UI.Inputs;

using static Eaf.MiddlewareCore.SampleApp.Application.AppLocalizationHelper;

namespace Eaf.MiddlewareCore.SampleApp.Application
{
    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                AppFeaturesSample.SimpleBooleanFeature,
                defaultValue: "false",
                displayName: L("SimpleBooleanFeature"),
                inputType: new CheckboxInputType()
            );

            context.Create(
                AppFeaturesSample.SimpleIntFeature,
                defaultValue: "0",
                displayName: L("SimpleIntFeature")
            );
        }
    }
}