using Abp.Application.Features;
using Abp.Localization;

namespace Eaf.Middleware.Features
{
    /// <summary>
    /// Representa a classe FeatureExtensions.
    /// </summary>
    public static class FeatureExtensions
    {
        /// <summary>
        /// GetValueText.
        /// </summary>
        /// <param name="feature">Parâmetro feature.</param>
        /// <param name="value">Parâmetro value.</param>
        /// <param name="localizationContext">Parâmetro localizationContext.</param>
        /// <returns>Resultado da operação.</returns>
        public static string GetValueText(this Feature feature, string value, ILocalizationContext localizationContext)
        {
            var featureMetadata = feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata;
            if (featureMetadata?.ValueTextNormalizer == null)
            {
                return value;
            }

            return featureMetadata.ValueTextNormalizer(value).Localize(localizationContext);
        }
    }
}