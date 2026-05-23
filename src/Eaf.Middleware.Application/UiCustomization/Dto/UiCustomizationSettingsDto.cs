using Eaf.Middleware.Configuration.Dto;

namespace Eaf.Middleware.UiCustomization.Dto
{
    /// <summary>
    /// Representa a classe UiCustomizationSettingsDto.
    /// </summary>
    public class UiCustomizationSettingsDto
    {
        /// <summary>
        /// Obtém ou define AllowMenuScroll.
        /// </summary>
        public bool AllowMenuScroll { get; set; } = true;
        /// <summary>
        /// Obtém ou define BaseSettings.
        /// </summary>
        public ThemeSettingsDto BaseSettings { get; set; }

        /// <summary>
        /// Obtém ou define IsLeftMenuUsed.
        /// </summary>
        public bool IsLeftMenuUsed { get; set; }

        /// <summary>
        /// Obtém ou define IsTabMenuUsed.
        /// </summary>
        public bool IsTabMenuUsed { get; set; }
        /// <summary>
        /// Obtém ou define IsTopMenuUsed.
        /// </summary>
        public bool IsTopMenuUsed { get; set; }
    }
}