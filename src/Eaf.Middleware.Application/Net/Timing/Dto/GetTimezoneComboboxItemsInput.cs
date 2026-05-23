using Abp.Configuration;

namespace Eaf.Middleware.Timing.Dto
{
    /// <summary>
    /// Representa a classe GetTimezoneComboboxItemsInput.
    /// </summary>
    public class GetTimezoneComboboxItemsInput
    {
        /// <summary>
        /// Obtém ou define DefaultTimezoneScope.
        /// </summary>
        public SettingScopes DefaultTimezoneScope { get; set; }

        /// <summary>
        /// Obtém ou define SelectedTimezoneId.
        /// </summary>
        public string SelectedTimezoneId { get; set; }
    }
}