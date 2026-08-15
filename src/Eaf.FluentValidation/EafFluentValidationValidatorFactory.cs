using Abp.Dependency;
using System;

namespace Eaf.FluentValidation
{
    /// <summary>
    /// Fábrica responsável por resolver implementações de <see cref="global::FluentValidation.IValidator{T}"/> a partir do tipo validado.
    /// </summary>
    public class EafFluentValidationValidatorFactory : ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Cria uma nova instância da fábrica.
        /// </summary>
        /// <param name="iocResolver">Resolvedor de dependências do Castle Windsor.</param>
        public EafFluentValidationValidatorFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// Obtém o validator registrado para o tipo informado.
        /// </summary>
        /// <param name="type">Tipo do objeto a ser validado.</param>
        /// <returns>Validator não genérico quando encontrado; caso contrário, <c>null</c>.</returns>
        public virtual global::FluentValidation.IValidator GetValidator(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var validatorType = typeof(global::FluentValidation.IValidator<>).MakeGenericType(type);
            if (!_iocResolver.IsRegistered(validatorType))
            {
                return null;
            }

            return _iocResolver.Resolve(validatorType) as global::FluentValidation.IValidator;
        }
    }
}
