namespace Eaf.Middleware.Configuration.Dto
{
    /// <summary>
    /// Representa a classe ThemeHeaderSettingsDto.
    /// </summary>
    public class ThemeHeaderSettingsDto
    {
        /// <summary>
        /// Obtém ou define DesktopFixedHeader.
        /// </summary>
        public bool DesktopFixedHeader { get; set; }

        /// <summary>
        /// Obtém ou define HeaderSkin.
        /// </summary>
        public string HeaderSkin { get; set; }
        /// <summary>
        /// Obtém ou define MobileFixedHeader.
        /// </summary>
        public bool MobileFixedHeader { get; set; }
    }
}