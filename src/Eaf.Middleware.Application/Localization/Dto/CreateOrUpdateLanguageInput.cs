using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe CreateOrUpdateLanguageInput.
    /// </summary>
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}