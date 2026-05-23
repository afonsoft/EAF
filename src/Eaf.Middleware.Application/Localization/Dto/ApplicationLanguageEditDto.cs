using Abp.Localization;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe ApplicationLanguageEditDto.
    /// </summary>
    public class ApplicationLanguageEditDto
    {
        [StringLength(ApplicationLanguage.MaxIconLength)]
        public virtual string Icon { get; set; }

        public virtual int? Id { get; set; }

        /// <summary>
        /// Mapped from Language.IsDisabled with using manual mapping in CustomDtoMapper.cs
        /// </summary>
        public bool IsEnabled { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}