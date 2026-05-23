namespace Eaf.Middleware.Configuration.Dto
{
    /// <summary>
    /// Representa a classe ThemeMenuSettingsDto.
    /// </summary>
    public class ThemeMenuSettingsDto
    {
        /// <summary>
        /// Obtém ou define AllowAsideHiding.
        /// </summary>
        public bool AllowAsideHiding { get; set; }
        /// <summary>
        /// Obtém ou define AllowAsideMinimizing.
        /// </summary>
        public bool AllowAsideMinimizing { get; set; }
        /// <summary>
        /// Obtém ou define AsideSkin.
        /// </summary>
        public string AsideSkin { get; set; }
        /// <summary>
        /// Obtém ou define DefaultHiddenAside.
        /// </summary>
        public bool DefaultHiddenAside { get; set; }
        /// <summary>
        /// Obtém ou define DefaultMinimizedAside.
        /// </summary>
        public bool DefaultMinimizedAside { get; set; }
        /// <summary>
        /// Obtém ou define FixedAside.
        /// </summary>
        public bool FixedAside { get; set; }
        /// <summary>
        /// Obtém ou define Position.
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Obtém ou define SubmenuToggle.
        /// </summary>
        public string SubmenuToggle { get; set; }
    }
}