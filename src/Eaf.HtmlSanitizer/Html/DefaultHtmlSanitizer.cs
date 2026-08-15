using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.HtmlSanitizer.Html
{
    /// <summary>
    /// Implementação padrão de <see cref="IHtmlSanitizer"/> baseada no HtmlSanitizer (Ganss.XSS).
    /// </summary>
    public class DefaultHtmlSanitizer : IHtmlSanitizer, ISingletonDependency
    {
        private static readonly string[] DangerousTags = new[] { "script", "style" };
        private static readonly string[] DangerousSchemes = new[] { "javascript", "vbscript", "data" };
        private readonly EafHtmlSanitizerOptions _defaultOptions;

        /// <summary>
        /// Cria uma nova instância do sanitizer padrão.
        /// </summary>
        /// <param name="options">Opções padrão de configuração.</param>
        public DefaultHtmlSanitizer(EafHtmlSanitizerOptions options)
        {
            _defaultOptions = options ?? new EafHtmlSanitizerOptions();
        }

        /// <summary>
        /// Sanitiza o HTML removendo tags e atributos perigosos.
        /// </summary>
        /// <param name="html">HTML a ser sanitizado.</param>
        /// <param name="options">Opções opcionais de sanitização.</param>
        /// <returns>HTML seguro ou <see cref="string.Empty"/> quando a entrada for nula ou vazia.</returns>
        public virtual string Sanitize(string html, EafHtmlSanitizerOptions options = null)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            var effectiveOptions = options ?? _defaultOptions;
            var sanitizer = CreateSanitizer(effectiveOptions);

            return sanitizer.Sanitize(html);
        }

        /// <summary>
        /// Cria o sanitizer do Ganss.XSS com as opções do EAF aplicadas.
        /// </summary>
        /// <param name="options">Opções de sanitização do EAF.</param>
        /// <returns>Instância configurada do HtmlSanitizer.</returns>
        protected virtual global::Ganss.Xss.HtmlSanitizer CreateSanitizer(EafHtmlSanitizerOptions options)
        {
            var sanitizerOptions = new global::Ganss.Xss.HtmlSanitizerOptions();

            ApplySet(
                options.AllowedTags,
                global::Ganss.Xss.HtmlSanitizerDefaults.AllowedTags,
                tag => !DangerousTags.Contains(tag, StringComparer.OrdinalIgnoreCase),
                sanitizerOptions.AllowedTags);

            ApplySet(
                options.AllowedAttributes,
                global::Ganss.Xss.HtmlSanitizerDefaults.AllowedAttributes,
                attr => !IsEventHandlerAttribute(attr),
                sanitizerOptions.AllowedAttributes);

            ApplySet(
                options.AllowedUriSchemes,
                global::Ganss.Xss.HtmlSanitizerDefaults.AllowedSchemes,
                scheme => !DangerousSchemes.Contains(scheme, StringComparer.OrdinalIgnoreCase),
                sanitizerOptions.AllowedSchemes);

            ApplySet(
                options.AllowedCssProperties,
                global::Ganss.Xss.HtmlSanitizerDefaults.AllowedCssProperties,
                _ => true,
                sanitizerOptions.AllowedCssProperties);

            sanitizerOptions.UriAttributes = new HashSet<string>(global::Ganss.Xss.HtmlSanitizerDefaults.UriAttributes, StringComparer.OrdinalIgnoreCase);
            sanitizerOptions.AllowedAtRules = new HashSet<global::AngleSharp.Css.Dom.CssRuleType>(global::Ganss.Xss.HtmlSanitizerDefaults.AllowedAtRules);

            return new global::Ganss.Xss.HtmlSanitizer(sanitizerOptions);
        }

        /// <summary>
        /// Aplica as opções do EAF sobre as configurações padrão do sanitizer.
        /// </summary>
        /// <param name="userValues">Valores informados pelo usuário.</param>
        /// <param name="defaultValues">Valores padrão da biblioteca.</param>
        /// <param name="filter">Filtro de segurança para remover valores perigosos.</param>
        /// <param name="target">Conjunto alvo do HtmlSanitizer.</param>
        private static void ApplySet(
            ICollection<string> userValues,
            IEnumerable<string> defaultValues,
            Func<string, bool> filter,
            ISet<string> target)
        {
            var source = userValues != null && userValues.Count > 0
                ? userValues
                : defaultValues;

            foreach (var value in source.Where(filter))
            {
                target.Add(value);
            }
        }

        /// <summary>
        /// Determina se o nome do atributo corresponde a um manipulador de eventos.
        /// </summary>
        /// <param name="attributeName">Nome do atributo.</param>
        /// <returns><c>true</c> quando o atributo inicia com "on".</returns>
        private static bool IsEventHandlerAttribute(string attributeName)
        {
            return attributeName != null && attributeName.Length > 2
                && attributeName.StartsWith("on", StringComparison.OrdinalIgnoreCase);
        }
    }
}
