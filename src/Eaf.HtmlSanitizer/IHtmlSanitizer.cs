namespace Eaf.HtmlSanitizer
{
    /// <summary>
    /// Serviço de sanitização de HTML contra ataques XSS.
    /// </summary>
    public interface IHtmlSanitizer
    {
        /// <summary>
        /// Sanitiza o HTML removendo tags, atributos e URIs perigosos.
        /// </summary>
        /// <param name="html">HTML a ser sanitizado.</param>
        /// <param name="options">Opções opcionais de sanitização.</param>
        /// <returns>HTML seguro ou <see cref="string.Empty"/> quando a entrada for nula ou vazia.</returns>
        string Sanitize(string html, EafHtmlSanitizerOptions options = null);
    }
}
