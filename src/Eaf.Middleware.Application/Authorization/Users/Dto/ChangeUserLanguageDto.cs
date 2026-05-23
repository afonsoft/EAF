using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe ChangeUserLanguageDto.
    /// </summary>
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}