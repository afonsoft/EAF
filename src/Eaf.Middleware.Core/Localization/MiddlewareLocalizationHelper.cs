using Abp.Localization;
using Abp.Localization.Sources;
using System;
using System.Globalization;

namespace Eaf.Middleware.Localization
{
    /// <summary>
    /// Helper de localização com fallback para múltiplos sources.
    /// Busca a chave em EafCore, depois Abp, AbpZero e demais sources registrados.
    /// Equivalente ao mecanismo de fallback do LocalizePipe no Angular.
    /// </summary>
    public static class MiddlewareLocalizationHelper
    {
        /// <summary>
        /// Nome do source de localização principal do EAF middleware.
        /// </summary>
        public const string DefaultSourceName = "EafCore";

        /// <summary>
        /// Nomes dos sources de localização na ordem de busca.
        /// </summary>
        public static readonly string[] SourceNames =
        [
            "EafCore",
            "Abp",
            "AbpZero",
            "AbpWeb",
            "EafAzureActiveDirectory",
            "EafLdap"
        ];

        /// <summary>
        /// Obtém a string localizada buscando em múltiplos sources com fallback.
        /// </summary>
        /// <param name="localizationManager">Gerenciador de localização do ABP.</param>
        /// <param name="key">Chave de localização.</param>
        /// <returns>Texto localizado ou a chave original se não encontrada.</returns>
        public static string Localize(ILocalizationManager localizationManager, string key)
        {
            return Localize(localizationManager, key, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Obtém a string localizada buscando em múltiplos sources com fallback.
        /// </summary>
        /// <param name="localizationManager">Gerenciador de localização do ABP.</param>
        /// <param name="key">Chave de localização.</param>
        /// <param name="args">Argumentos de formatação.</param>
        /// <returns>Texto localizado formatado ou a chave original se não encontrada.</returns>
        public static string Localize(ILocalizationManager localizationManager, string key, params object[] args)
        {
            var result = Localize(localizationManager, key, CultureInfo.CurrentUICulture);
            if (args != null && args.Length > 0)
            {
                return string.Format(result, args);
            }
            return result;
        }

        /// <summary>
        /// Obtém a string localizada buscando em múltiplos sources com fallback para uma cultura específica.
        /// </summary>
        /// <param name="localizationManager">Gerenciador de localização do ABP.</param>
        /// <param name="key">Chave de localização.</param>
        /// <param name="culture">Cultura para localização.</param>
        /// <returns>Texto localizado ou a chave original se não encontrada.</returns>
        public static string Localize(ILocalizationManager localizationManager, string key, CultureInfo culture)
        {
            if (localizationManager == null || string.IsNullOrEmpty(key))
            {
                return key;
            }

            foreach (var sourceName in SourceNames)
            {
                try
                {
                    var source = localizationManager.GetSource(sourceName);
                    var result = source.GetStringOrNull(key, culture);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // Source não registrado, tentar o próximo
                }
            }

            return key;
        }

        /// <summary>
        /// Obtém a string localizada buscando em múltiplos sources com fallback para uma cultura específica com formatação.
        /// </summary>
        /// <param name="localizationManager">Gerenciador de localização do ABP.</param>
        /// <param name="key">Chave de localização.</param>
        /// <param name="culture">Cultura para localização.</param>
        /// <param name="args">Argumentos de formatação.</param>
        /// <returns>Texto localizado formatado ou a chave original se não encontrada.</returns>
        public static string Localize(ILocalizationManager localizationManager, string key, CultureInfo culture, params object[] args)
        {
            var result = Localize(localizationManager, key, culture);
            if (args != null && args.Length > 0)
            {
                return string.Format(result, args);
            }
            return result;
        }
    }
}
