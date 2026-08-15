using Abp.Dependency;
using Abp.Runtime.Validation.Interception;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Eaf.FluentValidation
{
    /// <summary>
    /// Adaptador que executa validadores FluentValidation dentro do pipeline de validação do ABP.
    /// </summary>
    public class EafFluentValidationMethodParameterValidator : IMethodParameterValidator, ITransientDependency
    {
        private readonly EafFluentValidationValidatorFactory _validatorFactory;

        /// <summary>
        /// Cria uma nova instância do validador de parâmetros.
        /// </summary>
        /// <param name="validatorFactory">Fábrica de resolução de validators.</param>
        public EafFluentValidationMethodParameterValidator(EafFluentValidationValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        /// <summary>
        /// Valida o objeto utilizando um <see cref="global::FluentValidation.IValidator{T}"/> registrado,
        /// mapeando os erros para o formato esperado pelo ABP.
        /// </summary>
        /// <param name="validatingObject">Objeto a ser validado.</param>
        /// <returns>Lista de erros de validação no formato DataAnnotations.</returns>
        public virtual IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            if (validatingObject == null)
            {
                return new List<ValidationResult>();
            }

            var validator = _validatorFactory.GetValidator(validatingObject.GetType());
            if (validator == null)
            {
                return new List<ValidationResult>();
            }

            var fluentValidationResult = validator.Validate(new global::FluentValidation.ValidationContext<object>(validatingObject));

            if (fluentValidationResult.IsValid)
            {
                return new List<ValidationResult>();
            }

            return fluentValidationResult.Errors
                .Select(MapFailure)
                .ToList();
        }

        /// <summary>
        /// Converte um <see cref="global::FluentValidation.Results.ValidationFailure"/> em <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="failure">Falha de validação do FluentValidation.</param>
        /// <returns>Resultado de validação compatível com ABP.</returns>
        protected virtual ValidationResult MapFailure(global::FluentValidation.Results.ValidationFailure failure)
        {
            var memberNames = string.IsNullOrEmpty(failure.PropertyName)
                ? Array.Empty<string>()
                : new[] { failure.PropertyName };

            return new ValidationResult(failure.ErrorMessage, memberNames);
        }
    }
}
