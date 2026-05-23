using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Eaf.Middleware.Localization
{
    /// <summary>
    /// Representa a classe MiddlewareLocalizationConfigurer.
    /// </summary>
    public static class MiddlewareLocalizationConfigurer
    {
        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="localizationConfiguration">Parâmetro localizationConfiguration.</param>
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
               new DictionaryBasedLocalizationSource(
                   "EafCore",
                   new XmlEmbeddedFileLocalizationDictionaryProvider(
                       typeof(MiddlewareLocalizationConfigurer).GetAssembly(),
                       "Eaf.Middleware.Core.Localization.Source"
                   )
               )
           );
        }
    }
}