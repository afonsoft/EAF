namespace Eaf.Middleware.Configuration.Dto
{
    /// <summary>
    /// Representa a classe ThemeSettingsDto.
    /// </summary>
    public class ThemeSettingsDto
    {
        /// <summary>
        /// Obtém ou define Header.
        /// </summary>
        public ThemeHeaderSettingsDto Header { get; set; } = new ThemeHeaderSettingsDto();
        /// <summary>
        /// Obtém ou define Layout.
        /// </summary>
        public ThemeLayoutSettingsDto Layout { get; set; } = new ThemeLayoutSettingsDto();
        /// <summary>
        /// Obtém ou define Menu.
        /// </summary>
        public ThemeMenuSettingsDto Menu { get; set; } = new ThemeMenuSettingsDto();
        /// <summary>
        /// Obtém ou define Theme.
        /// </summary>
        public string Theme { get; set; }
    }
}