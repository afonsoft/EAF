using Abp.Localization;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe UpdateLanguageTextInput.
    /// </summary>
    public class UpdateLanguageTextInput
    {
        [Required]
        [StringLength(ApplicationLanguageText.MaxKeyLength)]
        public string Key { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public string LanguageName { get; set; }

        [Required]
        [StringLength(ApplicationLanguageText.MaxSourceNameLength)]
        public string SourceName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(ApplicationLanguageText.MaxValueLength)]
        public string Value { get; set; }
    }
}