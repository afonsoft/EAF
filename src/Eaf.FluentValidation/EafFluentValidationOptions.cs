using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eaf.FluentValidation
{
    /// <summary>
    /// Opções de configuração do módulo EAF para integração com FluentValidation.
    /// </summary>
    [Serializable]
    public class EafFluentValidationOptions : IOptions<EafFluentValidationOptions>
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafFluentValidationOptions"/>.
        /// </summary>
        public EafFluentValidationOptions()
        {
            ValidatorAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Assemblies onde os validadores <see cref="global::FluentValidation.IValidator{T}"/> serão buscados.
        /// </summary>
        public List<Assembly> ValidatorAssemblies { get; set; }

        /// <summary>
        /// Instância das opções para compatibilidade com <see cref="IOptions{TOptions}"/>.
        /// </summary>
        EafFluentValidationOptions IOptions<EafFluentValidationOptions>.Value => this;
    }
}
