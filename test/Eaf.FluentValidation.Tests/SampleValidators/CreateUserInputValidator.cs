using global::FluentValidation;

namespace Eaf.FluentValidation.Tests.SampleValidators
{
    /// <summary>
    /// Validador FluentValidation de exemplo para <see cref="CreateUserInput"/>.
    /// </summary>
    public class CreateUserInputValidator : global::FluentValidation.AbstractValidator<CreateUserInput>
    {
        /// <summary>
        /// Inicializa as regras de validação.
        /// </summary>
        public CreateUserInputValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("E-mail inválido.");
            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres.");
        }
    }
}
