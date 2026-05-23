using Abp.Localization;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe SetDefaultLanguageInput.
    /// </summary>
    public class SetDefaultLanguageInput
    {
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}