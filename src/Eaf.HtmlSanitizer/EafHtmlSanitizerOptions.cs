using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Eaf.HtmlSanitizer
{
    /// <summary>
    /// Opções de configuração do sanitizer de HTML do EAF.
    /// </summary>
    [Serializable]
    public class EafHtmlSanitizerOptions : IOptions<EafHtmlSanitizerOptions>
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafHtmlSanitizerOptions"/>.
        /// </summary>
        public EafHtmlSanitizerOptions()
        {
            AllowedTags = new HashSet<string>();
            AllowedAttributes = new HashSet<string>();
            AllowedUriSchemes = new HashSet<string>();
            AllowedCssProperties = new HashSet<string>();
        }

        /// <summary>
        /// Tags HTML permitidas. Valores vazios indicam uso da configuração padrão do sanitizer.
        /// </summary>
        public ICollection<string> AllowedTags { get; set; }

        /// <summary>
        /// Atributos HTML permitidos. Valores vazios indicam uso da configuração padrão do sanitizer.
        /// </summary>
        public ICollection<string> AllowedAttributes { get; set; }

        /// <summary>
        /// Esquemas de URI permitidos (por exemplo, http, https, mailto). Valores vazios indicam uso da configuração padrão.
        /// </summary>
        public ICollection<string> AllowedUriSchemes { get; set; }

        /// <summary>
        /// Propriedades CSS permitidas. Valores vazios indicam uso da configuração padrão do sanitizer.
        /// </summary>
        public ICollection<string> AllowedCssProperties { get; set; }

        /// <summary>
        /// Instância das opções para compatibilidade com <see cref="IOptions{TOptions}"/>.
        /// </summary>
        EafHtmlSanitizerOptions IOptions<EafHtmlSanitizerOptions>.Value => this;
    }
}
