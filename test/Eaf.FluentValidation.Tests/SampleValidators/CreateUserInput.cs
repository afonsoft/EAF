using System.ComponentModel.DataAnnotations;

namespace Eaf.FluentValidation.Tests.SampleValidators
{
    /// <summary>
    /// DTO de exemplo para testes de integração FluentValidation + DataAnnotations.
    /// </summary>
    public class CreateUserInput
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
