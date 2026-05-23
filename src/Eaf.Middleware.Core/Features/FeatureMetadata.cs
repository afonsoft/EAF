using Abp.Localization;
using System;

namespace Eaf.Middleware.Features
{
    /// <summary>
    /// Representa a classe FeatureMetadata.
    /// </summary>
    public class FeatureMetadata
    {
        public const string CustomFeatureKey = "FeatureMetadata";

        /// <summary>
        /// FeatureMetadata.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public FeatureMetadata()
        {
            TextHtmlColor = value => "inherit";
            IsVisibleOnPricingTable = false;
        }

        /// <summary>
        /// Obtém ou define IsVisibleOnPricingTable.
        /// </summary>
        public bool IsVisibleOnPricingTable { get; set; }
        public Func<string, string> TextHtmlColor { get; set; }
        public Func<string, ILocalizableString> ValueTextNormalizer { get; set; }
    }
}