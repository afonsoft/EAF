using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Eaf.FluentValidation
{
    /// <summary>
    /// Módulo EAF que integra FluentValidation ao pipeline de validação do ABP.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafFluentValidationModule : AbpModule
    {
        /// <summary>
        /// Pré-inicializa o módulo registrando opções e adicionando o validator ABP.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafFluentValidationOptions>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafFluentValidationOptions, IOptions<EafFluentValidationOptions>>()
                             .ImplementedBy<EafFluentValidationOptions>()
                             .LifestyleSingleton()
                );
            }

            Configuration.Validation.Validators.Add<EafFluentValidationMethodParameterValidator>();
        }

        /// <summary>
        /// Inicializa o módulo registrando componentes por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafFluentValidationModule).GetAssembly());
        }

        /// <summary>
        /// Registra todos os <see cref="global::FluentValidation.IValidator{T}"/> encontrados nas assemblies configuradas.
        /// </summary>
        public override void PostInitialize()
        {
            RegisterValidators();
        }

        /// <summary>
        /// Registra todos os <see cref="global::FluentValidation.IValidator{T}"/> encontrados nas assemblies configuradas.
        /// </summary>
        protected virtual void RegisterValidators()
        {
            var options = IocManager.Resolve<EafFluentValidationOptions>();
            var validatorInterfaceType = typeof(global::FluentValidation.IValidator<>);

            foreach (var assembly in options.ValidatorAssemblies)
            {
                if (assembly == null)
                {
                    continue;
                }

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    var validatorInterfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType)
                        .ToList();

                    foreach (var serviceType in validatorInterfaces)
                    {
                        IocManager.IocContainer.Register(
                            Component.For(serviceType)
                                     .ImplementedBy(type)
                                     .LifestyleTransient()
                        );
                    }
                }
            }
        }
    }
}
