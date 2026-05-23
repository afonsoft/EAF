namespace Eaf.Middleware.Configuration.Host.Dto
{
    /// <summary>
    /// Representa a classe GoogleSettingsEditDto.
    /// </summary>
    public class GoogleSettingsEditDto
    {
        /// <summary>
        /// Obtém ou define Analytics.
        /// </summary>
        public string Analytics { get; set; }
        /// <summary>
        /// Obtém ou define RecaptchaSiteKey.
        /// </summary>
        public string RecaptchaSiteKey { get; set; }
        /// <summary>
        /// Obtém ou define Tag.
        /// </summary>
        public string Tag { get; set; }
    }
}